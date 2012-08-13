//Sample license text.

using System;
using System.Windows.Forms;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.UseCases.Aspects
{
  public class InvokeAspectAttribute : MethodInterceptionAspectAttribute
  {
    public override void OnIntercept (IInvocation invocation)
    {
      var form = (Form) invocation.Context.Instance;
      form.Invoke (new Action (invocation.Proceed));
    }
  }
}