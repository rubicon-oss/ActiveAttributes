using System;
using System.Windows.Forms;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Common
{
  /// <summary>
  /// Invokes a method call if required.
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