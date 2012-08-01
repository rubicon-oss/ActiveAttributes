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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Assembly
{
  public interface ITypeProvider
  {
    Type InvocationType { get; }
    Type InvocationContextType { get; }
  }

  public class TypeProvider : ITypeProvider
  {
    private static readonly Type[] _actionInvocationOpenTypes
        = new[]
          {
              typeof (ActionInvocation<>),
              typeof (ActionInvocation<,>),
              typeof (ActionInvocation<,,>),
              typeof (ActionInvocation<,,,>),
              typeof (ActionInvocation<,,,,>)
          };

    private static readonly Type[] _funcInvocationOpenTypes
        = new[]
          {
              null,
              typeof (FuncInvocation<,>),
              typeof (FuncInvocation<,,>),
              typeof (FuncInvocation<,,,>),
              typeof (FuncInvocation<,,,,>),
              typeof (FuncInvocation<,,,,,>)
          };

    private static readonly Type[] _actionInvocationContextOpenTypes
        = new[]
          {
              typeof (ActionInvocationContext<>),
              typeof (ActionInvocationContext<,>),
              typeof (ActionInvocationContext<,,>),
              typeof (ActionInvocationContext<,,,>),
              typeof (ActionInvocationContext<,,,,>)
          };

    private static readonly Type[] _funcInvocationContextOpenTypes
        = new[]
          {
              null,
              typeof (FuncInvocationContext<,>),
              typeof (FuncInvocationContext<,,>),
              typeof (FuncInvocationContext<,,,>),
              typeof (FuncInvocationContext<,,,,>),
              typeof (FuncInvocationContext<,,,,,>)
          };

    public TypeProvider (MethodInfo methodInfo)
    {
      var instanceType = new[] { methodInfo.DeclaringType.UnderlyingSystemType };
      var parameterTypes = methodInfo.GetParameters().Select (x => x.ParameterType).ToArray();
      var returnType = new[] { methodInfo.ReturnType };

      var isPropertyAccessor = methodInfo.IsPropertyAccessor();
      var isEventAccessor = methodInfo.IsEventAccessor ();
      //if (!isPropertyAccessor && !isEventAccessor)
      //{
        if (methodInfo.IsAction ())
        {
          var genericTypes = instanceType.Concat (parameterTypes).ToArray ();
          InvocationType = GetType (_actionInvocationOpenTypes, genericTypes);
          InvocationContextType = GetType (_actionInvocationContextOpenTypes, genericTypes);
        }
        else
        {
          var genericTypes = instanceType.Concat (parameterTypes).Concat (returnType).ToArray ();
          InvocationType = GetType (_funcInvocationOpenTypes, genericTypes);
          InvocationContextType = GetType (_funcInvocationContextOpenTypes, genericTypes);
        }
      //}
    }

    private Type GetType (IList<Type> openTypes, Type[] genericTypes)
    {
      var openType = openTypes[genericTypes.Length - 1];
      return openType.MakeGenericType (genericTypes);
    }

    public Type InvocationType { get; private set; }
    public Type InvocationContextType { get; private set; }
  }
}