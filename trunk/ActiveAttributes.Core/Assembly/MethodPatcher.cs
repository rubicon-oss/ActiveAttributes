// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.CompileTimeAspects;
using ActiveAttributes.Core.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Patches a method aspect-ready.
  /// </summary>
  public class MethodPatcher
  {
    private TypeProvider _typeProvider;

    private readonly MethodInfo _onInterceptMethodInfo;
    private readonly MethodInfo _onInterceptGetMethodInfo;
    private readonly MethodInfo _onInterceptSetMethodInfo;

    private readonly MethodInfo _createDelegateMethodInfo;

    public MethodPatcher ()
    {
      _onInterceptMethodInfo = typeof (MethodInterceptionAspectAttribute).GetMethod ("OnIntercept");
      _onInterceptGetMethodInfo = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptGet");
      _onInterceptSetMethodInfo = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptSet");

      _createDelegateMethodInfo = typeof (Delegate).GetMethod (
          "CreateDelegate",
          new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
    }

    /// <summary>
    /// Patches a method so that it can be intercepted by applied aspects.
    /// </summary>
    /// <param name="mutableMethod">The mutable method.</param>
    /// <param name="fieldData">Field data provided by <see cref="FieldIntroducer"/>.</param>
    /// <param name="aspects">Aspects provided by <see cref="AspectsProvider"/></param>
    /// <returns>A copy of the original method.</returns>
    public void Patch (MutableMethodInfo mutableMethod, FieldIntroducer.Data fieldData, IEnumerable<CompileTimeAspectBase> aspects)
    {
      _typeProvider = new TypeProvider (mutableMethod);
      var mutableType = (MutableType) mutableMethod.DeclaringType;

      mutableMethod.SetBody (ctx => GetPatchedBody (mutableMethod, ctx, fieldData, aspects));
    }

    private Expression GetPatchedBody (MutableMethodInfo mutableMethod, BodyContextBase ctx, FieldIntroducer.Data fieldData, IEnumerable<CompileTimeAspectBase> aspects)
    {
      var aspectsAsList = aspects.ToList();

      var methodInfoFieldExpression = Expression.Field (ctx.This, fieldData.MethodInfoField);
      var delegateFieldExpression = Expression.Field (ctx.This, fieldData.DelegateField);
      var aspectsFieldExpression = Expression.Field (ctx.This, fieldData.InstanceAspectsField);

      // ic = invocationContext
      var invocationContextType = _typeProvider.GetInvocationContextType();
      var invocationContextVariableExpression = Expression.Variable (invocationContextType, "ctx");
      var invocationContextAssignExpression = GetInvocationContextAssignExpression(invocationContextVariableExpression, methodInfoFieldExpression, ctx, invocationContextType);

      var invocationVariableExpressions = GetInvocationVariableExpressions (aspectsAsList.Count);
      var invocationInitExpressions = GetInvocationInitExpressions (delegateFieldExpression, aspectsFieldExpression, invocationContextVariableExpression, invocationVariableExpressions, aspectsAsList, mutableMethod);

      var aspectCallExpression = GetLastAspectCallExpression(aspectsFieldExpression, aspectsAsList, invocationVariableExpressions, mutableMethod);

      var returnValueExpression = Expression.Property (invocationContextVariableExpression, "ReturnValue");

      return Expression.Block (
          new[] { invocationContextVariableExpression }.Concat(invocationVariableExpressions),
          invocationContextAssignExpression,
          Expression.Block(invocationInitExpressions),
          aspectCallExpression,
          returnValueExpression);
    }

    private static Expression GetInvocationContextAssignExpression (Expression invocationContextVariableExpression, Expression methodInfoFieldExpression, BodyContextBase ctx, Type invocationContextType)
    {
      var invocationContextConstructor = invocationContextType.GetConstructors().Single();

      var invocationContextArgumentExpressions = new[] { methodInfoFieldExpression, ctx.This }.Concat (ctx.Parameters.Cast<Expression>());

      var invocationContextCreateExpression = Expression.New (invocationContextConstructor, invocationContextArgumentExpressions);
      var invocationContextAssignExpression = Expression.Assign (invocationContextVariableExpression, invocationContextCreateExpression);

      return invocationContextAssignExpression;
    }

    private ParameterExpression[] GetInvocationVariableExpressions (int count)
    {
      var variables = new ParameterExpression[count];
      for (var i = 0; i < count; i++)
      {
        var invocationType = i == 0
                                 ? _typeProvider.GetInvocationType()
                                 : typeof (OuterInvocation);
        variables[i] = Expression.Variable (invocationType, "invocation" + (i + 1));
      }
      return variables;
    }

    private IEnumerable<Expression> GetInvocationInitExpressions (Expression delegateFieldExpression, Expression aspectsFieldExpression, Expression invocationContextVariableExpression, IList<Expression> invocationParameterExpressions, IList<CompileTimeAspectBase> aspectsAsList, MutableMethodInfo mutableMethod)
    {
      for (var i = 0; i < invocationParameterExpressions.Count; i++)
      {
        if (i == 0)
        {
          var invocationVariableExpression = invocationParameterExpressions[0];
          var invocationAssignExpression = GetMostInnerInvocationAssignExpression(delegateFieldExpression, invocationContextVariableExpression, invocationVariableExpression);
          yield return invocationAssignExpression;
        }
        else
        {
          var last = i - 1;
          var innerInvocationVariableExpression = invocationParameterExpressions[last];
          var innerAspectAccessExpression = Expression.ArrayAccess (aspectsFieldExpression, Expression.Constant (last));
          var innerAspectType = aspectsAsList[last].AspectType;
          var invocationVariableExpression = invocationParameterExpressions[i];

          var innerInvocationAssignExpression = GetOuterInvocationAssignExpression(invocationVariableExpression, invocationContextVariableExpression, innerAspectAccessExpression, innerInvocationVariableExpression, innerAspectType, mutableMethod);
          yield return innerInvocationAssignExpression;
        }
      }
    }

    private Expression GetMostInnerInvocationAssignExpression (Expression delegateFieldExpression, Expression invocationContextVariableExpression, Expression invocationParameterExpression)
    {
      var invocationType = _typeProvider.GetInvocationType();
      var invocationConstructor = invocationType.GetConstructors().Single();

      var invocationCreateExpression = Expression.New (invocationConstructor, invocationContextVariableExpression, delegateFieldExpression);
      var invocationAssignExpression = Expression.Assign (invocationParameterExpression, invocationCreateExpression);

      return invocationAssignExpression;
    }

    private Expression GetOuterInvocationAssignExpression (Expression invocationVariableExpression, Expression invocationContextVariableExpression, Expression innerAspectAccessExpression, Expression innerInvocationVariableExpression, Type innerAspectType, MutableMethodInfo mutableMethod)
    {
      var invocationType = typeof (OuterInvocation);
      var invocationConstructor = invocationType.GetConstructors().Single();

      var innerAspectInterceptMethodInfo = GetAspectInterceptMethodInfo (innerAspectType, mutableMethod);
      var innerAspectInterceptMethodInfoExpression = Expression.Constant (innerAspectInterceptMethodInfo);

      var innerInvocationActionTypeExpression = Expression.Constant (typeof (Action<IInvocation>));
      var innerInvocationUnconvertedDelegateExpression = Expression.Call (
          null, _createDelegateMethodInfo, innerInvocationActionTypeExpression, innerAspectAccessExpression, innerAspectInterceptMethodInfoExpression);
      var innerInvocationDelegateExpression = Expression.Convert (innerInvocationUnconvertedDelegateExpression, typeof (Action<IInvocation>));

      var outerInvocationCreateExpression = Expression.New (
          invocationConstructor, invocationContextVariableExpression, innerInvocationDelegateExpression, innerInvocationVariableExpression);
      var outerInvocationAssignExpression = Expression.Assign (invocationVariableExpression, outerInvocationCreateExpression);

      return outerInvocationAssignExpression;
    }

    private Expression GetLastAspectCallExpression (Expression aspectsFieldExpression, IList<CompileTimeAspectBase> aspectsAsList, IList<ParameterExpression> invocationVariableExpressions, MutableMethodInfo mutableMethod)
    {
      var last = invocationVariableExpressions.Count - 1;

      var aspectAccessExpression = Expression.ArrayAccess (aspectsFieldExpression, Expression.Constant (last));
      var aspectType = aspectsAsList[last].AspectType;
      var aspectExpression = Expression.Convert (aspectAccessExpression, aspectType);

      var aspectInterceptMethodInfo = GetAspectInterceptMethodInfo (aspectType, mutableMethod);

      var lastAspectCallExpression = Expression.Call (aspectExpression, aspectInterceptMethodInfo, new[] { invocationVariableExpressions[last] });
      return lastAspectCallExpression;
    }

    private MethodInfo GetAspectInterceptMethodInfo (Type aspectType, MutableMethodInfo mutableMethod)
    {
      if (typeof (MethodInterceptionAspectAttribute).IsAssignableFrom (aspectType))
        return _onInterceptMethodInfo;
      else
      {
        if (mutableMethod.Name.StartsWith ("set"))
          return _onInterceptSetMethodInfo;
        else
          return _onInterceptGetMethodInfo;
      }
    }
  }
}