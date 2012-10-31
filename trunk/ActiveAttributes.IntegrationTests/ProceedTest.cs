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
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;
using ActiveAttributes.Core.Assembly;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ProceedTest : TestBase
  {
    [Test]
    public void ProceedMethod ()
    {
      var type = AssembleType<DomainType1> (TypeAssembler.Singleton.ModifyType);
      var instance = type.CreateInstance<DomainType1> ();

      instance.Method1();

      Assert.That (instance.Method1Called, Is.True);
    }

    [Test]
    public void NotProceedMethod ()
    {
      var type = AssembleType<DomainType1> (TypeAssembler.Singleton.ModifyType);
      var instance = type.CreateInstance<DomainType1> ();

      instance.Method2();

      Assert.That (instance.Method2Called, Is.False);
    }

    [Test]
    public void ProceedProperty ()
    {
      var type = AssembleType<DomainType3> (TypeAssembler.Singleton.ModifyType);
      var instance = type.CreateInstance<DomainType3> ();
      SkipDeletion();
      
      instance.Property1 = "test";

      Assert.That (instance.Property1Called, Is.True);
      instance.Property1Called = false;

      var x = instance.Property1;

      Assert.That (instance.Property1Called, Is.True);
    }

    [Test]
    public void NotProceedProperty ()
    {
      var type = AssembleType<DomainType3> (TypeAssembler.Singleton.ModifyType);
      var instance = type.CreateInstance<DomainType3>();

      instance.Property2 = "test";

      Assert.That (instance.Property2Called, Is.False);
      instance.Property2Called = false;

      var x = instance.Property2;

      Assert.That (instance.Property2Called, Is.False);
    }


    [Test]
    public void ProceedPropertyAsMethod ()
    {
      var type = AssembleType<DomainType2> (TypeAssembler.Singleton.ModifyType);
      var instance = type.CreateInstance<DomainType2> ();

      instance.Property3 = "test";

      Assert.That (instance.Property3Called, Is.True);
      instance.Property3Called = false;

      var x = instance.Property3;

      Assert.That (instance.Property3Called, Is.True);
    }

    [Test]
    public void NotProceedPropertyAsMethod ()
    {
      var type = AssembleType<DomainType2> (TypeAssembler.Singleton.ModifyType);
      var instance = type.CreateInstance<DomainType2> ();

      instance.Property4 = "test";

      Assert.That (instance.Property4Called, Is.False);
      instance.Property4Called = false;

      var x = instance.Property4;

      Assert.That (instance.Property4Called, Is.False);
    }

    public class DomainType1
    {
      public bool Method1Called { get; set; }
      public bool Method2Called { get; set; }

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

    }

    public class DomainType2
    {
      public bool Property3Called { get; set; }
      public bool Property4Called { get; set; }

      public virtual string Property3
      {
        [ProceedMethodAspect]
        get
        {
          Property3Called = true;
          return "test";
        }
        [ProceedMethodAspect]
        set { Property3Called = true; }
      }

      public virtual string Property4
      {
        [NoProceedMethodAspect]
        get
        {
          Property4Called = true;
          return "test";
        }
        [NoProceedMethodAspect]
        set { Property4Called = true; }
      }
    }

    public class DomainType3
    {
      public virtual bool Property1Called { get; set; }
      public virtual bool Property2Called { get; set; }

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
       
    }

    public class DT4 : DomainType3
    {
       
    }

    public class ProceedMethodAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Proceed ();
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
      public override void OnInterceptGet (IPropertyInvocation invocation)
      {
        Assert.That (invocation.Context.PropertyInfo, Is.Not.Null);
        invocation.Proceed ();
      }

      public override void OnInterceptSet (IPropertyInvocation invocation)
      {
        invocation.Proceed ();
      }
    }

    public class NoProceedPropertyAspectAttribute : PropertyInterceptionAspectAttribute
    {
      public override void OnInterceptGet (IPropertyInvocation invocation)
      {
      }

      public override void OnInterceptSet (IPropertyInvocation invocation)
      {
      }
    }
  }
}