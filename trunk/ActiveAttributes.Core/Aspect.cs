using System;

namespace ActiveAttributes.Core
{
  [AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
  public class Aspect : Attribute
  {
    public AspectScope Scope { get; set; }

    public virtual void OnInvoke (Invocation invocation)
    {
      invocation.Proceed ();
    }
  }
}