using System;
using System.Windows.Forms;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Common
{
  /// <summary>
  /// Executes a method on the UI-thread.
  /// </summary>
  public sealed class InvokeAspectAttribute : MethodInterceptionAspectAttribute
  {
    public override void OnIntercept (IInvocation invocation)
    {
      var control = (Control) invocation.Context.Instance;
      if (control.InvokeRequired)
        control.BeginInvoke (new Action (invocation.Proceed));
      else
        invocation.Proceed();
    }
  }
}