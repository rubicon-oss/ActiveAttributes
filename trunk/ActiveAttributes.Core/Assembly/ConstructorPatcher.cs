// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Extensions;

using Microsoft.Scripting.Ast;
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
      var mutableConstructor = mutableType.AllMutableConstructors.Single();



      mutableConstructor.SetBody (
          ctx =>
          Expression.Block (
              ctx.PreviousBody,
              GetMethodInfoAssignExpression (fieldData.MethodInfoField, mutableMethod, ctx),
              GetDelegateAssignExpression (fieldData.DelegateField, mutableMethod, ctx, copiedMethod),
              GetInstanceAspectAssignExpression (fieldData.InstanceAspectsField, compileTimeAspects, ctx)));
    }

    private Expression GetInstanceAspectAssignExpression (FieldInfo instanceAspectsField, IEnumerable<CompileTimeAspect> compileTimeAspects, ConstructorBodyModificationContext ctx)
    {
      var instanceAspectsFieldExpression = Expression.Field (ctx.This, instanceAspectsField);
      var instanceAspectsInitExpression = Expression.NewArrayInit (typeof (AspectAttribute),
        GetInstanceAspectsInitExpressions (compileTimeAspects));
      var instanceAspectsAssignExpression = Expression.Assign (instanceAspectsFieldExpression, instanceAspectsInitExpression);
      return instanceAspectsAssignExpression;
    }

    private IEnumerable<Expression> GetInstanceAspectsInitExpressions (IEnumerable<CompileTimeAspect> compileTimeAspects)
    {
      return compileTimeAspects.Select (x => Expression.New (x.ConstructorInfo)).Cast<Expression>();
    }

    private Expression GetDelegateAssignExpression (FieldInfo delegateField, MutableMethodInfo mutableMethod, ConstructorBodyModificationContext ctx, MutableMethodInfo copiedMethod)
    {
      var delegateFieldExpression = Expression.Field (ctx.This, delegateField);
      var delegateType = mutableMethod.GetDelegateType ();
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