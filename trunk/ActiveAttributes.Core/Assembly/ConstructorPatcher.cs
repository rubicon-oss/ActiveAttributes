// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;

using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace ActiveAttributes.Core.Assembly
{
  public class ConstructorPatcher
  {
    public void Patch (FieldIntroducer.Data fieldData, IEnumerable<CompileTimeAspect> compileTimeAspects, MutableMethodInfo mutableMethod,
      MutableMethodInfo copiedMethod)
    {
      var mutableType = ((MutableType) mutableMethod.DeclaringType);

      foreach (var mutableConstructor in mutableType.AllMutableConstructors)
      {
        mutableConstructor.SetBody (
            ctx =>
            Expression.Block (
                ctx.PreviousBody,
                GetMethodInfoAssignExpression (fieldData.MethodInfoField, mutableMethod, ctx),
                GetDelegateAssignExpression (fieldData.DelegateField, mutableMethod, ctx, copiedMethod),
                GetAspectsInitExpression (fieldData.StaticAspectsField, fieldData.InstanceAspectsField, compileTimeAspects, ctx)));
      }
    }

    private Expression GetAspectsInitExpression (FieldInfo staticAspectsField,
        FieldInfo instanceAspectsField, IEnumerable<CompileTimeAspect> compileTimeAspects, ConstructorBodyModificationContext ctx)
    {
      var compileTimeAspectsAsCollection = compileTimeAspects.ConvertToCollection();

      var staticAspectsFieldExpression = Expression.Field (null, staticAspectsField);
      var staticAspectsInitExpression = Expression.NewArrayInit (
          typeof (AspectAttribute),
          compileTimeAspectsAsCollection.Select (GetAspectInitExpression));
      var staticAspectsAssignExpression = Expression.Assign (staticAspectsFieldExpression, staticAspectsInitExpression);
      var staticAspectsIfNullExpression = Expression.Equal (staticAspectsFieldExpression, Expression.Constant (null));
      var staticAspectsAssignIfNullExpression = Expression.IfThen (staticAspectsIfNullExpression, staticAspectsAssignExpression);

      var instanceAspectsFieldExpression = Expression.Field (ctx.This, instanceAspectsField);
      var instanceAspectToStaticAspectIndex = 0;
      var instanceAspectsElementInitExpressions = compileTimeAspectsAsCollection
          .Select (
              x =>
              x.Scope == AspectScope.Static
                  ? GetStaticAspectsArrayAccess(staticAspectsFieldExpression, ref instanceAspectToStaticAspectIndex)
                  : GetAspectInitExpression (x));
      var instanceAspectsInitExpression = Expression.NewArrayInit (
          typeof (AspectAttribute),
          instanceAspectsElementInitExpressions);
      var instanceAspectsAssignExpression = Expression.Assign (instanceAspectsFieldExpression, instanceAspectsInitExpression);

      return Expression.Block (staticAspectsAssignIfNullExpression, instanceAspectsAssignExpression);
    }

    private Expression GetAspectInitExpression (CompileTimeAspect compileTimeAspect)
    {
      var ctorArgumentExpressions = compileTimeAspect.ConstructorArguments.Select (GetConstantExpressionForTypedArgument);
      var newExpression = Expression.New (compileTimeAspect.ConstructorInfo, ctorArgumentExpressions);
      var memberBindingExpressions = compileTimeAspect.NamedArguments.Select (GetMemberBindingExpression);
      var initExpression = Expression.MemberInit (newExpression, memberBindingExpressions.Cast<MemberBinding>());
      return initExpression;
    }

    private MemberAssignment GetMemberBindingExpression (CustomAttributeNamedArgument namedArgument)
    {
      var argumentType = namedArgument.TypedValue.ArgumentType;
      var constantExpression = GetConstantExpressionForTypedArgument (namedArgument.TypedValue);
      var bindingExpression = Expression.Bind (namedArgument.MemberInfo, Expression.Convert (constantExpression, argumentType));
      return bindingExpression;
    }

    private Expression GetConstantExpressionForTypedArgument (CustomAttributeTypedArgument typedArgument)
    {
      if (!typedArgument.ArgumentType.IsArray)
        return Expression.Convert (Expression.Constant (typedArgument.Value), typedArgument.ArgumentType);
      else
      {
        var elementCollection = (ReadOnlyCollection<CustomAttributeTypedArgument>) typedArgument.Value;
        var elementInitExpressions = elementCollection.Select (GetConstantExpressionForTypedArgument);
        return Expression.NewArrayInit (typedArgument.ArgumentType.GetElementType (), elementInitExpressions);
      }
    }

    private Expression GetStaticAspectsArrayAccess (MemberExpression staticAspectsFieldExpression, ref int instanceAspectToStaticAspectIndex)
    {
      return Expression.ArrayAccess (staticAspectsFieldExpression, Expression.Constant (instanceAspectToStaticAspectIndex++));
    }

    private Expression GetDelegateAssignExpression (
        FieldInfo delegateField, MutableMethodInfo mutableMethod, ConstructorBodyModificationContext ctx, MutableMethodInfo copiedMethod)
    {
      var delegateFieldExpression = Expression.Field (ctx.This, delegateField);
      var delegateType = mutableMethod.GetDelegateType();
      var createDelegateMethodInfo = typeof (Delegate).GetMethod (
          "CreateDelegate",
          new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
      var delegateCreateExpression = Expression.Call (
          null,
          createDelegateMethodInfo,
          Expression.Constant (delegateType),
          ctx.This,
          Expression.Constant (copiedMethod, typeof (MethodInfo)));
      var delegateConvertExpression = Expression.Convert (delegateCreateExpression, delegateField.FieldType);
      var delegateAssignExpression = Expression.Assign (delegateFieldExpression, delegateConvertExpression);

      return delegateAssignExpression;
    }

    private Expression GetMethodInfoAssignExpression (FieldInfo methodInfoField, MutableMethodInfo mutableMethod, BodyContextBase ctx)
    {
      var methodInfoFieldExpression = Expression.Field (ctx.This, methodInfoField);
      var methodInfoConstantExpression = Expression.Constant (mutableMethod, typeof (MethodInfo));
      var methodInfoAssignExpression = Expression.Assign (methodInfoFieldExpression, methodInfoConstantExpression);

      return methodInfoAssignExpression;
    }
  }
}