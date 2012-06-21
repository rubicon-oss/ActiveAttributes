using System;

namespace ActiveAttributes.Core.Contexts
{
  public interface ITypedInvocationContext<out TInstance> : IInvocationContext
  {
    new TInstance Instance { get; }
  }
}