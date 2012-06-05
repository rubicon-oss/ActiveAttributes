using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace ActiveAttributes.Core
{
  public class Invocation
  {
    private Delegate _delegate;

    public Invocation (Delegate @delegate, params object[] arguments)
    {
      _delegate = @delegate;
      Arguments = arguments;
    }

    public void Proceed ()
    {
      ReturnValue = _delegate.DynamicInvoke (Arguments);
    }

    public object[] Arguments { get; private set; }
    public object ReturnValue { get; set; }

    // FROM CASTLE WINDSOR
    //public object Instance { get; private set; }
    //public MethodInfo Method { get; private set; }
    //public Type TargetType { get; private set; }
  }
}