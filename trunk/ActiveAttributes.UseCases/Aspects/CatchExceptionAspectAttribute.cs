//Sample license text.

using System;
using System.Windows.Forms;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.UseCases.Aspects
{
  public class CatchExceptionAspectAttribute : MethodInterceptionAspectAttribute
  {
    public override void OnIntercept (IInvocation invocation)
    {
      try
      {
        invocation.Proceed();
      }
      catch (Exception exception)
      {
        MessageBox.Show (exception.Message);
      }
    }
  }
}