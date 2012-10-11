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
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ReturnValueManipulationTest : TestBase
  {
    private DomainType _instance;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      var type = AssembleType<DomainType> (Assembler.Singleton.ModifyType);
      _instance = type.CreateInstance<DomainType>();
    }

    [Test]
    public void ReturnNullReferenceType ()
    {
      Assert.That (_instance.StringMethod (), Is.Null);
    }

    [Test]
    public void ReturnTenReferenceType ()
    {
      Assert.That (_instance.IntMethod (), Is.EqualTo (10));
    }

    [Test]
    [ExpectedException(typeof(NullReferenceException))]
    public void ReturnNullValueType ()
    {
      // TODO: is this behavior really expected?
      _instance.IntMethod22 ();
    }

    public class DomainType
    {
      [ReturnNullAspect]
      public virtual string StringMethod ()
      {
        return "abc";
      }

      [ReturnTenAspect]
      public virtual int IntMethod ()
      {
        return 1;
      }

      [ReturnNullAspect]
      public virtual int IntMethod22 ()
      {
        return 1;
      }
    }

    public class ReturnNullAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Context.ReturnValue = null;
      }
    }

    public class ReturnTenAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Context.ReturnValue = 10;
      }
    }
  }
}