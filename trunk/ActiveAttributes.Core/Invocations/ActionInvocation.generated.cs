using System;
using ActiveAttributes.Core.Contexts;
using Remotion;

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
      _action ();
    }
  }
  public class ActionInvocation<TInstance, TA1, TA2> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA1, TA2> _context;
    private readonly Action<TA1, TA2> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA1, TA2> context, Action<TA1, TA2> action)
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
      _action (_context.Arg1, _context.Arg2);
    }
  }
  public class ActionInvocation<TInstance, TA1, TA2, TA3> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA1, TA2, TA3> _context;
    private readonly Action<TA1, TA2, TA3> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA1, TA2, TA3> context, Action<TA1, TA2, TA3> action)
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
      _action (_context.Arg1, _context.Arg2, _context.Arg3);
    }
  }
  public class ActionInvocation<TInstance, TA1, TA2, TA3, TA4> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4> _context;
    private readonly Action<TA1, TA2, TA3, TA4> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4> context, Action<TA1, TA2, TA3, TA4> action)
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
      _action (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4);
    }
  }
  public class ActionInvocation<TInstance, TA1, TA2, TA3, TA4, TA5> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5> _context;
    private readonly Action<TA1, TA2, TA3, TA4, TA5> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5> context, Action<TA1, TA2, TA3, TA4, TA5> action)
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
      _action (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4, _context.Arg5);
    }
  }
  public class ActionInvocation<TInstance, TA1, TA2, TA3, TA4, TA5, TA6> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6> _context;
    private readonly Action<TA1, TA2, TA3, TA4, TA5, TA6> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6> context, Action<TA1, TA2, TA3, TA4, TA5, TA6> action)
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
      _action (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4, _context.Arg5, _context.Arg6);
    }
  }
  public class ActionInvocation<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7> _context;
    private readonly Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7> context, Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7> action)
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
      _action (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4, _context.Arg5, _context.Arg6, _context.Arg7);
    }
  }
  public class ActionInvocation<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> _context;
    private readonly Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> context, Action<TA1, TA2, TA3, TA4, TA5, TA6, TA7, TA8> action)
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
      _action (_context.Arg1, _context.Arg2, _context.Arg3, _context.Arg4, _context.Arg5, _context.Arg6, _context.Arg7, _context.Arg8);
    }
  }
}
