using System;
using ActiveAttributes.Core.Contexts;
using Remotion;

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
  public class FuncInvocation<TInstance, TA1, TA2, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA1, TA2, TR> _context;
    private readonly Func<TA1, TA2, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA1, TA2, TR> context, Func<TA1, TA2, TR> func)
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
      _context.ReturnValue = _func (_context.Arg1, _context.Arg2);
    }
  }
  public class FuncInvocation<TInstance, TA1, TA2, TA3, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA1, TA2, TA3, TR> _context;
    private readonly Func<TA1, TA2, TA3, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA1, TA2, TA3, TR> context, Func<TA1, TA2, TA3, TR> func)
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
      _context.ReturnValue = _func (_context.Arg1, _context.Arg2, _context.Arg3);
    }
  }
  public class FuncInvocation<TInstance, TA1, TA2, TA3, TA4, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TR> _context;
    private readonly Func<TA1, TA2, TA3, TA4, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TR> context, Func<TA1, TA2, TA3, TA4, TR> func)
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
      _context.ReturnValue = _func (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4);
    }
  }
  public class FuncInvocation<TInstance, TA1, TA2, TA3, TA4, TA5, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TR> _context;
    private readonly Func<TA1, TA2, TA3, TA4, TA5, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TR> context, Func<TA1, TA2, TA3, TA4, TA5, TR> func)
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
      _context.ReturnValue = _func (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4, _context.Arg5);
    }
  }
  public class FuncInvocation<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TR> _context;
    private readonly Func<TA1, TA2, TA3, TA4, TA5, TA6, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TR> context, Func<TA1, TA2, TA3, TA4, TA5, TA6, TR> func)
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
      _context.ReturnValue = _func (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4, _context.Arg5, _context.Arg6);
    }
  }
  public class FuncInvocation<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TR> _context;
    private readonly Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TR> context, Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TR> func)
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
      _context.ReturnValue = _func (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4, _context.Arg5, _context.Arg6, _context.Arg7);
    }
  }
  public class FuncInvocation<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TR> _context;
    private readonly Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TR> context, Func<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8, TR> func)
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
      _context.ReturnValue = _func (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4, _context.Arg5, _context.Arg6, _context.Arg7, _context.Arg8);
    }
  }
}
