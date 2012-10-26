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
using Ninject.Activation;
using Ninject.Components;
using Ninject.MockingKernel;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public class RhinoMocksStubProvider : NinjectComponent, IProvider, IMockProviderCallbackProvider
  {
    /// <summary>
    /// Gets the type (or prototype) of instances the provider creates.
    /// </summary>
    public Type Type
    {
      get { return typeof (RhinoMocks); }
    }

    /// <summary>
    /// Creates an instance within the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The created instance.</returns>
    public object Create (IContext context)
    {
      //var mockRepo = context.Get
      var stub = MockRepository.GenerateStub (context.Request.Service);
      //var type = context.Request.Service;
      //var methods = type.GetMethods (BindingFlags.Instance | BindingFlags.Public);
      //foreach (var method in methods)
      //{
      //  var returnType = method.ReturnType;
      //  if (returnType.IsGenericType && returnType.GetGenericTypeDefinition () == typeof (IEnumerable<>))
      //  {
      //    var parameterCount = method.GetParameters ().Length;
      //    var elementType = returnType.GetGenericArguments().Single();
      //    var emptyReturn = Array.CreateInstance (elementType, 0);
      //    stub.Stub (x => method.Invoke (x, new object[parameterCount]))
      //        .IgnoreArguments ()
      //        .Return (emptyReturn);
      //  }
      //}
      return stub;
    }

    /// <summary>
    /// Gets a callback that creates an instance of the <see cref="IProvider"/> that creates the mock.
    /// </summary>
    /// <returns>The created callback.</returns>
    public Func<IContext, IProvider> GetCreationCallback ()
    {
      return ctx => this;
    }
  }
}