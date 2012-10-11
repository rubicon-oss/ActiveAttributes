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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using TypePipe.IntegrationTests;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class AspectScopeTest : TestBase
  {
    [Test]
    public void InstanceScope ()
    {
      var type = AssembleType<DomainType> (Assembler.Singleton.ModifyType);
      var instance1 = type.CreateInstance<DomainType>();
      var instance2 = type.CreateInstance<DomainType>();

      Assert.That (instance1.InstanceMethod(), Is.Not.EqualTo (instance2.InstanceMethod()));
    }

    [Test]
    public void StaticScope ()
    {
      var type = AssembleType<DomainType> (Assembler.Singleton.ModifyType);
      var instance1 = type.CreateInstance<DomainType> ();
      var instance2 = type.CreateInstance<DomainType>();

      Assert.That (instance1.StaticMethod(), Is.Not.EqualTo (Guid.Empty));
      Assert.That (instance1.StaticMethod(), Is.EqualTo (instance2.StaticMethod()));
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