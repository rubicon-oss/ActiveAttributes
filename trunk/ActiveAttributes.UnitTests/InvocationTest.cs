// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class InvocationTest
  {
    private DomainType _obj;

    [SetUp]
    public void SetUp ()
    {
      _obj = new DomainType();
    }

    [Test]
    public void Proceed ()
    {
      var @delegate = new Action (_obj.Method1);
      var invocation = new Invocation (@delegate);

      invocation.Proceed();

      Assert.That (_obj.Method1Called, Is.True);
    }

    [Test]
    public void Proceed_WithArguments ()
    {
      var @delegate = new Action<string, int> (_obj.Method2);
      var arguments = new object[] { "test", 5 };
      var invocation = new Invocation (@delegate, arguments);

      invocation.Proceed ();

      Assert.That (_obj.Method2Arguments, Is.EqualTo (arguments));
    }

    [Test]
    public void Proceed_WithModifiedArguments ()
    {
      var @delegate = new Action<string, int> (_obj.Method2);
      var arguments = new object[] { "test", 5 };
      var invocation = new Invocation (@delegate, arguments);

      invocation.Arguments[1] = 2;
      invocation.Proceed ();

      Assert.That (_obj.Method2Arguments[1], Is.EqualTo (2));
    }

    [Test]
    public void Proceed_ReturnValue ()
    {
      var @delegate = new Func<int> (_obj.Method3);
      var invocation = new Invocation (@delegate);

      invocation.Proceed();

      Assert.That (invocation.ReturnValue, Is.EqualTo (10));
    }

    class DomainType
    {
      public void Method1 () { Method1Called = true; }
      public bool Method1Called { get; private set; }

      public void Method2 (string arg0, int arg1) { Method2Arguments = new object[] { arg0, arg1 }; }
      public object[] Method2Arguments { get; private set; }

      public int Method3 () { return 10; }
    }
  }
}