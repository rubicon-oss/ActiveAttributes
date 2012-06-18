// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class MethodBoundaryAspectTest
  {
    private TestableMethodBoundaryAspect _obj;
    private Invocation _throwingInvocation;
    private Invocation _blankInvocation;

    [SetUp]
    public void SetUp ()
    {
      _obj = new TestableMethodBoundaryAspect();

      _throwingInvocation = new Invocation (new Action (() => { throw new Exception("Throwing Invocation"); }), null, null);
      _blankInvocation = new Invocation (new Action (() => { }), null, null);
    }

    [Test]
    public void OnEntry ()
    {
      var invocation = new Invocation (new Action (() => Assert.That (_obj.OnEntryCalled, Is.True)), null, null);

      _obj.OnInvoke (invocation);
    }

    [Test]
    public void OnExit ()
    {
      var invocation = new Invocation (new Action (() => Assert.That (_obj.OnExitCalled, Is.False)), null, null);

      _obj.OnInvoke (invocation);

      Assert.That (_obj.OnExitCalled, Is.True);
    }

    [Test]
    public void OnExit_WithException ()
    {
      try
      {
        _obj.OnInvoke (_throwingInvocation);
      }
      catch
      {
      }

      Assert.That (_obj.OnExitCalled, Is.True);
      Assert.That (_obj.ThrowedException, Is.TypeOf<Exception>());
      Assert.That (_obj.ThrowedException.Message, Is.EqualTo ("Throwing Invocation"));
    }

    [Test]
    public void OnException_WithException ()
    {
      try
      {
        _obj.OnInvoke (_throwingInvocation);
      }
      catch (Exception)
      {
      }

      Assert.That (_obj.OnExceptionCalled, Is.True);
    }

    [Test]
    public void OnException_WithoutException ()
    {
      _obj.OnInvoke (_blankInvocation);

      Assert.That (_obj.OnExceptionCalled, Is.False);
    }

    [Test]
    public void OnSuccess_WithException ()
    {
      try
      {
        _obj.OnInvoke (_throwingInvocation);
      }
      catch (Exception)
      {
      }

      Assert.That (_obj.OnSuccessCalled, Is.False);
    }

    [Test]
    public void OnSuccess_WithoutException ()
    {
      _obj.OnInvoke (_blankInvocation);

      Assert.That (_obj.OnSuccessCalled, Is.True);
    }

    [Test]
    [ExpectedException (typeof (Exception), ExpectedMessage = "Throwing Invocation")]
    public void FlowBehavior_Rethrow ()
    {
      var rethrowingAspect = new RethrowingMethodBoundaryAspect ();
      rethrowingAspect.OnInvoke (_throwingInvocation);
    }

    [Test]
    public void FlowBehavior_Continuing_Explicit ()
    {
      var rethrowingAspect = new ExplicitContinuingMethodBoundaryAspect ();
      rethrowingAspect.OnInvoke (_throwingInvocation);
    }

    [Test]
    public void FlowBehavior_Continuing_Implicit ()
    {
      var rethrowingAspect = new ImplicitContinuingMethodBoundaryAspect ();
      rethrowingAspect.OnInvoke (_throwingInvocation);
    }

    class TestableMethodBoundaryAspect : MethodBoundaryAspect
    {
      public bool OnEntryCalled { get; private set; }
      public override void OnEntry (Invocation invocation) { OnEntryCalled = true; }

      public bool OnExitCalled { get; private set; }
      public override void OnExit (Invocation invocation) { OnExitCalled = true; }

      public bool OnExceptionCalled { get; private set; }
      public Exception ThrowedException { get; private set; }
      public override void OnException (Invocation invocation) { OnExceptionCalled = true; ThrowedException = invocation.Exception; }

      public bool OnSuccessCalled { get; private set; }
      public override void OnSuccess (Invocation invocation) { OnSuccessCalled = true; }
    }

    class RethrowingMethodBoundaryAspect : TestableMethodBoundaryAspect
    {
      public override void OnException (Invocation invocation)
      {
        invocation.FlowBehavior = FlowBehavior.Rethrow;
      }
    }

    class ExplicitContinuingMethodBoundaryAspect : TestableMethodBoundaryAspect
    {
      public override void OnException (Invocation invocation)
      {
        invocation.FlowBehavior = FlowBehavior.Continue;
      }
    }

    class ImplicitContinuingMethodBoundaryAspect : TestableMethodBoundaryAspect
    {
      public override void OnException (Invocation invocation)
      {
        invocation.Exception = null;
      }
    }
  }
}