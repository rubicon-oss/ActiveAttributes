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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.CompileTimeAspects;
using ActiveAttributes.Core.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
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
      mutableMethod.SetBody (ctx => GetPatchedBody (mutableMethod, ctx, fieldData, aspects));
    }

    private Expression GetPatchedBody (
        MutableMethodInfo mutableMethod, 
        BodyContextBase ctx, 
        FieldIntroducer.Data fieldData, 
        IEnumerable<CompileTimeAspectBase> aspects)
    {
      var aspectsAsList = aspects.ToList();

      var methodInfoFieldExpression = Expression.Field (ctx.This, fieldData.MethodInfoField);
      var delegateFieldExpression = Expression.Field (ctx.This, fieldData.DelegateField);
      var aspectsFieldExpression = Expression.Field (ctx.This, fieldData.InstanceAspectsField);

      // ActionInvocationContext<...> ctx = new ActionInvocationContext<...> (_methodInfo, this, arg1, arg2, ...);
      var invocationContextType = _typeProvider.GetInvocationContextType();
      var invocationContextVariableExpression = Expression.Variable (invocationContextType, "ctx");
      var invocationContextCreateExpression = GetInvocationContextNewExpression (invocationContextType, methodInfoFieldExpression, ctx.This, ctx.Parameters);
      var invocationContextAssignExpression = Expression.Assign (invocationContextVariableExpression, invocationContextCreateExpression);

      var invocationVariablesAndInitializations = GetInvocationVariablesAndAssignExpressions (
          aspectsAsList, mutableMethod, invocationContextVariableExpression, delegateFieldExpression, aspectsFieldExpression);
      var invocationVariableExpressions = invocationVariablesAndInitializations.Item1;
      var invocationInitExpressions = invocationVariablesAndInitializations.Item2;

      var outermostAspectExpression = Expression.ArrayAccess (aspectsFieldExpression, Expression.Constant (invocationVariableExpressions.Length - 1));
      var outermostAspectInterceptMethod = GetAspectInterceptMethod (aspectsAsList.Last().AspectType, mutableMethod);
      var outermostInvocationExpression = invocationVariableExpressions.Last();
      var aspectCallExpression = GetOutermostAspectCallExpression (outermostAspectExpression, outermostAspectInterceptMethod, outermostInvocationExpression);

      var returnValueExpression = Expression.Property (invocationContextVariableExpression, "ReturnValue");

      return Expression.Block (
          new[] { invocationContextVariableExpression }.Concat(invocationVariableExpressions),
          invocationContextAssignExpression,
          Expression.Block(invocationInitExpressions),
          aspectCallExpression,
          returnValueExpression);
    }

    private Tuple<ParameterExpression[], Expression[]> GetInvocationVariablesAndAssignExpressions (
        IList<CompileTimeAspectBase> aspects,
        MutableMethodInfo mutableMethod,
        Expression invocationContextExpression,
        Expression originalBodyDelegateExpression,
        Expression aspectArrayExpression)
    {
      // var invocation0 = new InnerInvocation (invocationContext, _originalBodyDelegate);
      // var invocation1 = new OuterInvocation (invocationContext, Delegate.CreateDelegate (typeof (Action<IInvocation>), _aspects[0], method), invocation0);
      // var invocation2 = new OuterInvocation (invocationContext, Delegate.CreateDelegate (typeof (Action<IInvocation>), _aspects[1], method), invocation1);

      var invocationVariableExpressions = new ParameterExpression[aspects.Count];
      var invocationAssignExpressions = new Expression[aspects.Count];

      for (var i = 0; i < aspects.Count; i++)
      {
        Expression invocationCreationExpression;
        if (i == 0)
        {
          invocationCreationExpression = GetInnermostInvocationCreationExpression (invocationContextExpression, originalBodyDelegateExpression);
        }
        else
        {
          var innerAspectType = aspects[i - 1].AspectType;
          var innerAspectExpression = Expression.ArrayAccess (aspectArrayExpression, Expression.Constant (i - 1));
          var innerAspectInterceptMethod = GetAspectInterceptMethod (innerAspectType, mutableMethod);
          var innerInvocationExpression = invocationVariableExpressions[i - 1];

          invocationCreationExpression = GetOuterInvocationCreationExpression (
              invocationContextExpression,
              innerAspectExpression,
              innerAspectInterceptMethod,
              innerInvocationExpression);
        }

        invocationVariableExpressions[i] = Expression.Variable (invocationCreationExpression.Type, "invocation" + (i + 1));
        invocationAssignExpressions[i] = Expression.Assign (invocationVariableExpressions[i], invocationCreationExpression);
      }
      
      return Tuple.Create (invocationVariableExpressions, invocationAssignExpressions);
    }

    private static NewExpression GetInvocationContextNewExpression (
        Type invocationContextType, Expression methodInfoExpression, Expression thisExpression, IEnumerable<ParameterExpression> parameterExpressions)
    {
      var invocationContextConstructor = invocationContextType.GetConstructors().Single();

      var invocationContextArgumentExpressions = new[] { methodInfoExpression, thisExpression }.Concat (parameterExpressions.Cast<Expression>());

      var invocationContextCreateExpression = Expression.New (invocationContextConstructor, invocationContextArgumentExpressions);
      return invocationContextCreateExpression;
    }
    
    private Expression GetInnermostInvocationCreationExpression (Expression invocationContextVariableExpression, Expression delegateFieldExpression)
    {
      var invocationType = _typeProvider.GetInvocationType();
      var invocationConstructor = invocationType.GetConstructors().Single();

      return Expression.New (invocationConstructor, invocationContextVariableExpression, delegateFieldExpression);
    }

    private Expression GetOuterInvocationCreationExpression (
        Expression invocationContextExpression,
        Expression innerAspectExpression,
        MethodInfo innerAspectInterceptMethod,
        Expression innerInvocationExpression)
    {
      // new OuterInvocation (invocationContext, innerInvocationDelegate, innerInvocation)
      var invocationConstructor = typeof (OuterInvocation).GetConstructors().Single();
      var innerInvocationDelegateExpression = GetInnerInvocationDelegateCreationExpression (innerAspectExpression, innerAspectInterceptMethod);
      var outerInvocationCreateExpression = Expression.New (
          invocationConstructor, invocationContextExpression, innerInvocationDelegateExpression, innerInvocationExpression);
      return outerInvocationCreateExpression;
    }

    private Expression GetInnerInvocationDelegateCreationExpression (Expression innerAspectExpression, MethodInfo innerAspectInterceptMethod)
    {
      // (Action<IInvocation>) Delegate.CreateDelegate (typeof (Action<IInvocation>), innerAspect, innerAspectInterceptMethod)
      var innerInvocationActionTypeExpression = Expression.Constant (typeof (Action<IInvocation>));
      var innerAspectInterceptMethodExpression = Expression.Constant (innerAspectInterceptMethod);
      var innerInvocationUnconvertedDelegateExpression = Expression.Call (
          null, _createDelegateMethodInfo, innerInvocationActionTypeExpression, innerAspectExpression, innerAspectInterceptMethodExpression);
      var innerInvocationDelegateExpression = Expression.Convert (innerInvocationUnconvertedDelegateExpression, typeof (Action<IInvocation>));
      return innerInvocationDelegateExpression;
    }

    private Expression GetOutermostAspectCallExpression (
        Expression outermostAspectExpression, MethodInfo aspectInterceptMethodInfo, Expression outermostInvocationExpression)
    {
      var convertedAspectExpression = Expression.Convert (outermostAspectExpression, aspectInterceptMethodInfo.DeclaringType);
      var lastAspectCallExpression = Expression.Call (convertedAspectExpression, aspectInterceptMethodInfo, new[] { outermostInvocationExpression });
      return lastAspectCallExpression;
    }

    private MethodInfo GetAspectInterceptMethod (Type aspectType, MutableMethodInfo mutableMethod)
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