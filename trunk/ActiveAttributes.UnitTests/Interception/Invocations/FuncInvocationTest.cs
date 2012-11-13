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
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.UnitTests.Interception.Invocations
{
  [TestFixture]
  public class FuncInvocationTest
  {
    [Test]
    public void Initialization ()
    {
      var instance = new DomainType ();
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (""));

      var invocation = new FuncInvocation<DomainType, string, int> (method, instance, "arg", instance.Method);

      Assert.That (invocation.Instance, Is.SameAs (instance));
      Assert.That (invocation.MemberInfo, Is.SameAs (method));
      Assert.That (invocation.Arg1, Is.EqualTo ("arg"));
    }

    [Test]
    public void Proceed ()
    {
      var instance = new DomainType ();
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (""));

      var invocation = new FuncInvocation<DomainType, string, int> (method, instance, "arg", instance.Method);

      invocation.Arg1 = "7";
      invocation.Proceed();
      Assert.That (invocation.ReturnValue, Is.EqualTo (7));
    }

    class DomainType
    {
      public int Method (string arg) { return int.Parse(arg); }
    }
  }
}