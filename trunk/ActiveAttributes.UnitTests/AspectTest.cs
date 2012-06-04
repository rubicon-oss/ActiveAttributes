// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class AspectTest
  {
    [Test]
    public void OnInvoke ()
    {
      var called = false;
      var invocation = new Invocation (new Action (() => { called = true; }));
      var aspect = new TestableAspect();

      aspect.OnInvoke (invocation);

      Assert.That (called, Is.True);
    }

    class TestableAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        base.OnInvoke (invocation);
      } 
    }
  }
}