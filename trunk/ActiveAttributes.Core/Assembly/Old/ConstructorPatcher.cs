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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Extensions;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly.Old
{
  public class ConstructorPatcher : IConstructorPatcher
  {
    public void AddReflectionAndDelegateInitialization (
        MutableMethodInfo mutableMethod,
        FieldInfoContainer fieldInfoContainer,
        MutableMethodInfo copiedMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      ArgumentUtility.CheckNotNull ("copiedMethod", copiedMethod);

      Func<BodyContextBase, Expression> mutation =
          ctx => Expression.Block (
              GetPropertyInfoAssignExpression (fieldInfoContainer.PropertyInfoField, ctx, mutableMethod),
              GetEventInfoAssignExpression (fieldInfoContainer.EventInfoField, ctx, mutableMethod),
              GetMethodInfoAssignExpression (fieldInfoContainer.MethodInfoField, ctx, mutableMethod),
              GetDelegateAssignExpression (fieldInfoContainer.DelegateField, ctx, mutableMethod, copiedMethod));

      var mutableType = (MutableType) mutableMethod.DeclaringType;
      AddMutation (mutableType, mutation);
    }

    private Expression GetDelegateAssignExpression (FieldInfo delegateFieldInfo, BodyContextBase ctx, MutableMethodInfo mutableMethod, MutableMethodInfo copiedMethodInfo)
    {
      var delegateField = Expression.Field (ctx.This, delegateFieldInfo);

      var delegateType = Expression.Constant (mutableMethod.GetDelegateType ());
      var convertedDelegate = Expression.NewDelegate (mutableMethod.GetDelegateType (), ctx.This, copiedMethodInfo);
      //var createDelegateMethodInfo = typeof (Delegate).GetMethod ("CreateDelegate", new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
      //var copiedMethod = Expression.Constant (copiedMethodInfo.UnderlyingSystemMethodInfo, typeof (MethodInfo));
      //var createDelegate = Expression.Call (null, createDelegateMethodInfo, delegateType, ctx.This, copiedMethod);
      //var convertedDelegate = Expression.Convert (createDelegate, (Type) delegateType.Value);

      var delegateAssignExpression = Expression.Assign (delegateField, convertedDelegate);
      return delegateAssignExpression;
    }

    private Expression GetMethodInfoAssignExpression (FieldInfo methodInfoFieldInfo, BodyContextBase ctx, MutableMethodInfo methodInfo)
    {
      var methodInfoField = Expression.Field (ctx.This, methodInfoFieldInfo);

      var methodInfoConstantExpression = Expression.Constant (methodInfo.UnderlyingSystemMethodInfo, typeof (MethodInfo));
      var methodInfoAssignExpression = Expression.Assign (methodInfoField, methodInfoConstantExpression);

      return methodInfoAssignExpression;
    }

    private Expression GetPropertyInfoAssignExpression (FieldInfo propertyInfoFieldInfo, BodyContextBase ctx, MutableMethodInfo methodInfo)
    {
      var propertyInfo = methodInfo.UnderlyingSystemMethodInfo.GetRelatedPropertyInfo ();
      if (propertyInfo == null)
        return Expression.Empty();

      var propertyInfoField = Expression.Field (ctx.This, propertyInfoFieldInfo);
      var propertyInfoGetExpression =
          Expression.Call (
              Expression.Constant (methodInfo.UnderlyingSystemMethodInfo.DeclaringType, typeof (Type)),
              typeof (Type).GetMethod ("GetProperty", new[] { typeof (string), typeof (BindingFlags) }),
              Expression.Constant (propertyInfo.Name),
              Expression.Constant (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
      var propertyInfoAssignExpression = Expression.Assign (propertyInfoField, propertyInfoGetExpression);

      return propertyInfoAssignExpression;
    }

    private Expression GetEventInfoAssignExpression (FieldInfo eventInfoFieldInfo, BodyContextBase ctx, MutableMethodInfo methodInfo)
    {
      var eventInfo = methodInfo.UnderlyingSystemMethodInfo.GetRelatedEventInfo();
      if (eventInfo == null)
        return Expression.Empty();

      var eventInfoField = Expression.Field (ctx.This, eventInfoFieldInfo);
      var eventInfoGetExpression =
          Expression.Call (
              Expression.Constant (methodInfo.UnderlyingSystemMethodInfo.DeclaringType, typeof (Type)),
              typeof (Type).GetMethod ("GetEvent", new[] { typeof (string), typeof (BindingFlags) }),
              Expression.Constant (eventInfo.Name),
              Expression.Constant (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
      var eventInfoAssignExpression = Expression.Assign (eventInfoField, eventInfoGetExpression);

      return eventInfoAssignExpression;
    }

    public void AddAspectInitialization (
        MutableType mutableType,
        IFieldWrapper staticAccessor,
        IFieldWrapper instanceAccessor,
        IEnumerable<IExpressionGenerator> aspects)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      ArgumentUtility.CheckNotNull ("staticAccessor", staticAccessor);
      ArgumentUtility.CheckNotNull ("instanceAccessor", instanceAccessor);
      ArgumentUtility.CheckNotNull ("aspects", aspects);

      var aspectsAsCollection = aspects.ConvertToCollection();
      Func<BodyContextBase, Expression> mutation =
          ctx => Expression.Block (
              GeAspectsArrayAssignExpression (instanceAccessor, ctx, aspectsAsCollection.Where (x => x.AspectDescriptor.Scope == Scope.Instance)),
              GeAspectsArrayAssignExpression (staticAccessor, ctx, aspectsAsCollection.Where (x => x.AspectDescriptor.Scope == Scope.Static)));

      AddMutation (mutableType, mutation);
    }

    private Expression GeAspectsArrayAssignExpression (IFieldWrapper accessor, BodyContextBase ctx, IEnumerable<IExpressionGenerator> aspects)
    {
      var aspectsField = accessor.GetAccessExpression (ctx.This);
      var newAspectsArrayExpression = Expression.NewArrayInit (typeof (AspectAttribute), aspects.Select (x => x.GetInitExpression ()));
      Expression instanceAspectsAssign = Expression.Assign (aspectsField, newAspectsArrayExpression);

      // WAIT RM-5119 static constructor
      if (accessor.IsStatic)
      {
        instanceAspectsAssign = Expression.IfThen (
            Expression.Equal (aspectsField, Expression.Constant (null)),
            instanceAspectsAssign);
      }

      return instanceAspectsAssign;
    }

    private void AddMutation (MutableType mutableType, Func<BodyContextBase, Expression> expression)
    {
      foreach (var constructor in mutableType.AllMutableConstructors)
        constructor.SetBody (ctx => Expression.Block (ctx.PreviousBody, expression (ctx)));
    }
  }
}