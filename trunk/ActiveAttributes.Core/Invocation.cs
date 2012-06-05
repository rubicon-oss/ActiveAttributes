using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace ActiveAttributes.Core
{
  public class Invocation
  {
    private Delegate _delegate;

    public Invocation (Delegate @delegate, MethodInfo method, object instance, params object[] arguments)
    {
      _delegate = @delegate;
      Instance = instance;
      Method = method;
      Arguments = arguments;
    }

    public void Proceed ()
    {
      ReturnValue = _delegate.DynamicInvoke (Arguments);
    }

    public object[] Arguments { get; private set; }
    public object ReturnValue { get; set; }

    public MethodInfo Method { get; private set; }
    public object Instance { get; private set; }

    // FROM CASTLE WINDSOR
    //public Type TargetType { get; private set; }
  }
}