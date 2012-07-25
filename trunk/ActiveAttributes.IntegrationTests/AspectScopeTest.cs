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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class AspectScopeTest : TestBase
  {
    private DomainType _instance1;
    private DomainType _instance2;

    [SetUp]
    public void SetUp ()
    {
      base.SetUp();

      var type = AssembleType<DomainType> (new Assembler().ModifyType);
      _instance1 = (DomainType) Activator.CreateInstance (type);
      _instance2 = (DomainType) Activator.CreateInstance (type);
    }

    [Test]
    public void InstanceScope ()
    {
      var guid1 = _instance1.InstanceMethod ();
      var guid2 = _instance2.InstanceMethod ();

      Assert.That (guid1, Is.Not.EqualTo (guid2));
    }

    [Test]
    public void StaticScope ()
    {
      var guid1 = _instance1.StaticMethod ();
      var guid2 = _instance2.StaticMethod ();

      Assert.That (guid1, Is.EqualTo (guid2));
    }

    public class DomainType
    {
      [DomainAspect (Scope = AspectScope.Instance)]
      public virtual Guid InstanceMethod () { return Guid.Empty; }

      [DomainAspect (Scope = AspectScope.Static)]
      public virtual Guid StaticMethod () { return Guid.Empty; }
    }

    public class DomainAspectAttribute : MethodInterceptionAspectAttribute
    {
      private readonly Guid _guid = Guid.NewGuid();

      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Context.ReturnValue = _guid;
      }
    }
  }
}