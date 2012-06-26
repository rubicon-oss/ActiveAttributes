// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Contexts;
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

    public class DomainType
    {
      [DomainAspect]
      public virtual string Anything { get; set; }

      public string AnotherValue { get; set; }
    }

    public class DomainAspectAttribute : PropertyInterceptionAspectAttribute
    {
      public override void OnInterceptGet (IInvocation invocation)
      {
        var context = (FuncInvocationContext<DomainType, string>) invocation.Context;
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