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
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ArgumentManipulationTest : TestBase
  {
    [Test]
    public void IncrementArguments ()
    {
      var instance = ObjectFactory.Create<DomainType>();
      var a = 10;
      var b = 20;
      var actual = instance.Multiply (a, b);
      var expected = (a + 1) * (b + 1);

      Assert.That (actual, Is.EqualTo (expected));
    }

    public class DomainType
    {
      [IncrementArgumentsAspect]
      public virtual int Multiply (int a, int b) { return a * b; }
    }

    public class IncrementArgumentsAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        var arguments = invocation.Context.Arguments;
        for (var i = 0; i < arguments.Count; i++)
          arguments[i] = (int) arguments[i] + 1;

        invocation.Proceed();
      }
    }
  }
}