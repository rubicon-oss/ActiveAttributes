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
using System.Diagnostics;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class InterceptionTest : TestBase
  {
    private DomainType _instance;

    [SetUp]
    public void SetUp ()
    {
      base.SetUp();

    }

    [Test]
    public void ProceedMethod ()
    {
      SkipDeletion();
      _instance = ObjectFactory.Create<DomainType> ();
      _instance.Method1 ();

      Assert.That (_instance.Method1Called, Is.True);
    }

    [Test]
    public void NotProceedMethod ()
    {
      _instance.Method2 ();

      Assert.That (_instance.Method2Called, Is.False);
    }

    [Test]
    public void IncrementProperty ()
    {
      _instance.Integer = 1;

      Assert.That (_instance.Integer, Is.EqualTo (5));
    }

    [Test]
    public void IncrementPropertyNoSet ()
    {
      _instance.IntegerNoSet = 1;

      Assert.That (_instance.IntegerNoSet, Is.EqualTo (2));
    }

    public class DomainType
    {
      public bool Method1Called { get; set; }
      public bool Method2Called { get; set; }

      [ProceedingAspect]
      public virtual void Method1 () { Method1Called = true; }

      [NotProceedingAspect]
      public virtual void Method2 () { Method2Called = true; }

      [IncrementPropertyAspect]
      public virtual int Integer { get; set; }

      [IncrementPropertyAspect]
      public virtual int IntegerNoSet { get; [NotProceedingAspect] set; }
    }

    public class ProceedingAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Proceed();
      }
    }

    public class NotProceedingAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
      }
    }

    public class IncrementPropertyAspectAttribute : PropertyInterceptionAspectAttribute
    {
      public override void OnInterceptGet (IInvocation invocation)
      {
        invocation.Proceed();
        invocation.Context.ReturnValue = (int) invocation.Context.ReturnValue + 2;
      }

      public override void OnInterceptSet (IInvocation invocation)
      {
        invocation.Context.Arguments[0] = (int) invocation.Context.Arguments[0] + 2;
        invocation.Proceed();
      }
    }
  }
}