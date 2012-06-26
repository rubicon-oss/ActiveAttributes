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
    private readonly MethodInfo _onInterceptGetMethodInfo;
    private readonly MethodInfo _onInterceptMethodInfo;
    private readonly MethodInfo _onInterceptSetMethodInfo;

    public MethodPatcher ()
    {
      _onInterceptMethodInfo = typeof (MethodInterceptionAspectAttribute).GetMethod ("OnIntercept");
      _onInterceptGetMethodInfo = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptGet");
      _onInterceptSetMethodInfo = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptSet");
    }

    public void PatchMethod (
        MutableType mutableType,
        MutableMethodInfo mutableMethod,
        FieldInfo allMethodAspectsArrayField,
        AspectAttribute[] aspectAttributes)
    {
      var copiedMethod = GetCopiedMethod (mutableType, mutableMethod);

      mutableMethod.SetBody (ctx => GetPatchedBody (mutableMethod, ctx, copiedMethod, allMethodAspectsArrayField, aspectAttributes));
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

    private Expression GetPatchedBody (
        MutableMethodInfo methodInfo,
        BodyContextBase bodyContext,
        MutableMethodInfo copiedMethod,
        FieldInfo allMethodAspectsArrayField,
        AspectAttribute[] aspectAttributes)
    {
      var typeProvider = new TypeProvider (methodInfo);

      // var context = new InvocationContext(methodInfo, this, args)
      var invocationContextType = typeProvider.GetInvocationContextType();
      var invocationContextVariableExpression = Expression.Variable (invocationContextType);
      var invocationContextNewExpression = Expression.New (
          invocationContextType.GetConstructors().Single(),
          new Expression[]
          {
              Expression.Constant (methodInfo.UnderlyingSystemMethodInfo),
              Expression.Convert (bodyContext.This, methodInfo.DeclaringType.UnderlyingSystemType)
          }.Concat (bodyContext.Parameters.Cast<Expression>()));
      var invocationContextAssignmentExpression = Expression.Assign (invocationContextVariableExpression, invocationContextNewExpression);

      // invocations[0] = new FuncInvocation<,,>(context, methodDelegate)
      // invocations[1] = new OuterInvocation(context, aspects[0], invocation[0])
      var invocationsVariableExpression = Expression.Variable (typeof (IInvocation[]));
      var invocationsAssignExpression = Expression.Assign (
          invocationsVariableExpression, Expression.NewArrayBounds (typeof (IInvocation), Expression.Constant (aspectAttributes.Length)));
      var invocationsInitExpression = GetInvocationsInitExpressions (
          methodInfo,
          copiedMethod,
          bodyContext,
          invocationsVariableExpression,
          aspectAttributes,
          typeProvider,
          invocationContextVariableExpression,
          allMethodAspectsArrayField);

      // aspects[length - 1].OnIntercept(invocations[length - 1])
      var aspectsArrayFieldExpression = Expression.Field (bodyContext.This, allMethodAspectsArrayField);
      var lastIndexExpression = Expression.Constant (aspectAttributes.Length - 1);
      var lastAspectExpression = Expression.Convert (
          Expression.ArrayAccess (aspectsArrayFieldExpression, lastIndexExpression), typeof (MethodInterceptionAspectAttribute));
      var lastInvocationExpression = Expression.ArrayAccess (invocationsVariableExpression, lastIndexExpression);

      var interceptMethodInfo = GetAspectInterceptMethodInfo (aspectAttributes[0], methodInfo);
      var interceptCallExpression = Expression.Call (lastAspectExpression, interceptMethodInfo, new[] { lastInvocationExpression });

      return Expression.Block (
          new[] { invocationContextVariableExpression, invocationsVariableExpression },
          invocationContextAssignmentExpression,
          GetReturnValueDefaultOrEmptyExpression (methodInfo, invocationContextVariableExpression),
          invocationsAssignExpression,
          invocationsInitExpression,
          interceptCallExpression,
          GetReturnOrEmptyExpression (methodInfo, invocationContextVariableExpression));
    }


    private Expression GetInvocationsInitExpressions (
        MutableMethodInfo methodInfo,
        MutableMethodInfo copiedMethod,
        BodyContextBase bodyContext,
        ParameterExpression invocationsVariableExpression,
        AspectAttribute[] aspectAttributes,
        TypeProvider typeProvider,
        ParameterExpression invocationContextVariableExpression,
        FieldInfo allMethodAspectsArrayField)
    {
      var expressionList = new List<Expression>();

      for (var i = 0; i < aspectAttributes.Length; i++)
      {
        // If this is the first iteration, we need to create a typed invocation (i.e., ActionInvocation<>, FuncInvocation<>)
        if (i == 0)
        {
          // var delegate = delegate(copiedMethod)
          var methodDelegateType = GetMethodDelegateType (methodInfo);
          var methodDelegateVariableExpression = Expression.Variable (methodDelegateType);
          var createDelegateMethodInfo = typeof (Delegate).GetMethod (
              "CreateDelegate",
              new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
          var methodDelegateCreateExpression = Expression.Call (
              null,
              createDelegateMethodInfo,
              Expression.Constant (methodDelegateType),
              bodyContext.This,
              Expression.Constant (copiedMethod, typeof (MethodInfo)));
          var methodDelegateAssignmentExpression = Expression.Assign (
              methodDelegateVariableExpression,
              Expression.Convert (methodDelegateCreateExpression, methodDelegateType));

          // invocations[0] = new XInvocation<> (context, delegate)
          var invocationType = typeProvider.GetInvocationType();
          var invocationActionType = typeProvider.GetInvocationActionType();
          var invocationCreateExpression = Expression.New (
              invocationType.GetConstructors().Single(),
              invocationContextVariableExpression,
              Expression.Convert (methodDelegateVariableExpression, invocationActionType));
          var invocationAssignExpression = Expression.Assign (
              Expression.ArrayAccess (invocationsVariableExpression, Expression.Constant (i)), invocationCreateExpression);

          var expression = Expression.Block (
              new[] { methodDelegateVariableExpression },
              methodDelegateAssignmentExpression,
              invocationAssignExpression);

          expressionList.Add (expression);
        }
            // Otherwise we create an invocation pointing to the last aspects invocation
        else
        {
          var lastItem = Expression.Constant (i - 1);

          // get last aspects intercept method (i.e., OnIntercept, OnInterceptSet, ...)
          // var delegate = delegate(aspects[i - 1])
          var interceptMethodInfo = GetAspectInterceptMethodInfo (aspectAttributes[i], methodInfo);
          var interceptDelegateType = GetMethodDelegateType (interceptMethodInfo); // TODO: change to Action<IInvocation> ?
          var interceptDelegateVariableExpression = Expression.Variable (interceptDelegateType);
          var createDelegateMethodInfo = typeof (Delegate).GetMethod (
              "CreateDelegate",
              new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
          var interceptDelegateCreateExpression = Expression.Call (
              null,
              createDelegateMethodInfo,
              Expression.Constant (interceptDelegateType),
              Expression.ArrayAccess (Expression.Field (bodyContext.This, allMethodAspectsArrayField), lastItem),
              Expression.Constant (interceptMethodInfo, typeof (MethodInfo)));
          var interceptDelegateAssignmentExpression = Expression.Assign (
              interceptDelegateVariableExpression,
              Expression.Convert (interceptDelegateCreateExpression, interceptDelegateType));

          // invocations[i - 1] = new OuterInvocation(context, delegate, aspects[i - 1])
          var invocationType = typeof (OuterInvocation);
          var invocationCreateExpression = Expression.New (
              invocationType.GetConstructors().Single(),
              invocationContextVariableExpression,
              Expression.Convert (interceptDelegateVariableExpression, interceptDelegateType),
              Expression.ArrayAccess (invocationsVariableExpression, lastItem));
          var invocationAssignExpression = Expression.Assign (
              Expression.ArrayAccess (invocationsVariableExpression, Expression.Constant (i)), invocationCreateExpression);

          var expression = Expression.Block (
              new[] { interceptDelegateVariableExpression },
              interceptDelegateAssignmentExpression,
              invocationAssignExpression
              );

          expressionList.Add (expression);
        }
      }

      return Expression.Block (expressionList);
    }

    private Type GetMethodDelegateType (MethodInfo methodInfo)
    {
      var parameters = methodInfo.GetParameters().Select (x => x.ParameterType);
      var delegateTypes = parameters.Concat (new[] { methodInfo.ReturnType }).ToArray();
      return Expression.GetDelegateType (delegateTypes);
    }

    private Expression GetReturnOrEmptyExpression (MutableMethodInfo methodInfo, Expression invocationContext)
    {
      if (methodInfo.ReturnType == typeof (void))
        return Expression.Empty();
      else
      {
        return Expression.Convert (
            Expression.Property (invocationContext, "ReturnValue"),
            methodInfo.ReturnType);
      }
    }

    private Expression GetReturnValueDefaultOrEmptyExpression (MutableMethodInfo methodInfo, ParameterExpression invocationContextVariableExpression)
    {
      if (methodInfo.ReturnType == typeof (void) || !methodInfo.ReturnType.IsValueType)
        return Expression.Empty();
      else
      {
        return Expression.Assign (
            Expression.Property (invocationContextVariableExpression, "ReturnValue"),
            Expression.Default (methodInfo.ReturnType));
      }
    }

    private MethodInfo GetAspectInterceptMethodInfo (AspectAttribute aspectAttribute, MutableMethodInfo methodInfo)
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