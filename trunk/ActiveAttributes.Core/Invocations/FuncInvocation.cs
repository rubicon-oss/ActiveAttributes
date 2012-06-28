using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class FuncInvocation<TInstance, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TR> _context;
    private readonly Func<TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TR> context, Func<TR> func)
    {
      _context = context;
      _func = func;
    }


    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      _context.ReturnValue = _func ();
    }
  }

  public class FuncInvocation<TInstance, TA0, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA0, TR> _context;
    private readonly Func<TA0, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA0, TR> context, Func<TA0, TR> func)
    {
      _context = context;
      _func = func;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      _context.ReturnValue = _func (_context.Arg0);
    }
  }
}