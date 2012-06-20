using System;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocation;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Aspects
{
  [TestFixture]
  public class MethodBoundaryAspectAttributeTest
  {
    #region Setup/Teardown

    [SetUp]
    public void SetUp ()
    {
      _obj = new TestableMethodBoundaryAspectAttribute();

      _returningInvocation = new FuncInvocation<object, int> (null, null, null, () => 1);
      _throwingInvocation = new ActionInvocation (null, null, null, () => { throw new Exception ("Throwing Invocation2"); });


      // TODO use mock
      //_mockRepository = new MockRepository();
      //_invocationMock = _mockRepository.StrictMock<IInvocation>();
      //_throwingInvocationMock = _mockRepository.StrictMock<IInvocation>();
    }

    #endregion

    private TestableMethodBoundaryAspectAttribute _obj;

    //private MockRepository _mockRepository;

    //private IInvocation _invocationMock;
    //private IInvocation _throwingInvocationMock;

    private FuncInvocation<object, int> _returningInvocation;
    private ActionInvocation _throwingInvocation;

    private class TestableMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      public bool OnEntryCalled { get; private set; }

      public bool OnExitCalled { get; private set; }

      public bool OnExceptionCalled { get; private set; }
      public Exception Exception { get; private set; }

      public bool OnSuccessCalled { get; private set; }
      public object ReturnValue { get; private set; }

      protected override void OnEntry (IInvocationInfo invocationInfo)
      {
        OnEntryCalled = true;
      }

      protected override void OnExit (IInvocationInfo invocation)
      {
        OnExitCalled = true;
      }

      protected override void OnException (IInvocationInfo invocation, Exception exception)
      {
        OnExceptionCalled = true;
        Exception = exception;
      }

      protected override void OnSuccess (IInvocationInfo invocation, object returnValue)
      {
        OnSuccessCalled = true;
        ReturnValue = returnValue;
      }
    }

    private class OnEntryThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnEntry (IInvocationInfo invocationInfo)
      {
        throw new Exception ("OnEntry");
      }
    }

    private class OnExitThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnExit (IInvocationInfo invocationInfo)
      {
        throw new Exception ("OnExit");
      }
    }

    private class OnExceptionThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnExit (IInvocationInfo invocationInfo)
      {
        throw new Exception ("OnException");
      }
    }

    private class OnSuccessThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnExit (IInvocationInfo invocationInfo)
      {
        throw new Exception ("OnSuccess");
      }
    }

    [Test]
    public void OnEntry_BeforeProceed ()
    {
      var invocation = new ActionInvocation (null, null, null, () => Assert.That (_obj.OnEntryCalled, Is.True));

      _obj.OnIntercept (invocation);
    }

    [Test]
    public void OnEntry_WrapException ()
    {
      var instance = new OnEntryThrowingMethodBoundaryAspectAttribute();

      try
      {
        instance.OnIntercept (_returningInvocation);
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.TypeOf<AspectInvocationException>());
        Assert.That (ex.Message, Is.EqualTo ("TODO"));
        Assert.That (ex.InnerException.Message, Is.EqualTo ("OnEntry"));
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
    public void OnException_WrapException ()
    {
      var instance = new OnExceptionThrowingMethodBoundaryAspectAttribute();

      try
      {
        instance.OnIntercept (_returningInvocation);
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.TypeOf<AspectInvocationException>());
        Assert.That (ex.Message, Is.EqualTo ("TODO"));
        Assert.That (ex.InnerException.Message, Is.EqualTo ("OnException"));
      }
    }

    [Test]
    public void OnExit_AfterProceed ()
    {
      var invocation = new ActionInvocation (null, null, null, () => Assert.That (_obj.OnExitCalled, Is.False));

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
      Assert.That (_obj.Exception.Message, Is.EqualTo ("Throwing Invocation2"));
    }

    [Test]
    public void OnExit_WrapException ()
    {
      var instance = new OnExitThrowingMethodBoundaryAspectAttribute();

      try
      {
        instance.OnIntercept (_returningInvocation);
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.TypeOf<AspectInvocationException>());
        Assert.That (ex.Message, Is.EqualTo ("TODO"));
        Assert.That (ex.InnerException.Message, Is.EqualTo ("OnExit"));
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
      _obj.OnIntercept (_returningInvocation);

      Assert.That (_obj.OnSuccessCalled, Is.True);
      Assert.That (_obj.ReturnValue, Is.EqualTo (1));
    }


    [Test]
    public void OnSuccess_WrapException ()
    {
      var instance = new OnSuccessThrowingMethodBoundaryAspectAttribute();

      try
      {
        instance.OnIntercept (_returningInvocation);
      }
      catch (Exception ex)
      {
        Assert.That (ex, Is.TypeOf<AspectInvocationException>());
        Assert.That (ex.Message, Is.EqualTo ("TODO"));
        Assert.That (ex.InnerException.Message, Is.EqualTo ("OnSuccess"));
      }
    }
  }
}