using System;
using ActiveAttributes.Core.Invocation;

namespace ActiveAttributes.Core.Aspects
{
  [AttributeUsage (AttributeTargets.Property)]
  public abstract class PropertyInterceptionAspectAttribute : AspectAttribute
  {
    public abstract void InterceptGet (IInvocation invocation);
    public abstract void InterceptSet (IInvocation invocation);
  }
}