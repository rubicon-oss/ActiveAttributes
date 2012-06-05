using System;

namespace ActiveAttributes.Core
{
  public class Invocation
  {
    private readonly Delegate _delegate;

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
  }
}