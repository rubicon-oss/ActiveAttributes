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
using ActiveAttributes.Interception.Invocations;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Interception.Invocations
{
  [TestFixture]
  public class ControlFlowInvocationTest
  {
    [Test]
    public void Initialization ()
    {
      var contextMock = MockRepository.GenerateStrictMock<IInvocationContext>();
      var fakeMethod = ObjectMother.GetMethodInfo();
      var instance = new object();
      var arguments = MockRepository.GenerateStub<IArgumentCollection>();
      var returnValue = new object();
      contextMock.Expect (x => x.MemberInfo).Return (fakeMethod);
      contextMock.Expect (x => x.Instance).Return (instance);
      contextMock.Expect (x => x.Arguments).Return (arguments);
      contextMock.Expect (x => x.ReturnValue).Return (returnValue);

      var invocation = new ControlFlowInvocation (contextMock, () => { }, "condition", () => { });

      Dev.Null = invocation.MemberInfo;
      Dev.Null = invocation.Instance;
      Dev.Null = invocation.Arguments;
      Dev.Null = invocation.ReturnValue;

      contextMock.VerifyAllExpectations();
    }

    [Test]
    public void Set_ReturnValue ()
    {
      var contextMock = MockRepository.GenerateStrictMock<IInvocationContext>();
      var returnValue = new object();
      contextMock.Expect (x => x.ReturnValue).SetPropertyWithArgument (returnValue);

      var invocation = new ControlFlowInvocation (contextMock, () => { }, "", () => { });

      invocation.ReturnValue = returnValue;
      contextMock.VerifyAllExpectations();
    }

    [Test]
    public void Proceed_InnerMethod ()
    {
      var context = ObjectMother.GetInvocationContext();
      var condition = "Proceed_InnerMethod";

      var innerMethod = false;
      var nextInnerMethod = false;
      var invocation = new ControlFlowInvocation(context, () => innerMethod = true, condition, () => nextInnerMethod = true);

      invocation.Proceed();

      Assert.That (innerMethod, Is.True);
      Assert.That (nextInnerMethod, Is.False);
    }

    [Test]
    public void Proceed_NextInnerMethod ()
    {
      var context = ObjectMother.GetInvocationContext();
      var condition = "false condition";

      var innerMethod = false;
      var nextInnerMethod = false;
      var invocation = new ControlFlowInvocation(context, () => innerMethod = true, condition, () => nextInnerMethod = true);

      invocation.Proceed();

      Assert.That (nextInnerMethod, Is.True);
      Assert.That (innerMethod, Is.False);
    }
  }
}