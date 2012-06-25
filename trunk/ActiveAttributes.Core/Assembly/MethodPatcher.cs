// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.FunctionalProgramming;

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

      mutableMethod.SetBody (ctx => GetPatchedBody (mutableMethod, ctx, aspectAttributes.Length));
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

    private Expression GetPatchedBody (MutableMethodInfo methodInfo, BodyContextBase context, int count)
    {
      var invocationContextType = GetInvocationContextType (methodInfo);
      var invocationContextVariableExpression = Expression.Variable (invocationContextType);
      var constructorInfo = invocationContextType.GetConstructors().First();
      var newExpression = Expression.New (
          constructorInfo,
          new Expression[]
          {
              Expression.Constant (methodInfo.UnderlyingSystemMethodInfo),
              Expression.Convert (context.This, methodInfo.DeclaringType.UnderlyingSystemType)
          }.Concat (context.Parameters.Cast<Expression>()));
      var invocationContextAssignmentExpression = Expression.Assign (
          invocationContextVariableExpression,
          newExpression);

      return Expression.Block (
          new[] { invocationContextVariableExpression },
          invocationContextAssignmentExpression,
          Expression.Constant (1));
      //System.Reflection.Emit.TypeBuilderInstantiation.
      //Type invocationType = null;
      //var invocationVariableExpressions = Enumerable.Repeat (Expression.Variable (invocationType), count);
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

    private Type GetInvocationContextType (MutableMethodInfo methodInfo)
    {
      var instanceType = new[] { methodInfo.UnderlyingSystemMethodInfo.DeclaringType };
      var parameterTypes = methodInfo.GetParameters().Select (x => x.ParameterType);

      if (methodInfo.ReturnType == typeof (void))
        return GetActionInvocationContextType (instanceType.Concat (parameterTypes).ToArray());
      else
        return GetFuncInvocationContextType (instanceType.Concat (parameterTypes).Concat (new[] { methodInfo.ReturnType }).ToArray ());
    }

    private Type GetFuncInvocationContextType (Type[] types)
    {
      var invocationContextType = GetFuncInvocationContextBaseType (types.Length);
      return invocationContextType.MakeGenericType (types);
    }

    private Type GetActionInvocationContextType (Type[] types)
    {
      var invocationContextType = GetActionInvocationContextBaseType (types.Length);
      return invocationContextType.MakeGenericType (types);
    }

    private Type GetActionInvocationContextBaseType (int typeCount)
    {
      switch (typeCount)
      {
        case 1: return typeof (ActionInvocationContext<>);
        case 2: return typeof (ActionInvocationContext<,>);
        default: throw new ArgumentException ("typeCount");
      }
    }

    private Type GetFuncInvocationContextBaseType (int typeCount)
    {
      switch (typeCount)
      {
        case 3: return typeof (FuncInvocationContext<,,>);
        default: throw new ArgumentException ("typeCount");
      }
    }

    private Expression GetReturnOrEmptyExpression (MutableMethodInfo methodInfo, Expression invocationContext)
    {
      if (methodInfo.ReturnType == typeof (void))
        return Expression.Empty();
      else
        return Expression.Convert (
            Expression.Property (invocationContext, "ReturnValue"),
            methodInfo.ReturnType);
    }
  }
}