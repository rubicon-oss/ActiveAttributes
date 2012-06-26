using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;

namespace ActiveAttributes.Core.Assembly
{
  public class Assembler : ITypeAssemblyParticipant
  {
    private Type _aspectArrayType;
    private Type _aspectInstanceType;

    #region ITypeAssemblyParticipant Members

    public void ModifyType (MutableType mutableType)
    {
      var methodsWithAspectsProvider = new MethodWithAspectsProvider();
      var aspectsPreparer = new AspectPreparer();
      var methodPatcher = new MethodPatcher();
      foreach (var methodAndAspects in methodsWithAspectsProvider.GetMethodsWithAspects (mutableType))
      {
        var methodInfo = methodAndAspects.Item1;
        var aspectAttributes = methodAndAspects.Item2;
        var aspectsFieldInfo = aspectsPreparer.PrepareAspects (mutableType, methodInfo, aspectAttributes);
        methodPatcher.PatchMethod (mutableType, methodInfo, aspectsFieldInfo, aspectAttributes.ToArray());
      }

      return;
      foreach (var originalMethod in mutableType.AllMutableMethods.ToArray())
      {
        var method = originalMethod;
        var customAttributes = method.GetCustomAttributes (typeof (MethodInterceptionAspectAttribute), true);

        if (customAttributes.Length > 0)
        {
          _aspectInstanceType = typeof (MethodInterceptionAspectAttribute);
          _aspectArrayType = typeof (MethodInterceptionAspectAttribute[]);
        }
        else
        {
          //var attributes = method.GetCustomAttributes (typeof (CompilerGeneratedAttribute), true);
          //if (attributes.Length == 1)
          //{
          //  var propertyInfo = mutableType.UnderlyingSystemType.GetProperty (method.Name.Substring (4));
          //  customAttributes = propertyInfo.GetCustomAttributes (typeof (PropertyInterceptionAspectAttribute), true);
          //}

          //if (customAttributes.Length == 0)
          //  continue;

          //_aspectInstanceType = typeof (PropertyInterceptionAspectAttribute);
          //_aspectArrayType = typeof (PropertyInterceptionAspectAttribute[]);
        }

        // introduce field and init in ctors
        var aspectsFieldInfo = mutableType.AddField (_aspectArrayType, "tp<>__aspects_" + method.Name);
        var aspectAttributes = customAttributes.Cast<MethodInterceptionAspectAttribute>();

        ValidateMethod (method, aspectAttributes);

        AddAspectsInitializationToConstructors (mutableType, aspectAttributes, aspectsFieldInfo);

        // copy method body to helper method
        var newMethod = CreateHelperMethod (mutableType, method);


        // adapt original method body to
        // - create invocation object (delegate, arguments)
        // - invoke aspects with invocation object
        // - set return type
        var parameterTypes = new List<Type>();
        parameterTypes.AddRange (originalMethod.GetParameters().Select (x => x.ParameterType));
        parameterTypes.Add (originalMethod.ReturnType);

        // Func<TArg1, TArg2>
        // FuncInvocation<TArg1, TArg2>
        var delegateType = Expression.GetDelegateType (parameterTypes.ToArray());
        var createDelegateMethodInfo = typeof (Delegate).GetMethod (
            "CreateDelegate",
            new[] { typeof (Type), typeof (object), typeof (MethodInfo) });

        var invokeMethodInfo = GetInvocationMethod (method);


        // Expression variables
        var invocation = Expression.Variable (typeof (Invocation));
        var aspect = Expression.Variable (_aspectInstanceType);
        var aspects = Expression.Variable (_aspectArrayType);
        var i = Expression.Variable (typeof (int));
        var label = Expression.Label();

        originalMethod.SetBody (
            ctx =>
            Expression.Block (
                new[] { invocation, aspect, aspects, i },
                // AspectAttribute[] aspects = _aspects_methodName
                Expression.Assign (
                    aspects,
                    Expression.Field (ctx.This, aspectsFieldInfo)
                    ),
                // Invocation2 invocation
                //   = new Invocation2(delegate(newMethod), this, methodInfo, new object[] { arg0, arg1, ... });
                Expression.Assign (
                    invocation,
                    Expression.New (
                        // invocationType => FuncInvocation<TArg1, TArg2>
                        typeof (Invocation).GetConstructors().First(),
                        //Expression.Convert (delegateType,
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
                GetInitReturnValueExpression (invocation, method),
                // while (i < aspects.Length) { aspects[i].OnIntercept(invocation); i++; }
                Expression.Loop (
                    Expression.IfThenElse (
                        Expression.LessThan (i, Expression.ArrayLength (aspects)),
                        Expression.Block (
                            Expression.Assign (
                                aspect,
                                Expression.ArrayAccess (aspects, i)),
                            Expression.Call (
                                aspect,
                                invokeMethodInfo,
                                invocation
                                ),
                            Expression.PostIncrementAssign (i)
                            ),
                        Expression.Break (label)),
                    label),
                // return (ReturnType) invocation.ReturnValue;
                method.ReturnType == typeof (void)
                    ? (Expression) Expression.Default (typeof (void))
                    : (Expression) Expression.Convert (
                        Expression.Property (invocation, "ReturnValue"),
                        method.ReturnType)));
      }
    }

    #endregion

    private MethodInfo GetInvocationMethod (MutableMethodInfo method)
    {
      if (_aspectInstanceType == typeof (MethodInterceptionAspectAttribute))
        return typeof (MethodInterceptionAspectAttribute).GetMethod ("OnIntercept");
      else
      {
        //  switch (method.Name.Substring (0, 3))
        //  {
        //    case "set":
        //      return typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnSet");
        //    case "get":
        //      return typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnGet");
        //    default:
        //      throw new InvalidOperationException (); // TODO
        //  }
        throw new InvalidExpressionException();
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

    private static void ValidateMethod (MutableMethodInfo method, IEnumerable<MethodInterceptionAspectAttribute> aspectAttributes)
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

    private void AddAspectsInitializationToConstructors (
        MutableType mutableType, IEnumerable<MethodInterceptionAspectAttribute> aspectAttributes, MutableFieldInfo aspectsFieldInfo)
    {
      foreach (var ctor in mutableType.AllMutableConstructors)
      {
        ctor.SetBody (
            ctx =>
            Expression.Block (
                Expression.Assign (
                    Expression.Field (ctx.This, aspectsFieldInfo),
                    Expression.NewArrayInit (
                        _aspectInstanceType,
                        aspectAttributes
                            .OrderByDescending (x => x.Priority)
                            .Select (x => Expression.New (x.GetType()))
                            .Cast<Expression>())),
                ctx.GetPreviousBodyWithArguments()));
      }
    }
  }
}