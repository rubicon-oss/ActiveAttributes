using System;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Aspects
{
  /// <summary>
  /// Provides common pointcuts for the execution of methods. This aspect provides observing functionality only.
  /// </summary>
  public class MethodBoundaryAspectAttribute : MethodInterceptionAspectAttribute
  {
    public override sealed void OnIntercept (IInvocation invocation)
    {
      var readOnlyInvocation = (IReadOnlyInvocation) invocation;
      WrapIfThrowing (() => OnEntry (readOnlyInvocation));

      try
      {
        invocation.Proceed();
        WrapIfThrowing (() => OnSuccess (readOnlyInvocation, invocation.Context.ReturnValue));
      }
      catch (Exception exception)
      {
        WrapIfThrowing (() => OnException (readOnlyInvocation, exception));
        throw;
      }
      finally
      {
        WrapIfThrowing (() => OnExit (readOnlyInvocation));
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

    protected virtual void OnEntry (IReadOnlyInvocation invocationInfo) {}
    protected virtual void OnSuccess (IReadOnlyInvocation invocationInfo, object returnValue) { }
    protected virtual void OnException (IReadOnlyInvocation invocationInfo, Exception exception) { }
    protected virtual void OnExit (IReadOnlyInvocation invocationInfo) { }
  }
}