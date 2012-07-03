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
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Contexts.ArgumentCollection;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class Test1
  {
    [Test]
    public void name ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      instance.Anything = "muh";
      instance.AnotherValue = "kuh";

      Assert.That (instance.Anything, Is.EqualTo ("muhkuh"));
    }

    ////[ApplyAspect(typeof(DomainAspectAttribute), If = AttributeTargets.Class)]
    //[ApplyAspect(typeof(DomainAspectAttribute), If = Is.Not(typeof(void)))]
    public class DomainType
    {
      [DomainAspect]
      public virtual string Anything { get; set; }

      public string AnotherValue { get; set; }
    }

    //public class ApplyAspectAttribute : Attribute
    //{
    //  public ApplyAspect (Type type)
    //  {
        
    //  }
    //  public object If { get; set; }
    //}

    //public static class Is
    //{
    //  public static 
    //  public static Type Not(Type t)
    //  {
    //    return null;
    //  }
    //}

    public class DomainAspectAttribute : PropertyInterceptionAspectAttribute
    {
      public override void OnInterceptGet (IInvocation invocation)
      {
        var context = (FuncInvocationContext<DomainType, string>) invocation.Context;
        var argsList = (IArgumentCollection) invocation.Context;
        invocation.Proceed();
        context.ReturnValue += context.Instance.AnotherValue;
      }

      public override void OnInterceptSet (IInvocation invocation)
      {
        invocation.Proceed();
      }
    }
  }
}