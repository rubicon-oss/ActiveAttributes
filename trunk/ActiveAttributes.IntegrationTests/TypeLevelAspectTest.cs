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
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class TypeLevelAspectTest : TestBase
  {
    private DomainClass _instance1;
    private DomainClass _instance2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      var type = AssembleType<DomainClass> (Assembler.Singleton.ModifyType);
      _instance1 = type.CreateInstance<DomainClass> ();
      _instance2 = type.CreateInstance<DomainClass> ();
    }

    [Test]
    public void InterfaceAspect ()
    {
      var result1 = _instance1.Method1();
      var result2 = _instance1.Method2();

      Assert.That (result1, Is.EqualTo (result2));
    }

    [DomainAspect]
    public class DomainClass
    {
      public virtual Guid Method1 ()
      {
        return Guid.NewGuid();
      }
      public virtual Guid Method2 ()
      {
        return Guid.NewGuid();
      }
    }

    [AttributeUsage(AttributeTargets.Class)]
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