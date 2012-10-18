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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Checked;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class InterfaceAspectsTest : TestBase
  {
    [Test]
    public void InterfaceAspect ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      var result = instance.Method();

      Assert.That (result, Is.EqualTo ("interface_aspect"));
    }

    public class DomainType : IDomainInterface
    {
      public virtual string Method () { return "method"; }
    }

    public interface IDomainInterface
    {
      [DomainAspect]
      string Method ();
    }

    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class DomainAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Context.ReturnValue = "interface_aspect";
      }
    }
  }
}