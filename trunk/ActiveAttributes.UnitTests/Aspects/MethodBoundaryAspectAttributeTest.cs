using System;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Aspects
{
  [TestFixture]
  public class MethodBoundaryAspectAttributeTest
  {
    private TestableMethodBoundaryAspectAttribute _obj;
    private FuncInvocation<object, int, int> _returningInvocation;
    private ActionInvocation<object> _throwingInvocation;

    private ActionInvocationContext<object> _actionInvocationContext;
    private FuncInvocationContext<object, int, int> _funcInvocationContext;

    [SetUp]
    public void SetUp ()
    {
      _obj = new TestableMethodBoundaryAspectAttribute();

      _actionInvocationContext = new ActionInvocationContext<object> ();
      _throwingInvocation = new ActionInvocation<object> (_actionInvocationContext, () => { throw new Exception ("Throwing invocation"); });

      _funcInvocationContext = new FuncInvocationContext<object, int, int>();
      _returningInvocation = new FuncInvocation<object, int, int> (_funcInvocationContext, i => i);
    }

    [Test]
    public void OnEntry_BeforeProceed ()
    {
      var invocation = new ActionInvocation<object> (_actionInvocationContext, () => Assert.That (_obj.OnEntryCalled, Is.True));

      _obj.OnIntercept (invocation);
    }

    [Test]
    [ExpectedException (typeof (AspectInvocationException))]
    public void OnEntry_WrapException ()
    {
      var instance = new OnEntryThrowingMethodBoundaryAspectAttribute();

      try
      {
        instance.OnIntercept (_returningInvocation);
      }
      catch (Exception ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("TODO"));
        Assert.That (ex.InnerException.Message, Is.EqualTo ("OnEntry"));
        throw;
      }
    }

    [Test]
    public void OnExit_AfterProceed ()
    {
      var invocation = new ActionInvocation<object> (_actionInvocationContext, () => Assert.That (_obj.OnExitCalled, Is.False));

      _obj.OnIntercept (invocation);

      Assert.That (_obj.OnExitCalled, Is.True);
    }

    [Test]
    public void OnExit_WhenExceptionOccures ()
    {
      try
      {
        _obj.OnIntercept (_throwingInvocation);
      }
      catch {}

      Assert.That (_obj.OnExitCalled, Is.True);
      Assert.That (_obj.Exception, Is.TypeOf<Exception>());
      Assert.That (_obj.Exception.Message, Is.EqualTo ("Throwing invocation"));
    }

    [Test]
    [ExpectedException (typeof (AspectInvocationException))]
    public void OnExit_WrapException ()
    {
      var instance = new OnExitThrowingMethodBoundaryAspectAttribute();

      try
      {
        instance.OnIntercept (_returningInvocation);
      }
      catch (Exception ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("TODO"));
        Assert.That (ex.InnerException.Message, Is.EqualTo ("OnExit"));
        throw;
      }
    }

    [Test]
    public void OnSuccess_WithException ()
    {
      try
      {
        _obj.OnIntercept (_throwingInvocation);
      }
      catch (Exception) {}

      Assert.That (_obj.OnSuccessCalled, Is.False);
    }

    [Test]
    public void OnSuccess_WithoutException ()
    {
      _returningInvocation.Context.Arg0 = 1;
      _obj.OnIntercept (_returningInvocation);

      Assert.That (_obj.OnSuccessCalled, Is.True);
      Assert.That (_obj.ReturnValue, Is.EqualTo (1));
    }


    [Test]
    [ExpectedException(typeof(AspectInvocationException))]
    public void OnSuccess_WrapException ()
    {
      var instance = new OnSuccessThrowingMethodBoundaryAspectAttribute();

      try
      {
        instance.OnIntercept (_returningInvocation);
      }
      catch (Exception ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("TODO"));
        Assert.That (ex.InnerException.Message, Is.EqualTo ("OnSuccess"));
        throw;
      }
    }

    [Test]
    public void OnException_WithException ()
    {
      try
      {
        _obj.OnIntercept (_throwingInvocation);
      }
      catch (Exception) {}

      Assert.That (_obj.OnExceptionCalled, Is.True);
    }

    [Test]
    public void OnException_WithoutException ()
    {
      _obj.OnIntercept (_returningInvocation);

      Assert.That (_obj.OnExceptionCalled, Is.False);
    }

    [Test]
    [ExpectedException (typeof (AspectInvocationException))]
    public void OnException_WrapException ()
    {
      var instance = new OnExceptionThrowingMethodBoundaryAspectAttribute();

      try
      {
        instance.OnIntercept (_throwingInvocation);
      }
      catch (Exception ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("TODO"));
        Assert.That (ex.InnerException.Message, Is.EqualTo ("OnException"));
        throw;
      }
    }

    private class TestableMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      public bool OnEntryCalled { get; private set; }

      public bool OnExitCalled { get; private set; }

      public bool OnExceptionCalled { get; private set; }
      public Exception Exception { get; private set; }

      public bool OnSuccessCalled { get; private set; }
      public object ReturnValue { get; private set; }

      protected override void OnEntry (IReadOnlyInvocation invocationInfo)
      {
        OnEntryCalled = true;
      }

      protected override void OnExit (IReadOnlyInvocation invocation)
      {
        OnExitCalled = true;
      }

      protected override void OnException (IReadOnlyInvocation invocation, Exception exception)
      {
        OnExceptionCalled = true;
        Exception = exception;
      }

      protected override void OnSuccess (IReadOnlyInvocation invocation, object returnValue)
      {
        OnSuccessCalled = true;
        ReturnValue = returnValue;
      }
    }

    private class OnEntryThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnEntry (IReadOnlyInvocation invocationInfo)
      {
        throw new Exception ("OnEntry");
      }
    }

    private class OnExitThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnExit (IReadOnlyInvocation invocationInfo)
      {
        throw new Exception ("OnExit");
      }
    }

    private class OnExceptionThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnException (IReadOnlyInvocation invocationInfo, Exception exception)
      {
        throw new Exception ("OnException");
      }
    }

    private class OnSuccessThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnSuccess (IReadOnlyInvocation invocationInfo, object returnValue)
      {
        throw new Exception ("OnSuccess");
      }
    }
  }
}