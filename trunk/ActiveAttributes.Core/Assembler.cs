using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        {
          //var attributes = method.GetCustomAttributes (typeof (CompilerGeneratedAttribute), true);
          //if (attributes.Length == 1)
          //{
          //  var propertyInfo = mutableType.UnderlyingSystemType.GetProperty (method.Name.Substring (4));
          //  customAttributes = propertyInfo.GetCustomAttributes (typeof (Aspect), true);
          //}

          if (customAttributes.Length == 0)
            continue;
        }

        // introduce field and init in ctors
        var aspectsFieldInfo = mutableType.AddField (typeof (Aspect[]), "tp<>__aspects_" + method.Name);
        var aspectAttributes = customAttributes.Cast<Aspect> ();


        ValidateMethod(method, aspectAttributes);

        AddAspectsConstructorInitialization(mutableType, aspectAttributes, aspectsFieldInfo);

        // copy method body to helper method
        var newMethod = CreateHelperMethod(mutableType, method);




        // adapt original method body to
        // - create invocation object (delegate, arguments)
        // - invoke aspects with invocation object
        // - set return type
        var parameterTypes = new List<Type>();
        parameterTypes.AddRange (originalMethod.GetParameters().Select (x => x.ParameterType));
        parameterTypes.Add (originalMethod.ReturnType);

        var delegateType = Expression.GetDelegateType (parameterTypes.ToArray());
        var createDelegateMethodInfo = typeof (Delegate).GetMethod (
            "CreateDelegate",
            new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
        var onInvokeMethodInfo = typeof (Aspect).GetMethod ("OnInvoke");

        var invocation = Expression.Variable (typeof (Invocation));
        var aspect = Expression.Variable (typeof (Aspect));
        var aspects = Expression.Variable (typeof (Aspect[]));
        var i = Expression.Variable (typeof (int));
        var label = Expression.Label();

        //mutableType.GetOrAddMutableMethod

        originalMethod.SetBody (
            ctx =>
            Expression.Block (

                new[] { invocation, aspect, aspects, i },

                // Aspect[] aspects = _aspects_methodName
                Expression.Assign (
                    aspects,
                    Expression.Field (ctx.This, aspectsFieldInfo)
                    ),

                // Invocation invocation
                //   = new Invocation(delegate(newMethod), this, methodInfo, new object[] { arg0, arg1, ... });
                Expression.Assign (
                    invocation,

                    Expression.New (
                        typeof (Invocation).GetConstructors().First(),

                        Expression.Call (
                            null,
                            createDelegateMethodInfo,
                            Expression.Constant (delegateType),
                            ctx.This,
                            Expression.Constant (newMethod, typeof (MethodInfo))),

                        Expression.Constant (
                            method.UnderlyingSystemMethodInfo,
                            typeof (MethodInfo)),

                        ctx.This,

                        Expression.NewArrayInit (
                            typeof (object),
                            ctx.Parameters.Select (
                                x =>
                                (Expression) Expression.Convert (x, typeof (object)))))),

                // invocation.ReturnValue = default(returnType)
                GetInitReturnValueExpression(invocation, method),

                // while (i < aspects.Length) { aspects[i].OnInvoke(invocation); i++; }
                Expression.Loop (
                    Expression.IfThenElse (
                        Expression.LessThan (i, Expression.ArrayLength (aspects)),
                        Expression.Block (
                            Expression.Assign (
                                aspect,
                                Expression.ArrayAccess (aspects, i)),
                            Expression.Call (
                                aspect,
                                onInvokeMethodInfo,
                                invocation
                                ),
                            Expression.PostIncrementAssign (i)
                            ),
                        Expression.Break (label)),
                    label),

              // return (ReturnType) invocation.ReturnValue;
                method.ReturnType == typeof (void)
                  ? (Expression) Expression.Default (typeof (void))
                  : (Expression) Expression.Convert (Expression.Property (invocation, "ReturnValue"),
                    method.ReturnType)));


      }
    }

    private static Expression GetInitReturnValueExpression (ParameterExpression invocation, MutableMethodInfo method)
    {
      if (method.ReturnType == typeof (void) || !method.ReturnType.IsValueType)
        return Expression.Empty();
      else
      {
        return Expression.Assign (
            Expression.Property (invocation, "ReturnValue"),
            Expression.Convert (Expression.Default (method.ReturnType), typeof (object)));
      }
    }

    private static MutableMethodInfo CreateHelperMethod (MutableType mutableType, MutableMethodInfo method)
    {
      return mutableType.AddMethod (
          method.Name + "_privcop",
          MethodAttributes.Private,
          method.ReturnType,
          ParameterDeclaration.CreateForEquivalentSignature (method),
          ctx => ctx.GetCopiedMethodBody (method, ctx.Parameters.Cast<Expression>()));
    }

    private static void ValidateMethod (MutableMethodInfo method, IEnumerable<Aspect> aspectAttributes)
    {
      foreach (var aspectAttribute in aspectAttributes)
      {
        if (!aspectAttribute.Validate (method))
        {
          var message = string.Format ("Method '{0}' is not valid for aspect of type '{1}'", method.Name, aspectAttribute.GetType().Name);
          throw new InvalidOperationException (message);
        }
      }
    }

    private static void AddAspectsConstructorInitialization (MutableType mutableType, IEnumerable<Aspect> aspectAttributes, MutableFieldInfo aspectsFieldInfo)
    {
      foreach (var ctor in mutableType.AllMutableConstructors)
      {
        ctor.SetBody (
            ctx =>
            Expression.Block (
                Expression.Assign (
                    Expression.Field (ctx.This, aspectsFieldInfo),
                    Expression.NewArrayInit (
                        typeof (Aspect),
                        aspectAttributes
                            .OrderByDescending (x => x.Priority)
                            .Select (x => Expression.New (x.GetType()))
                            .Cast<Expression>())),
                ctx.GetPreviousBodyWithArguments()));
      }
    }
  }
}