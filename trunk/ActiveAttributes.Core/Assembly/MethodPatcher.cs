// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace ActiveAttributes.Core.Assembly
{
  public class MethodPatcher
  {
    private readonly MethodInfo _onInterceptMethodInfo;
    private readonly MethodInfo _onInterceptGetMethodInfo;
    private readonly MethodInfo _onInterceptSetMethodInfo;

    public MethodPatcher ()
    {
      _onInterceptMethodInfo = typeof (MethodInterceptionAspectAttribute).GetMethod ("OnIntercept");
      _onInterceptGetMethodInfo = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptGet");
      _onInterceptSetMethodInfo = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptSet");
    }

    public void PatchMethod (MutableType mutableType, MutableMethodInfo mutableMethod, FieldInfo allMethodAspectsArrayField,
      AspectAttribute[] aspectAttributes)
    {
      var copiedMethod = GetCopiedMethod (mutableType, mutableMethod);

      var onInterceptMethodInfo = typeof (MethodInterceptionAspectAttribute).GetMethod ("OnIntercept");

      mutableMethod.SetBody
          (
            ctx =>
            {
              var indexExpression = Expression.ArrayAccess(
                Expression.Field(ctx.This, allMethodAspectsArrayField),
                Expression.Constant(1));
              return Expression.Call(
                         Expression.Convert(indexExpression, typeof(MethodInterceptionAspectAttribute)),
                         onInterceptMethodInfo,
                         Expression.Constant(null, typeof(IInvocation))

                         );
            });
    }

    private MutableMethodInfo GetCopiedMethod (MutableType mutableType, MutableMethodInfo mutableMethod)
    {
      return mutableType.AddMethod (
          "_m_" + mutableMethod.Name,
          MethodAttributes.Private,
          mutableMethod.ReturnType,
          ParameterDeclaration.CreateForEquivalentSignature (mutableMethod),
          ctx => ctx.GetCopiedMethodBody (mutableMethod, ctx.Parameters.Cast<Expression>()));
    }

    private Expression GetPatchedBody (BodyContextBase context, int count)
    {
      Type invocationType = null;
      var invocationVariableExpressions = Enumerable.Repeat (Expression.Variable (invocationType), count);
      return null;
    }

    private MethodInfo GetAspectCallMethodInfo (AspectAttribute aspectAttribute, MutableMethodInfo methodInfo)
    {
      if (aspectAttribute is MethodInterceptionAspectAttribute)
        return _onInterceptMethodInfo;
      else
      {
        if (methodInfo.Name.StartsWith ("set"))
          return _onInterceptSetMethodInfo;
        else
          return _onInterceptGetMethodInfo;
      }
    }

  }
}