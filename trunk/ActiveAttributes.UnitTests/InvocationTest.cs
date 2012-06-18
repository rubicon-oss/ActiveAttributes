// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Reflection;
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class InvocationTest
  {
    [Test]
    public void Proceed ()
    {
      var called = false;
      var @delegate = new Action (() => called = true);
      var invocation = new Invocation (@delegate, null, null);

      invocation.Proceed();

      Assert.That (called, Is.True);
    }

    [Test]
    public void Proceed_WithArguments ()
    {
      object[] result = null;
      var @delegate = new Action<string, int> ((str, i) => result = new object[] { str, i });
      var arguments = new object[] { "test", 5 };
      var invocation = new Invocation (@delegate, null, null, arguments);

      invocation.Proceed ();

      Assert.That (result, Is.EqualTo (arguments));
    }

    [Test]
    public void Proceed_WithModifiedArguments ()
    {
      object[] result = null;
      var @delegate = new Action<string, int>((str, i) => result = new object[] { str, i });
      var arguments = new object[] { "test", 5 };
      var invocation = new Invocation (@delegate, null, null, arguments);

      invocation.Arguments[1] = 2;
      invocation.Proceed ();

      Assert.That (result[1], Is.EqualTo (2));
    }

    [Test]
    public void Proceed_ReturnValue ()
    {
      var @delegate = new Func<int> (() => 10);
      var invocation = new Invocation (@delegate, null, null);

      invocation.Proceed();

      Assert.That (invocation.ReturnValue, Is.EqualTo (10));
    }

    [Test]
    public void Proceed_Nested ()
    {
      var counter = 0;
      var @delegate = new Func<int, string, int> ((i, str) => ++counter);
      var innerInvocation = new Invocation (@delegate, null, null, 1, "test");
      var outerInvocation = new Invocation (innerInvocation);

      outerInvocation.Proceed();

      Assert.That (counter, Is.EqualTo (1));
      Assert.That (outerInvocation.Arguments, Is.EqualTo (innerInvocation.Arguments));
      Assert.That (outerInvocation.ReturnValue, Is.EqualTo (1));
    }
  }
}