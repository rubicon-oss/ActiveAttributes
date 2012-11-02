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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (InvocationTypeProvider2))]
  public interface IInvocationTypeProvider2
  {
    void GetInvocationTypes (MethodInfo method, out Type invocationType, out Type invocationContextType);
  }

  public class InvocationTypeProvider2 : IInvocationTypeProvider2
  {
    private readonly Type[] _actionInvocationOpenTypes
        = new[]
          {
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
              typeof (FuncInvocation<,>),
              typeof (FuncInvocation<,,>),
              typeof (FuncInvocation<,,,>),
              typeof (FuncInvocation<,,,,>),
              typeof (FuncInvocation<,,,,,>)
          };

    private readonly Type[] _actionInvocationContextOpenTypes
        = new[]
          {
              typeof (ActionInvocationContext<>),
              typeof (ActionInvocationContext<,>),
              typeof (ActionInvocationContext<,,>),
              typeof (ActionInvocationContext<,,,>),
              typeof (ActionInvocationContext<,,,,>)
          };

    private readonly Type[] _funcInvocationContextOpenTypes
        = new[]
          {
              null,
              typeof (FuncInvocationContext<,>),
              typeof (FuncInvocationContext<,,>),
              typeof (FuncInvocationContext<,,,>),
              typeof (FuncInvocationContext<,,,,>),
              typeof (FuncInvocationContext<,,,,,>)
          };

    public void GetInvocationTypes (MethodInfo method, out Type invocationType, out Type invocationContextType)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      Assertion.IsNotNull (method.DeclaringType);

      // TODO UNDERLYING
      var instanceType = new[] { method.DeclaringType.UnderlyingSystemType };
      var parameterTypes = method.GetParameters().Select (x => x.ParameterType).ToArray();
      var returnType = new[] { method.ReturnType };

      var genericTypes = method.IsAction()
                             ? instanceType.Concat (parameterTypes).ToArray()
                             : instanceType.Concat (parameterTypes).Concat (returnType).ToArray();

      var property = method.GetRelatedPropertyInfo();
      var event_ = method.GetRelatedEventInfo();
      if (property != null)
        GetInvocationOpenTypes (method, property, out invocationType, out invocationContextType);
      else if (event_ != null)
        GetInvocationOpenTypes (method, event_, out invocationType, out invocationContextType);
      else
        GetInvocationOpenTypes (method, out invocationType, out invocationContextType, parameterTypes.Length);

      invocationType = invocationType.MakeGenericType (genericTypes);
      invocationContextType = invocationContextType.MakeGenericType (genericTypes);
    }

    private void GetInvocationOpenTypes (MethodInfo method, out Type invocationType, out Type invocationContextType, int parameterCount)
    {
      if (method.IsAction())
      {
        invocationType = _actionInvocationOpenTypes[parameterCount];
        invocationContextType = _actionInvocationContextOpenTypes[parameterCount];
      }
      else
      {
        invocationType = _funcInvocationOpenTypes[parameterCount + 1];
        invocationContextType = _funcInvocationContextOpenTypes[parameterCount + 1];
      }
    }

    private void GetInvocationOpenTypes (MethodInfo method, PropertyInfo property, out Type invocationType, out Type invocationContextType)
    {
      if (property.IsIndexer())
      {
        throw new NotImplementedException();
      }
      else
      {
        if (method.IsAction())
        {
          invocationType = typeof (PropertySetInvocation<,>);
          invocationContextType = typeof (PropertySetInvocationContext<,>);
        }
        else
        {
          invocationType = typeof (PropertyGetInvocation<,>);
          invocationContextType = typeof (PropertyGetInvocationContext<,>);
        }
      }
    }

    private void GetInvocationOpenTypes (MethodInfo method, EventInfo event_, out Type invocationType, out Type invocationContextType)
    {
      throw new NotImplementedException();
    }
  }
}