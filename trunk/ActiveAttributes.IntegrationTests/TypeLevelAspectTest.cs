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
using ActiveAttributes.Aspects;
using ActiveAttributes.Assembly;
using ActiveAttributes.Interception.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [Ignore]
  [TestFixture]
  public class TypeLevelAspectTest
  {
    [Test]
    public void InterfaceAspect ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      var result1 = instance.Method1();
      var result2 = instance.Method2();
      var result3 = instance.Method3();

      Assert.That (result1, Is.EqualTo (result2));
      Assert.That (result3, Is.Not.EqualTo (result1));
    }

    [DomainAspect]
    public class DomainType
    {
      public virtual Guid Method1 () { return Guid.NewGuid(); }
      public virtual Guid Method2 () { return Guid.NewGuid(); }
      public virtual Guid Method3 () { return Guid.NewGuid(); }
    }

    public class DomainAspectAttribute : MethodInterceptionAspectAttributeBase
    {
      private readonly Guid _guid = Guid.NewGuid();

      public override void OnIntercept (IInvocation invocation)
      {
        invocation.ReturnValue = _guid;
      }
    }
  }
}