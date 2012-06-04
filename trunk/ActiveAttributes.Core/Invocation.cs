using System;

namespace ActiveAttributes.Core
{
  public class Invocation
  {
    private readonly Delegate _originalBody;

    internal Invocation (Delegate originalBody)
    {
      _originalBody = originalBody;
    }

    public void Invoke (params object[] args)
    {
      _originalBody.DynamicInvoke (args);
    }
  }
}