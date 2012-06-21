using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class FuncInvocation<TInstance, TA0, TR> : TypedInvocation<TInstance>
  {
    public FuncInvocation (IFuncInvocationContext<TInstance, TA0, TR> context, Func<TA0, TR> func)
        : base(context, () => context.ReturnValue = func (context.Arg0))
    {
    }

    public FuncInvocation (IFuncInvocationContext<TInstance, TA0, TR> context, Action<IInvocation> innerProceed, IInvocation innerInvocation)
        : base (context, () => innerProceed (innerInvocation))
    {
    }

    public new IFuncInvocationContext<TInstance, TA0, TR> Context
    {
      get { return (IFuncInvocationContext<TInstance, TA0, TR>) base.Context; }
    }
  }
}