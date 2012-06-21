using System;
using System.Reflection;

namespace ActiveAttributes.Core.Invocations
{
  public class ActionInvocation : Invocation
  {
    private readonly Action _action;

    public ActionInvocation (IInvocation innerInvocation, MethodInfo methodInfo, object instance, Action action)
        : base (innerInvocation, methodInfo, instance)
    {
      _action = action;
    }

    public override void Proceed ()
    {
      if (InnerInvocation == null)
        _action();
      else
        InnerInvocation.Proceed();
    }
  }

  public class ActionInvocation<TInstance> : Invocation<TInstance>
  {
    private readonly Action _action;

    public ActionInvocation (IInvocation innerInvocation, MethodInfo methodInfo, TInstance instance, Action action)
        : base (innerInvocation, methodInfo, instance)
    {
      _action = action;
    }

    public override void Proceed ()
    {
      if (InnerInvocation == null)
        _action();
      else
        InnerInvocation.Proceed();
    }
  }

  public class ActionInvocation<TInstance, TA0> : Invocation<TInstance>
  {
    private readonly Action<TA0> _action;

    public ActionInvocation (IInvocation innerInvocation, MethodInfo methodInfo, TInstance instance, Action<TA0> action, TA0 arg0)
        : base (innerInvocation, methodInfo, instance)
    {
      _action = action;
      Arg0 = arg0;
    }

    public TA0 Arg0 { get; set; }

    public override void Proceed ()
    {
      if (InnerInvocation == null)
        _action (Arg0);
      else
        InnerInvocation.Proceed();
    }
  }
}