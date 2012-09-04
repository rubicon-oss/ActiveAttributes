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
using System.Reflection;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Collections;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class TypedAspectTest
  {
    [Test]
    public void InterceptTyped ()
    {
      var instance = ObjectFactory.Create<DomainType>();
      var arg0 = new DomainType.NestedDomainType { Name = "Fabian" };

      var result = instance.Method (arg0);

      Assert.AreEqual (result, "Stefan");
    }

    public class DomainType
    {
      public class NestedDomainType
      {
        public string Name { get; set; }
      }

      [DomainAspect]
      public virtual string Method (NestedDomainType obj)
      {
        return obj.Name;
      }
    }

    public class DomainAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override bool Matches (MethodInfo methodInfo)
      {
        var parameters = methodInfo.GetParameters ();

        if (parameters.Length != 1)
          return false;

        if (parameters[0].ParameterType != typeof (DomainType.NestedDomainType))
          return false;

        if (methodInfo.ReturnType != typeof (string))
          return false;

        return true;
      }

      public override void OnIntercept (IInvocation invocation)
      {
        var funcInvocationContext = (FuncInvocationContext<DomainType, DomainType.NestedDomainType, string>) invocation.Context;

        var arg0 = funcInvocationContext.Arg1;

        arg0.Name = "Stefan";

        invocation.Proceed ();
      }
    }
  }
}