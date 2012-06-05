using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;

namespace ActiveAttributes.Core
{
  public class Assembler : ITypeAssemblyParticipant
  {
    public void ModifyType (MutableType mutableType)
    {
      foreach (var originalMethod in mutableType.AllMutableMethods.ToArray())
      {
        var method = originalMethod;
        var customAttributes = method.GetCustomAttributes (typeof (Aspect), true);

        if (customAttributes.Length == 0)
          continue;

        var newMethod = mutableType.AddMethod (
            originalMethod.Name + "_privcop",
            MethodAttributes.Private,
            originalMethod.ReturnType,
            ParameterDeclaration.CreateForEquivalentSignature (originalMethod),
            ctx => ctx.GetCopiedMethodBody (method, ctx.Parameters.Cast<Expression>()));

        var parameterTypes = new List<Type>();
        parameterTypes.AddRange (originalMethod.GetParameters().Select (x => x.ParameterType));
        parameterTypes.Add (originalMethod.ReturnType);

        var delegateType = Expression.GetDelegateType (parameterTypes.ToArray());
        var aspectType = customAttributes.Cast<Aspect>().Single().GetType();
        var createDelegateMethodInfo = typeof (Delegate).GetMethod (
            "CreateDelegate",
            new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
        var onInvokeMethodInfo = typeof (Aspect).GetMethod ("OnInvoke");

        var invocation = Expression.Variable (typeof (Invocation));
        var aspect = Expression.Variable (typeof (Aspect));


        originalMethod.SetBody (
            ctx =>
            Expression.Block (

                new[] { invocation, aspect },

                // Assign invocation object
                Expression.Assign (
                    invocation,
                    Expression.New (
                        typeof (Invocation).GetConstructors().Single(),
                        // Create delegate
                        Expression.Call (
                            null,
                            createDelegateMethodInfo,
                            Expression.Constant (delegateType),
                            ctx.This,
                            Expression.Constant (newMethod, typeof (MethodInfo))),
                        Expression.NewArrayInit (
                            typeof (object),
                            ctx.Parameters.Cast<Expression>()))),

                // Assign aspect object
                Expression.Assign (
                    aspect,
                    Expression.New (aspectType)),

                // Call OnInvoke with invocation
                Expression.Call (
                    aspect,
                    onInvokeMethodInfo,
                    invocation),

                // Return
                method.ReturnType == typeof(void)
                  ? (Expression) Expression.Default(typeof(void))
                  : (Expression) Expression.Convert(Expression.Property (invocation, "ReturnValue"),
                    method.ReturnType))
            );
      }
    }
  }
}