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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Extensions;
using ActiveAttributes.Interception.Invocations;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Interception
{
  public interface IInterceptionTypeProvider
  {
    Type GetInvocationType (MethodInfo method);
  }

  public class InterceptionTypeProvider : IInterceptionTypeProvider
  {
    private readonly Type[] _actionInvocationOpenTypes
        = new[]
          {
              null,
              typeof (ActionInvocation<>),
              typeof (ActionInvocation<,>),
              typeof (ActionInvocation<,,>),
              typeof (ActionInvocation<,,,>),
              typeof (ActionInvocation<,,,,>)
          };

    private readonly Type[] _funcInvocationOpenTypes
        = new[]
          {
              null,
              null,
              typeof (FuncInvocation<,>),
              typeof (FuncInvocation<,,>),
              typeof (FuncInvocation<,,,>),
              typeof (FuncInvocation<,,,,>),
              typeof (FuncInvocation<,,,,,>)
          };

    public Type GetInvocationType (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      Assertion.IsNotNull (method.DeclaringType);

      // TODO Underlying
      var declaringType = method.DeclaringType;
      declaringType = declaringType is MutableType ? declaringType.BaseType : declaringType;
      var instanceType = new[] { declaringType };
      var parameterTypes = method.GetParameters ().Select (x => x.ParameterType).ToArray ();
      var returnType = new[] { method.ReturnType };

      var genericTypes = instanceType.Concat (parameterTypes).Concat (method.IsFunc() ? returnType : new Type[0]).ToArray();

      var type = method.IsAction()
                     ? _actionInvocationOpenTypes[((ICollection<Type>) genericTypes).Count]
                     : _funcInvocationOpenTypes[((ICollection<Type>) genericTypes).Count];

      return genericTypes.Any()
                 ? type.MakeGenericType (genericTypes)
                 : type;
    }
  }
}