using System;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Common
{
  /// <summary>
  /// Catches exceptions of a certain type; namely <see cref="Exception"/> unless otherwise stated in the constructor.
  /// </summary>
  public class CatchExceptionAspectAttribute : MethodInterceptionAspectAttribute
  {
    private readonly Type _exceptionType;

    public CatchExceptionAspectAttribute (Type exceptionType = null)
    {
      _exceptionType = exceptionType ?? typeof (Exception);
    }

    public override void OnIntercept (IInvocation invocation)
    {
      try
      {
        invocation.Proceed();
      }
      catch (Exception exception)
      {
        if (!_exceptionType.IsInstanceOfType(exception))
          throw;
      }
    }
  }
}