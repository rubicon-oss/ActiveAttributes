using System;
using ActiveAttributes.Core.Invocation;

namespace ActiveAttributes.Core.Aspects
{
  /// <summary>
  /// Provides common pointcuts for the execution of methods. This aspect provides observing functionality only.
  /// </summary>
  public class MethodBoundaryAspectAttribute : MethodInterceptionAspectAttribute
  {
    public override sealed void OnIntercept (IInvocation invocation)
    {
      WrapIfThrowing (() => OnEntry (invocation));

      try
      {
        invocation.Proceed();
        OnSuccess (invocation, invocation.ReturnValue);
      }
      catch (Exception exception)
      {
        OnException (invocation, exception);
        throw;
      }
      finally
      {
        WrapIfThrowing (() => OnExit (invocation));
      }
    }

    private void WrapIfThrowing (Action action)
    {
      try
      {
        action();
      }
      catch (Exception exception)
      {
        throw new AspectInvocationException ("TODO", exception);
      }
    }

    protected virtual void OnEntry (IInvocationInfo invocationInfo) {}
    protected virtual void OnSuccess (IInvocationInfo invocationInfo, object returnValue) {}
    protected virtual void OnException (IInvocationInfo invocationInfo, Exception exception) {}
    protected virtual void OnExit (IInvocationInfo invocationInfo) {}
  }
}