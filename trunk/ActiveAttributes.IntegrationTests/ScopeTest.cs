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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Attributes.Aspects;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [Ignore]
  [TestFixture]
  public class ScopeTest
  {
    private DomainType _instance1;
    private DomainType _instance2;

    [SetUp]
    public void SetUp ()
    {
      _instance1 = ObjectFactory.Create<DomainType>();
      _instance2 = ObjectFactory.Create<DomainType>();
    }

    [Test]
    public void InstanceAdviced ()
    {
      var result1 = _instance1.InstanceAdvicedMethod();
      var result2 = _instance2.InstanceAdvicedMethod();

      Assert.That (result1, Is.Not.EqualTo (result2));
    }

    [Test]
    public void StaticAdviced ()
    {
      var result1 = _instance1.StaticAdvicedMethod ();
      var result2 = _instance2.StaticAdvicedMethod ();

      Assert.That (result1, Is.EqualTo (result2));
    }

    public class DomainType
    {
      [DomainAspect (AdviceScope = AdviceScope.Instance)]
      public virtual int InstanceAdvicedMethod () { return 0; }

      [DomainAspect (AdviceScope = AdviceScope.Static)]
      public virtual int StaticAdvicedMethod () { return 0; }
    }

    public class DomainAspectAttribute : MethodInterceptionAspectAttributeBase
    {
      private int _counter;

      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Context.ReturnValue = _counter++;
      }
    }
  }
}