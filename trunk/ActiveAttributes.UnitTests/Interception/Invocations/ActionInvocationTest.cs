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
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Interception.Invocations
{
  [TestFixture]
  public class ActionInvocationTest
  {
    [Test]
    public void Initialization ()
    {
      var method = ObjectMother2.GetMethodInfo();
      var instance = new DomainType();
      var context = new ActionInvocationContext<object, object> (method, instance, null);
      var invocation = new ActionInvocation<object, object> (context, instance.Method);
      invocation.Proceed ();

      Assert.That (invocation.Context, Is.SameAs (context));
      Assert.That (instance.MethodExecutionCounter, Is.EqualTo (1));
    }

    [Test]
    public void DelegatesContext ()
    {
      var method = ObjectMother2.GetMethodInfo();
      var instance = new DomainType();
      var arg1 = new object();
      var context = new ActionInvocationContext<object, object> (method, instance, arg1);
      var invocation = new ActionInvocation<object, object> (context, instance.Method);

      Assert.That (invocation.MethodInfo, Is.SameAs (method));
      Assert.That (invocation.Instance, Is.SameAs (instance));
      Assert.That (invocation.Arguments, Is.EqualTo (new[] { arg1 }));
    }

    class DomainType
    {
      public int MethodExecutionCounter { get; private set; }

      public void Method (object arg)
      {
        MethodExecutionCounter++;
      }
    }
  }
}