using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class ActionInvocation<TInstance> : Invocation
  {
    private readonly Action _action;

    public ActionInvocation (IInvocationContext context, Action action)
      : base(context)
    {
      _action = action;
    }

    public override void Proceed ()
    {
      _action();
    }
  }

  public class ActionInvocation<TInstance, TA0> : Invocation
  {
    private readonly Action<TA0> _action;

    public ActionInvocation (IInvocationContext context, Action<TA0> action)
      : base (context)
    {
      _action = action;
    }

    public override void Proceed ()
    {
      var context = (ActionInvocationContext<TInstance, TA0>) Context;
      _action (context.Arg0);
    }
  }

  public class ActionInvocation<TInstance, TA0, TA1> : Invocation
  {
    private readonly Action<TA0, TA1> _action;

    public ActionInvocation (IInvocationContext context, Action<TA0, TA1> action)
      : base (context)
    {
      _action = action;
    }

    public override void Proceed ()
    {
      var context = (ActionInvocationContext<TInstance, TA0, TA1>) Context;
      _action (context.Arg0, context.Arg1);
    }
  }
}