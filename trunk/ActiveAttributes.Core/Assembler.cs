using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Components.DictionaryAdapter;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace ActiveAttributes.Core
{
  public class Assembler : Remotion.TypePipe.TypeAssembly.ITypeAssemblyParticipant
  {

    // new Delegate() cannot be used because Delegate is abstract




    public void ModifyType (MutableType mutableType)
    {
      var originalMethod = mutableType.AllMutableMethods.First();
      var newMethod = mutableType.AddMethod (
          originalMethod.Name + "_privcop",
          MethodAttributes.Private,
          originalMethod.ReturnType,
          ParameterDeclaration.CreateForEquivalentSignature (originalMethod),
          ctx => originalMethod.Body);

      var parameterTypes = new List<Type> ();

      var parameterInfos = originalMethod.GetParameters ();
      if (parameterInfos.Length > 0)
        parameterTypes.AddRange (parameterInfos.Select (x => x.ParameterType));

      parameterTypes.Add (originalMethod.ReturnType);

      var delegateType = Expression.GetDelegateType (parameterTypes.ToArray());

      var dynamicInvokeMethodInfo = typeof (Delegate).GetMethod ("DynamicInvoke");
      var createDelegateMethodInfo = typeof (Delegate).GetMethod ("CreateDelegate", new[] { typeof (Type), typeof (object), typeof (MethodInfo) });

      var @delegate = Expression.Variable (typeof (Delegate));

      originalMethod.SetBody (
          ctx =>
          Expression.Block (
              new[] { @delegate },
              Expression.Assign (
                  @delegate,
                  Expression.Call (
                      null,
                      createDelegateMethodInfo,
                      Expression.Constant (delegateType),
                      ctx.This,
                      Expression.Constant (newMethod, typeof(MethodInfo)))
                  ),
              Expression.Convert (Expression.Call (@delegate, dynamicInvokeMethodInfo, Expression.NewArrayInit (typeof (object))), typeof (int))
              )
          );
    }
  }

  public static class BodyContextBaseExtensions
  {
    public static MethodCallExpression GetBodyAsDelegate (this MethodBodyModificationContext ctx)
    {
      var createDelegateMethodInfo = typeof (Delegate).GetMethod ("CreateDelegate", new[] { typeof (Type), typeof (object), typeof (MethodInfo) });

      var parameterTypes = ctx.Parameters.Select (x => x.Type);
      var delegateType = Expression.GetDelegateType (parameterTypes.ToArray ());

      var methodInfo = (MethodInfo) null;

      return Expression.Call (
          null,
          createDelegateMethodInfo,
          Expression.Constant (delegateType),
          ctx.This,
          Expression.Constant (methodInfo));
    }
  }
}
