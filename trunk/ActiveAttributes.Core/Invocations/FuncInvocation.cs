using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class FuncInvocation<TInstance, TA0, TR> : Invocation
  {
    public FuncInvocation (FuncInvocationContext<TInstance, TA0, TR> context, Func<TA0, TR> func)
        : base(() => context.ReturnValue = func (context.Arg0))
    {
      Context = context;
    }

    public FuncInvocation (FuncInvocationContext<TInstance, TA0, TR> context, Action<IInvocation> innerProceed, IInvocation innerInvocation)
        : base (() => innerProceed (innerInvocation))
    {
      Context = context;
    }

    protected override IInvocationContext GetInvocationContext ()
    {
      return Context;
    }

    public FuncInvocationContext<TInstance, TA0, TR> Context { get; private set; }
  }
}