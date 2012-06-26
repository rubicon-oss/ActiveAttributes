using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class FuncInvocation<TInstance, TR> : Invocation
  {
    private readonly Func<TR> _func;

    public FuncInvocation (IInvocationContext context, Func<TR> func)
      : base (context)
    {
      _func = func;
    }

    public override void Proceed ()
    {
      var context = (FuncInvocationContext<TInstance, TR>) Context;
      context.ReturnValue = _func ();
    }
  }

  public class FuncInvocation<TInstance, TA0, TR> : Invocation
  {
    private readonly Func<TA0, TR> _func;

    public FuncInvocation (IInvocationContext context, Func<TA0, TR> func)
        : base(context)
    {
      _func = func;
    }

    public override void Proceed ()
    {
      var context = (FuncInvocationContext<TInstance, TA0, TR>) Context;
      context.ReturnValue = _func (context.Arg0);
    }
  }
}