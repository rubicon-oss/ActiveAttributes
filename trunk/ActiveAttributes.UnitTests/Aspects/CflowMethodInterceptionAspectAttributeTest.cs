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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Aspects
{
  [TestFixture]
  public class CflowMethodInterceptionAspectAttributeTest
  {
    [Test]
    public void InExecutionOf ()
    {
      var invocation = MockRepository.GenerateStub<IInvocation>();
      invocation.Expect (x => x.Proceed()).Repeat.Never();
      var instance = new TestableCflowAspectAttribute { ExecutionOf = "CflowMethodInterceptionAspectAttributeTest.Method1" };

      Method1 (() => instance.OnIntercept (invocation));

      Assert.That (instance.OnCflowInterceptCalled, Is.True);
      Assert.That (instance.Invocation, Is.SameAs (invocation));
      invocation.VerifyAllExpectations();
    }

    [Test]
    public void NotInExecutionOf ()
    {
      var invocation = MockRepository.GenerateMock<IInvocation> ();
      invocation.Expect (x => x.Proceed());
      var instance = new TestableCflowAspectAttribute { ExecutionOf = "CflowMethodInterceptionAspectAttributeTest.Method1" };

      Method2 (() => instance.OnIntercept (invocation));

      Assert.That (instance.OnCflowInterceptCalled, Is.False);
      invocation.VerifyAllExpectations();
    }

    private void Method1 (Action action)
    {
      action();
    }
    private void Method2 (Action action)
    {
      action();
    }

    class TestableCflowAspectAttribute : CflowMethodInterceptionAspectAttribute
    {
      public bool OnCflowInterceptCalled { get; private set; }

      public IInvocation Invocation { get; private set; }

      protected override void OnCflowIntercept (IInvocation invocation)
      {
        Invocation = invocation;
        OnCflowInterceptCalled = true;
      }
    }
  }
}