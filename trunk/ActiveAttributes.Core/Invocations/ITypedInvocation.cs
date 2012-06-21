using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public interface ITypedInvocation<out TInstance> : IInvocation
  {
    new ITypedInvocationContext<TInstance> Context { get; }
  }
}