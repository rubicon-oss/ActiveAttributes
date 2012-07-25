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
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using ActiveAttributes.Core.Assembly;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ProceedTest : TestBase
  {
    private DomainType _instance;

    [SetUp]
    public void SetUp ()
    {
      base.SetUp();

      var type = AssembleType<DomainType> (Assembler.Singleton.ModifyType);
      _instance = (DomainType) Activator.CreateInstance (type);
    }

    [Test]
    public void ProceedMethod ()
    {
      _instance.Method1();

      Assert.That (_instance.Method1Called, Is.True);
    }

    [Test]
    public void NotProceedMethod ()
    {
      _instance.Method2();

      Assert.That (_instance.Method2Called, Is.False);
    }

    [Test]
    public void ProceedProperty ()
    {
      _instance.Property1 = "test";

      Assert.That (_instance.Property1Called, Is.True);
      _instance.Property1Called = false;

      var x = _instance.Property1;
      Assert.That (_instance.Property1Called, Is.True);
    }

    [Test]
    public void NotProceedProperty ()
    {
      _instance.Property2 = "test";

      Assert.That (_instance.Property2Called, Is.False);
      _instance.Property2Called = false;

      var x = _instance.Property2;
      Assert.That (_instance.Property2Called, Is.False);
    }

    public class DomainType
    {
      public bool Method1Called { get; set; }
      public bool Method2Called { get; set; }
      public bool Property1Called { get; set; }
      public bool Property2Called { get; set; }

      [ProceedMethodAspect]
      public virtual void Method1 ()
      {
        Method1Called = true;
      }

      [NoProceedMethodAspect]
      public virtual void Method2 ()
      {
        Method2Called = true;
      }

      [ProceedPropertyAspect]
      public virtual string Property1
      {
        get
        {
          Property1Called = true;
          return "test";
        }
        set { Property1Called = true; }
      }

      [NoProceedPropertyAspect]
      public virtual string Property2
      {
        get
        {
          Property2Called = true;
          return "test";
        }
        set { Property2Called = true; }
      }

      public class ProceedMethodAspectAttribute : MethodInterceptionAspectAttribute
      {
        public override void OnIntercept (IInvocation invocation)
        {
          invocation.Proceed();
        }
      }

      public class NoProceedMethodAspectAttribute : MethodInterceptionAspectAttribute
      {
        public override void OnIntercept (IInvocation invocation)
        {
        }
      }

      public class ProceedPropertyAspectAttribute : PropertyInterceptionAspectAttribute
      {
        public override void OnInterceptGet (IInvocation invocation)
        {
          invocation.Proceed();
        }

        public override void OnInterceptSet (IInvocation invocation)
        {
          invocation.Proceed();
        }
      }

      public class NoProceedPropertyAspectAttribute : PropertyInterceptionAspectAttribute
      {
        public override void OnInterceptGet (IInvocation invocation)
        {
        }

        public override void OnInterceptSet (IInvocation invocation)
        {
        }
      }
    }
  }
}