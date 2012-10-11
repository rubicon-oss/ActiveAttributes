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
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Invocations;
using Remotion.Utilities;

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
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      Assertion.IsTrue (methodInfo.DeclaringType != null);

      var instanceType = new[] { methodInfo.DeclaringType.UnderlyingSystemType };
      var parameterTypes = methodInfo.GetParameters().Select (x => x.ParameterType).ToArray();
      var returnType = new[] { methodInfo.ReturnType };

      Type invocationOpenType;
      Type invocationContextOpenType;

      var genericTypes = methodInfo.IsAction()
        ? instanceType.Concat (parameterTypes).ToArray ()
        : instanceType.Concat (parameterTypes).Concat (returnType).ToArray ();        

      if (methodInfo.IsPropertyAccessor())
      {
        var propertyInfo = methodInfo.GetRelatedPropertyInfo();
        if (methodInfo.IsAction())
        {
          if (propertyInfo.IsIndexer ())
          {
            invocationOpenType = typeof (IndexerSetInvocation<,,>);
            invocationContextOpenType = typeof (IndexerSetInvocationContext<,,>);
          }
          else
          {
            invocationOpenType = typeof (PropertySetInvocation<,>);
            invocationContextOpenType = typeof (PropertySetInvocationContext<,>);
          }
        }
        else
        {
          if (propertyInfo.IsIndexer ())
          {
            invocationOpenType = typeof (IndexerGetInvocation<,,>);
            invocationContextOpenType = typeof (IndexerGetInvocationContext<,,>);
          }
          else
          {
            invocationOpenType = typeof (PropertyGetInvocation<,>);
            invocationContextOpenType = typeof (PropertyGetInvocationContext<,>);
          }
        }
      }
      else if (methodInfo.IsEventAccessor ())
      {
        throw new NotImplementedException("TODO");
      }
      else
      {
        if (methodInfo.IsAction ())
        {
          invocationOpenType = _actionInvocationOpenTypes[genericTypes.Length - 1];
          invocationContextOpenType = _actionInvocationContextOpenTypes[genericTypes.Length - 1];
        }
        else
        {
          invocationOpenType = _funcInvocationOpenTypes[genericTypes.Length - 1];
          invocationContextOpenType = _funcInvocationContextOpenTypes[genericTypes.Length - 1];
        }
      }

      InvocationType = invocationOpenType.MakeGenericType (genericTypes);
      InvocationContextType = invocationContextOpenType.MakeGenericType (genericTypes);
    }

    public Type InvocationType { get; private set; }
    public Type InvocationContextType { get; private set; }
  }
}