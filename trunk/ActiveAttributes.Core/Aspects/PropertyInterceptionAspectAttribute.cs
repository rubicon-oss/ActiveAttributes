using System;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Aspects
{
  [AttributeUsage (AttributeTargets.Property)]
  public abstract class PropertyInterceptionAspectAttribute : AspectAttribute
  {
    public abstract void OnInterceptGet (IInvocation invocation);
    public abstract void OnInterceptSet (IInvocation invocation);
  }
}