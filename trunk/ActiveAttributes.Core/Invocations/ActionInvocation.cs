using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class ActionInvocation<TInstance> : Invocation
  {
    private readonly ActionInvocationContext<TInstance> _context;
    private readonly Action _action;

    public ActionInvocation (ActionInvocationContext<TInstance> context, Action action)
    {
      _context = context;
      _action = action;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      _action();
    }
  }

  public class ActionInvocation<TInstance, TA0> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA0> _context;
    private readonly Action<TA0> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA0> context, Action<TA0> action)
    {
      _context = context;
      _action = action;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      var context = (ActionInvocationContext<TInstance, TA0>) Context;
      _action (context.Arg0);
    }
  }

  public class ActionInvocation<TInstance, TA0, TA1> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA0, TA1> _context;
    private readonly Action<TA0, TA1> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA0, TA1> context, Action<TA0, TA1> action)
    {
      _context = context;
      _action = action;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      var context = (ActionInvocationContext<TInstance, TA0, TA1>) Context;
      _action (context.Arg0, context.Arg1);
    }
  }
}