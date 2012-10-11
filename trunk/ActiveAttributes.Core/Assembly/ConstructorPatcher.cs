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
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Extensions;

using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly
{
  public class ConstructorPatcher : IConstructorPatcher
  {
    public void AddReflectionAndDelegateInitialization (
        MutableMethodInfo mutableMethod,
        FieldInfo propertyInfoFieldInfo,
        FieldInfo eventInfoFieldInfo,
        FieldInfo methodInfoFieldInfo,
        FieldInfo delegateFieldInfo,
        MutableMethodInfo copiedMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      ArgumentUtility.CheckNotNull ("propertyInfoFieldInfo", propertyInfoFieldInfo);
      ArgumentUtility.CheckNotNull ("eventInfoFieldInfo", eventInfoFieldInfo);
      ArgumentUtility.CheckNotNull ("methodInfoFieldInfo", methodInfoFieldInfo);
      ArgumentUtility.CheckNotNull ("delegateFieldInfo", delegateFieldInfo);
      ArgumentUtility.CheckNotNull ("copiedMethod", copiedMethod);

      Func<BodyContextBase, Expression> mutation =
          ctx => Expression.Block (
              GetPropertyInfoAssignExpression (propertyInfoFieldInfo, ctx, mutableMethod),
              GetEventInfoAssignExpression (eventInfoFieldInfo, ctx, mutableMethod),
              GetMethodInfoAssignExpression (methodInfoFieldInfo, ctx, mutableMethod),
              GetDelegateAssignExpression (delegateFieldInfo, ctx, copiedMethod));

      var mutableType = (MutableType) mutableMethod.DeclaringType;
      AddMutation (mutableType, mutation);
    }

    private Expression GetDelegateAssignExpression (FieldInfo delegateFieldInfo, BodyContextBase ctx, MutableMethodInfo copiedMethodInfo)
    {
      var delegateField = Expression.Field (ctx.This, delegateFieldInfo);

      var createDelegateMethodInfo = typeof (Delegate).GetMethod ("CreateDelegate", new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
      var delegateType = Expression.Constant (copiedMethodInfo.GetDelegateType());
      var copiedMethod = Expression.Constant (copiedMethodInfo.UnderlyingSystemMethodInfo, typeof(MethodInfo));
      var createDelegate = Expression.Call (null, createDelegateMethodInfo, delegateType, ctx.This, copiedMethod);
      var convertedDelegate = Expression.Convert (createDelegate, (Type) delegateType.Value);

      var delegateAssignExpression = Expression.Assign (delegateField, convertedDelegate);
      return delegateAssignExpression;
    }

    private Expression GetMethodInfoAssignExpression (FieldInfo methodInfoFieldInfo, BodyContextBase ctx, MutableMethodInfo methodInfo)
    {
      var methodInfoField = Expression.Field (ctx.This, methodInfoFieldInfo);

      var methodInfoConstantExpression = Expression.Constant (methodInfo.UnderlyingSystemMethodInfo, typeof (MethodInfo)); // TODO HOW TO TEST????
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
        IArrayAccessor staticAccessor,
        IArrayAccessor instanceAccessor,
        IEnumerable<IAspectGenerator> aspects)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      ArgumentUtility.CheckNotNull ("staticAccessor", staticAccessor);
      ArgumentUtility.CheckNotNull ("instanceAccessor", instanceAccessor);
      ArgumentUtility.CheckNotNull ("aspects", aspects);

      var aspectsAsCollection = aspects.ConvertToCollection();
      Func<BodyContextBase, Expression> mutation =
          ctx => Expression.Block (
              GeAspectsArrayAssignExpression (instanceAccessor, ctx, aspectsAsCollection.Where (x => x.Descriptor.Scope == AspectScope.Instance)),
              GeAspectsArrayAssignExpression (staticAccessor, ctx, aspectsAsCollection.Where (x => x.Descriptor.Scope == AspectScope.Static)));

      AddMutation (mutableType, mutation);
    }

    private Expression GeAspectsArrayAssignExpression (IArrayAccessor accessor, BodyContextBase ctx, IEnumerable<IAspectGenerator> aspects)
    {
      var instanceAspectsField = accessor.GetAccessExpression (accessor.IsStatic ? null : ctx.This);
      var instanceAspectsArray = Expression.NewArrayInit (typeof (AspectAttribute), aspects.Select (x => x.GetInitExpression ()));
      Expression instanceAspectsAssign = Expression.Assign (instanceAspectsField, instanceAspectsArray);

      if (accessor.IsStatic)
      {
        instanceAspectsAssign = Expression.IfThen (
            Expression.Equal (instanceAspectsField, Expression.Constant (null)),
            instanceAspectsAssign);
      }

      return instanceAspectsAssign;
    }

    private void AddMutation (MutableType mutableType, Func<BodyContextBase, Expression> expression)
    {
      foreach (var constructor in mutableType.AllMutableConstructors)
        constructor.SetBody (ctx => Expression.Block (ctx.PreviousBody, expression (ctx)));
    }
































    //public void Patch (MutableMethodInfo mutableMethod, IEnumerable<IAspectDescriptor> aspects, FieldIntroducer.Data fieldData, MutableMethodInfo copiedMethod)
    //{
    //  var mutableType = ((MutableType) mutableMethod.DeclaringType);

    //  //var initMethod = mutableType.AddMethod ("_InitializeAspects", MethodAttributes.Private, typeof (void), new ParameterDeclaration[0],
    //  //                                        ctx => Expression.Block (
    //  //                                            GetMethodInfoAssignExpression (fieldData.MethodInfoField, mutableMethod, ctx),
    //  //                                            GetDelegateAssignExpression (fieldData.DelegateField, mutableMethod, ctx, copiedMethod),
    //  //                                            GetAspectsInitExpression (fieldData.StaticAspectsField, fieldData.InstanceAspectsField,
    //  //                                                                      aspects, ctx)));

    //  foreach (var mutableConstructor in mutableType.AllMutableConstructors)
    //  {
    //    mutableConstructor.SetBody (
    //        ctx =>
    //        Expression.Block (
    //            ctx.PreviousBody,
    //            GetMethodInfoAssignExpression (fieldData.MethodInfoField, mutableMethod, ctx),
    //            GetDelegateAssignExpression (fieldData.DelegateField, mutableMethod, ctx, copiedMethod),
    //            GetAspectsInitExpression (
    //                fieldData.StaticAspectsField,
    //                fieldData.InstanceAspectsField,
    //                aspects,
    //                ctx)));
    //  }
    //}

    //private Expression GetAspectsInitExpression (FieldInfo staticAspectsField,
    //    FieldInfo instanceAspectsField, IEnumerable<IAspectDescriptor> compileTimeAspects, BodyContextBase ctx)
    //{
    //  var compileTimeAspectsAsCollection = compileTimeAspects.ConvertToCollection();

    //  var staticAspectsFieldExpression = Expression.Field (null, staticAspectsField);
    //  var staticAspectsInitExpression = Expression.NewArrayInit (
    //      typeof (AspectAttribute),
    //      compileTimeAspectsAsCollection.Select (GetAspectInitExpression));
    //  var staticAspectsAssignExpression = Expression.Assign (staticAspectsFieldExpression, staticAspectsInitExpression);
    //  var staticAspectsIfNullExpression = Expression.Equal (staticAspectsFieldExpression, Expression.Constant (null));
    //  var staticAspectsAssignIfNullExpression = Expression.IfThen (staticAspectsIfNullExpression, staticAspectsAssignExpression);

    //  var instanceAspectsFieldExpression = Expression.Field (ctx.This, instanceAspectsField);
    //  var instanceAspectToStaticAspectIndex = 0;
    //  var instanceAspectsElementInitExpressions = compileTimeAspectsAsCollection
    //      .Select (
    //          x =>
    //          x.Scope == AspectScope.Static
    //              ? GetStaticAspectsArrayAccess(staticAspectsFieldExpression, ref instanceAspectToStaticAspectIndex)
    //              : GetAspectInitExpression (x));
    //  var instanceAspectsInitExpression = Expression.NewArrayInit (
    //      typeof (AspectAttribute),
    //      instanceAspectsElementInitExpressions);
    //  var instanceAspectsAssignExpression = Expression.Assign (instanceAspectsFieldExpression, instanceAspectsInitExpression);

    //  return Expression.Block (staticAspectsAssignIfNullExpression, instanceAspectsAssignExpression);
    //}

    //private Expression GetAspectInitExpression (IAspectDescriptor customDataAspectDescriptor)
    //{
    //  var ctorArgumentExpressions = customDataAspectDescriptor.ConstructorArguments.Select (GetConstantExpressionForTypedArgument);
    //  var newExpression = Expression.New (customDataAspectDescriptor.ConstructorInfo, ctorArgumentExpressions);
    //  var memberBindingExpressions = customDataAspectDescriptor.NamedArguments.Select (GetMemberBindingExpression);
    //  var initExpression = Expression.MemberInit (newExpression, memberBindingExpressions.Cast<MemberBinding>());
    //  return initExpression;
    //}

    //private MemberAssignment GetMemberBindingExpression (CustomAttributeNamedArgument namedArgument)
    //{
    //  var argumentType = namedArgument.TypedValue.ArgumentType;
    //  var constantExpression = GetConstantExpressionForTypedArgument (namedArgument.TypedValue);
    //  var bindingExpression = Expression.Bind (namedArgument.MemberInfo, Expression.Convert (constantExpression, argumentType));
    //  return bindingExpression;
    //}

    //private Expression GetConstantExpressionForTypedArgument (CustomAttributeTypedArgument typedArgument)
    //{
    //  if (!typedArgument.ArgumentType.IsArray)
    //    return Expression.Convert (Expression.Constant (typedArgument.Value), typedArgument.ArgumentType);
    //  else
    //  {
    //    var elementCollection = (ReadOnlyCollection<CustomAttributeTypedArgument>) typedArgument.Value;
    //    var elementInitExpressions = elementCollection.Select (GetConstantExpressionForTypedArgument);
    //    return Expression.NewArrayInit (typedArgument.ArgumentType.GetElementType (), elementInitExpressions);
    //  }
    //}

    //private Expression GetStaticAspectsArrayAccess (MemberExpression staticAspectsFieldExpression, ref int instanceAspectToStaticAspectIndex)
    //{
    //  return Expression.ArrayAccess (staticAspectsFieldExpression, Expression.Constant (instanceAspectToStaticAspectIndex++));
    //}


    //private Expression GetMethodInfoAssignExpression (FieldInfo methodInfoField, MutableMethodInfo mutableMethod, BodyContextBase ctx)
    //{
    //  var methodInfoFieldExpression = Expression.Field (ctx.This, methodInfoField);
    //  var methodInfoConstantExpression = Expression.Constant (mutableMethod.UnderlyingSystemMethodInfo, typeof (MethodInfo)); // TODO HOW TO TEST????
    //  var methodInfoAssignExpression = Expression.Assign (methodInfoFieldExpression, methodInfoConstantExpression);

    //  return methodInfoAssignExpression;
    //}

    //private Expression GetDelegateAssignExpression (FieldInfo delegateField, MutableMethodInfo mutableMethod, BodyContextBase ctx, MutableMethodInfo copiedMethod)
    //{
    //  var delegateFieldExpression = Expression.Field (ctx.This, delegateField);
    //  var delegateType = mutableMethod.GetDelegateType ();
    //  var createDelegateMethodInfo = typeof (Delegate).GetMethod (
    //      "CreateDelegate",
    //      new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
    //  var delegateCreateExpression = Expression.Call (
    //      null,
    //      createDelegateMethodInfo,
    //      Expression.Constant (delegateType),
    //      ctx.This,
    //      Expression.Constant (copiedMethod, typeof (MethodInfo)));
    //  var delegateConvertExpression = Expression.Convert (delegateCreateExpression, delegateField.FieldType);
    //  var delegateAssignExpression = Expression.Assign (delegateFieldExpression, delegateConvertExpression);

    //  return delegateAssignExpression;
    //}

  }
}