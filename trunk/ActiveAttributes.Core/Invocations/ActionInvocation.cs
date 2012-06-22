using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class ActionInvocation<TInstance> : Invocation
  {
    public ActionInvocation (ActionInvocationContext<TInstance> context, Action action)
      : base (() => action ())
    {
      Context = context;
    }

    public ActionInvocation (ActionInvocationContext<TInstance> context, Action<IInvocation> innerProceed, IInvocation innerInvocation)
      : base (() => innerProceed (innerInvocation))
    {
      Context = context;
    }

    protected override IInvocationContext GetInvocationContext ()
    {
      return Context;
    }

    public ActionInvocationContext<TInstance> Context { get; private set; }
  }

  public class ActionInvocation<TInstance, TA0> : Invocation
  {
    public ActionInvocation (ActionInvocationContext<TInstance, TA0> context, Action action)
      : base (() => action ())
    {
      Context = context;
    }

    public ActionInvocation (ActionInvocationContext<TInstance, TA0> context, Action<IInvocation> innerProceed, IInvocation innerInvocation)
      : base (() => innerProceed (innerInvocation))
    {
      Context = context;
    }

    protected override IInvocationContext GetInvocationContext ()
    {
      return Context;
    }

    public ActionInvocationContext<TInstance, TA0> Context { get; private set; }
  }
}