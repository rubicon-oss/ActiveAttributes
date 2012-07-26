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
using ActiveAttributes.IntegrationTests;
using NUnit.Framework;

[assembly: AssemblyAspectTest.AssemblyAspectAttribute(IfType = typeof(AssemblyAspectTest.DomainClass))]

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class AssemblyAspectTest : TestBase
  {
    private DomainClass _instance;

    [SetUp]
    public void SetUp ()
    {
      var type = AssembleType<DomainClass> (Assembler.Singleton.ModifyType);
      _instance = type.CreateInstance<DomainClass>();
    }

    [Test]
    public void name ()
    {
      _instance.Method();
    }

    public interface IDomainInterface
    {
      Guid Method();
    }

    public class DomainClass : IDomainInterface
    {
      public virtual Guid Method()
      {
        return Guid.NewGuid();
      }
    }

    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyAspectAttribute : MethodInterceptionAspectAttribute
    {
      private readonly Guid _guid;

      public AssemblyAspectAttribute ()
      {
        _guid = Guid.NewGuid();
      }

      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Context.ReturnValue = _guid;
      }
    }
  }
}