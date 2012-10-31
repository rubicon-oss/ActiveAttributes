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

#region

using System;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;

#endregion

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

      _actionInvocationContext = new ActionInvocationContext<object> (null, null);
      _throwingInvocation = new ActionInvocation<object> (_actionInvocationContext, () => { throw new Exception ("Throwing invocation"); });

      _funcInvocationContext = new FuncInvocationContext<object, int, int> (null, null, 1);
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
      catch
      {
      }

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
      catch (Exception)
      {
      }

      Assert.That (_obj.OnSuccessCalled, Is.False);
    }

    [Test]
    public void OnSuccess_WithoutException ()
    {
      var invocationContext = (FuncInvocationContext<object, int, int>) _returningInvocation.Context;
      invocationContext.Arg1 = 1;
      _obj.OnIntercept (_returningInvocation);

      Assert.That (_obj.OnSuccessCalled, Is.True);
      Assert.That (_obj.ReturnValue, Is.EqualTo (1));
    }


    [Test]
    [ExpectedException (typeof (AspectInvocationException))]
    public void OnSuccess_WrapException ()
    {
      var instance = new OnSuccessThrowingMethodBoundaryAspectAttribute();

      try
      {
        instance.OnIntercept (_returningInvocation);
      }
      catch (Exception ex)
      {
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
      catch (Exception)
      {
      }

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
        Assert.That (ex.InnerException.Message, Is.EqualTo ("OnException"));
        throw;
      }
    }

    private class OnEntryThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnEntry (IReadOnlyInvocation invocationInfo)
      {
        throw new Exception ("OnEntry");
      }
    }

    private class OnExceptionThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnException (IReadOnlyInvocation invocationInfo, Exception exception)
      {
        throw new Exception ("OnException");
      }
    }

    private class OnExitThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnExit (IReadOnlyInvocation invocationInfo)
      {
        throw new Exception ("OnExit");
      }
    }

    private class OnSuccessThrowingMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected override void OnSuccess (IReadOnlyInvocation invocationInfo, object returnValue)
      {
        throw new Exception ("OnSuccess");
      }
    }

    private class TestableMethodBoundaryAspectAttribute : MethodBoundaryAspectAttribute
    {
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

      public bool OnEntryCalled { get; private set; }

      public bool OnExitCalled { get; private set; }

      public bool OnExceptionCalled { get; private set; }
      public Exception Exception { get; private set; }

      public bool OnSuccessCalled { get; private set; }
      public object ReturnValue { get; private set; }
    }
  }
}