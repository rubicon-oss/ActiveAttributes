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
  public class ReturnManipulationTest : TestBase
  {
    private DomainType _instance;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      var type = AssembleType<DomainType> (Assembler.Singleton.ModifyType);
      _instance = (DomainType) Activator.CreateInstance (type);
    }

    [Test]
    public void ReturnNullReferenceType ()
    {
      var result = _instance.Method1 ();

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException(typeof(NullReferenceException))]
    public void ReturnNullValueType ()
    {
      // TODO: is this behavior really expected?
      _instance.Method2 ();
    }

    [Test]
    public void ReturnTenReferenceType ()
    {
      var result = _instance.Method3 ();

      Assert.That (result, Is.EqualTo (10));
    }

    public class DomainType
    {
      [ReturnNullAspect]
      public virtual string Method1 ()
      {
        return "abc";
      }

      [ReturnNullAspect]
      public virtual int Method2 ()
      {
        return 1;
      }

      [ReturnTenAspect]
      public virtual int Method3 ()
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