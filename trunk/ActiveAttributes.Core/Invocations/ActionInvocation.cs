using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class ActionInvocation<TInstance> : TypedInvocation<TInstance>
  {
    public ActionInvocation (IActionInvocationContext<TInstance> context, Action action)
      : base (context, () => action ())
    {
    }

    public ActionInvocation (IActionInvocationContext<TInstance> context, Action<IInvocation> innerProceed, IInvocation innerInvocation)
      : base (context, () => innerProceed (innerInvocation))
    {
    }

    public new IActionInvocationContext<TInstance> Context
    {
      get { return (IActionInvocationContext<TInstance>) base.Context; }
    }
  }

  public class ActionInvocation<TInstance, TA0> : TypedInvocation<TInstance>
  {
    public ActionInvocation (IActionInvocationContext<TInstance, TA0> context, Action<TA0> action)
        : base (context, () => action (context.Arg0))
    {
    }

    public ActionInvocation (IActionInvocationContext<TInstance, TA0> context, Action<IInvocation> innerProceed, IInvocation innerInvocation)
        : base (context, () => innerProceed (innerInvocation))
    {
    }

    public new IActionInvocationContext<TInstance, TA0> Context
    {
      get { return (IActionInvocationContext<TInstance, TA0>) base.Context; }
    }
  }
}