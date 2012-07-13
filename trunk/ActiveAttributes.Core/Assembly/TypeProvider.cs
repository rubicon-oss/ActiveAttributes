// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Assembly
{
  public interface ITypeProvider
  {
    Type GetInvocationType ();
    Type GetInvocationContextType ();
  }

  public class TypeProvider : ITypeProvider
  {
    private static readonly Type[] _actionInvocationOpenTypes
        = new[]
          {
              typeof (ActionInvocation<>),
              typeof (ActionInvocation<,>),
              typeof (ActionInvocation<,,>)
          };

    private static readonly Type[] _funcInvocationOpenTypes
        = new[]
          {
              null,
              typeof (FuncInvocation<,>),
              typeof (FuncInvocation<,,>),
              typeof (FuncInvocation<,,,>)
          };

    private static readonly Type[] _actionInvocationContextOpenTypes
        = new[]
          {
              typeof (ActionInvocationContext<>),
              typeof (ActionInvocationContext<,>),
              typeof (ActionInvocationContext<,,>)
          };

    private static readonly Type[] _funcInvocationContextOpenTypes
        = new[]
          {
              null,
              typeof (FuncInvocationContext<,>),
              typeof (FuncInvocationContext<,,>),
              typeof (FuncInvocationContext<,,,>)
          };


    private static readonly Type[] _actionOpenTypes
        = new[]
          {
              typeof (Action),
              typeof (Action<>),
              typeof (Action<,>)
          };

    private static readonly Type[] _funcOpenTypes
        = new[]
          {
              null,
              typeof (Func<>),
              typeof (Func<,>)
          };


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
        return GetActionInvocationOpenType ();
      else
        return GetFuncInvocationOpenType();
    }

    private Type GetActionInvocationOpenType ()
    {
      var typeArguments = _instanceType.Concat(_parameterTypes).ToArray();
      var openType = _actionInvocationOpenTypes[typeArguments.Length - 1];
      return openType.MakeGenericType (typeArguments);
    }

    private Type GetFuncInvocationOpenType ()
    {
      var typeArguments = _instanceType.Concat(_parameterTypes).Concat (_returnType).ToArray();
      var openType = _funcInvocationOpenTypes[typeArguments.Length - 1];
      return openType.MakeGenericType (typeArguments);
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
      var typeArguments = _instanceType.Concat (_parameterTypes).ToArray ();
      var openType = _actionInvocationContextOpenTypes[typeArguments.Length - 1];
      return openType.MakeGenericType (typeArguments);
    }

    private Type GetFuncInvocationContextType ()
    {
      var typeArguments = _instanceType.Concat (_parameterTypes).Concat(_returnType).ToArray ();
      var openType = _funcInvocationContextOpenTypes[typeArguments.Length - 1];
      return openType.MakeGenericType (typeArguments);
    }

    #endregion

    #region InvocationActionType

    public Type GetDelegateType ()
    {
      if (_isAction)
        return GetActionDelegateType ();
      else
        return GetFuncDelegateType ();
    }

    private Type GetActionDelegateType ()
    {
      var typeArguments = _parameterTypes;
      var openType = _actionOpenTypes[typeArguments.Length];
      if (openType.IsGenericTypeDefinition)
        return openType.MakeGenericType (typeArguments);
      else
        return openType;
    }

    private Type GetFuncDelegateType ()
    {
      var typeArguments = _parameterTypes.Concat (_returnType).ToArray();
      var openType = _actionOpenTypes[typeArguments.Length];
      if (openType.IsGenericTypeDefinition)
        return openType.MakeGenericType (typeArguments);
      else
        return openType;
    }

    #endregion
  }
}