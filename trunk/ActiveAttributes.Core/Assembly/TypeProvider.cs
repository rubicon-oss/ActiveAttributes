// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Assembly
{
  public class TypeProvider
  {
    private readonly bool _isAction;
    private readonly Type[] _instanceType;
    private readonly Type[] _parameterTypes;
    private readonly Type[] _returnType;
    private readonly int _parameterCount;

    public TypeProvider (MethodInfo methodInfo)
    {
      _isAction = methodInfo.ReturnType == typeof (void);
      _instanceType = new[] { methodInfo.DeclaringType.UnderlyingSystemType };
      _parameterTypes = methodInfo.GetParameters().Select (x => x.ParameterType).ToArray();
      _returnType = new[] { methodInfo.ReturnType };
      _parameterCount = _parameterTypes.Length;
    }

    #region InvocationType

    public Type GetInvocationType ()
    {
      if (_isAction)
        return GetActionInvocationType ();
      else
        return GetFuncInvocationType();
    }

    private Type GetActionInvocationType ()
    {
      var baseType = GetActionInvocationBaseType ();
      return baseType.MakeGenericType (_instanceType.Concat(_parameterTypes).ToArray());
    }

    private Type GetFuncInvocationType ()
    {
      var baseType = GetFuncInvocationBaseType();
      return baseType.MakeGenericType (_instanceType.Concat(_parameterTypes).Concat (_returnType).ToArray());
    }

    private Type GetActionInvocationBaseType ()
    {
      switch (_parameterCount)
      {
        case 1: return typeof (ActionInvocation<,>);
        default: throw new NotSupportedException ();
      }
    }

    private Type GetFuncInvocationBaseType ()
    {
      switch (_parameterCount)
      {
        case 1: return typeof (FuncInvocation<,,>);
        default: throw new NotSupportedException ();
      }
    }

    #endregion

    #region InvocationContextType

    public Type GetInvocationContextType ()
    {
      if (_isAction)
        return GetActionInvocationContextType ();
      else
        return GetFuncInvocationContextType ();
    }

    private Type GetActionInvocationContextType ()
    {
      var baseType = GetActionInvocationContextBaseType ();
      return baseType.MakeGenericType (_instanceType.Concat(_parameterTypes).ToArray());
    }

    private Type GetFuncInvocationContextType ()
    {
      var baseType = GetFuncInvocationContextBaseType ();
      return baseType.MakeGenericType (_instanceType.Concat(_parameterTypes).Concat (_returnType).ToArray ());
    }

    private Type GetActionInvocationContextBaseType ()
    {
      switch (_parameterCount)
      {
        case 1: return typeof (ActionInvocationContext<,>);
        default: throw new NotSupportedException ();
      }
    }

    private Type GetFuncInvocationContextBaseType ()
    {
      switch (_parameterCount)
      {
        case 1: return typeof (FuncInvocationContext<,,>);
        default: throw new NotSupportedException ();
      }
    }

    #endregion

    #region InvocationActionType

    public Type GetInvocationActionType ()
    {
      if (_isAction)
        return GetActionInvocationActionType ();
      else
        return GetFuncInvocationActionType ();
    }

    private Type GetActionInvocationActionType ()
    {
      var baseType = GetActionInvocationActionBaseType ();
      return baseType.MakeGenericType (_parameterTypes);
    }

    private Type GetFuncInvocationActionType ()
    {
      var baseType = GetFuncInvocationActionBaseType ();
      return baseType.MakeGenericType (_parameterTypes.Concat (_returnType).ToArray ());
    }

    private Type GetActionInvocationActionBaseType ()
    {
      switch (_parameterCount)
      {
        case 1: return typeof (Action<>);
        default: throw new NotSupportedException ();
      }
    }

    private Type GetFuncInvocationActionBaseType ()
    {
      switch (_parameterCount)
      {
        case 1: return typeof (Func<,>);
        default: throw new NotSupportedException ();
      }
    }

    #endregion
  }
}