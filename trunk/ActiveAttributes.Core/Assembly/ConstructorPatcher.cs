// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace ActiveAttributes.Core.Assembly
{
  public class ConstructorPatcher
  {
    public void Patch (FieldIntroducer.Data fieldData, IEnumerable<CompileTimeAspect> aspects, MutableMethodInfo mutableMethod)
    {
      var mutableType = ((MutableType) mutableMethod.DeclaringType);
      var mutableConstructor = mutableType.AllMutableConstructors.Single();


      mutableConstructor.SetBody (
          ctx =>
          Expression.Block (
              ctx.PreviousBody,
              GetMethodInfoAssignExpression (fieldData, mutableMethod, ctx)));
    }

    private Expression GetMethodInfoAssignExpression (FieldIntroducer.Data fieldData, MutableMethodInfo mutableMethod, BodyContextBase ctx)
    {
      var methodInfoFieldExpression = Expression.Field (ctx.This, fieldData.MethodInfoField);
      var methodInfoConstantExpression = Expression.Constant (mutableMethod, typeof (MethodInfo));
      var methodInfoAssignExpression = Expression.Assign (methodInfoFieldExpression, methodInfoConstantExpression);

      return methodInfoAssignExpression;
    }
  }
}