using System;
using System.Reflection;

namespace ActiveAttributes.Core
{
  public class Invocation
  {
    private readonly Invocation _innerInvocation;
    private readonly Delegate _delegate;

    public Invocation (Delegate @delegate, MethodInfo method, object instance, params object[] arguments)
    {
      _delegate = @delegate;
      Instance = instance;
      Method = method;
      Arguments = arguments;
    }

    public Invocation (Invocation innerInvocation)
    {
      _innerInvocation = innerInvocation;
      _delegate = new Action (innerInvocation.Proceed);
      Instance = innerInvocation.Instance;
      Method = innerInvocation.Method;
      Arguments = innerInvocation.Arguments;
    }


    public void Proceed ()
    {
      ReturnValue = _delegate.DynamicInvoke (_innerInvocation == null ? Arguments : new object[0]);

      if (_innerInvocation != null)
        ReturnValue = _innerInvocation.ReturnValue;
    }

    public object[] Arguments { get; private set; }
    public object ReturnValue { get; set; }

    public MethodInfo Method { get; private set; }
    public object Instance { get; private set; }

    public object Tag { get; set; }

    public FlowBehavior FlowBehavior { get; set; }

    public Exception Exception { get; set; }

    // FROM CASTLE WINDSOR
    //public Type TargetType { get; private set; }
  }
}