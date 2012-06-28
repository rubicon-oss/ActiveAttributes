using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public abstract class Invocation : IInvocation, IReadOnlyInvocation
  {
    public abstract IInvocationContext Context { get; }

    IReadOnlyInvocationContext IReadOnlyInvocation.Context
    {
      get { return (IReadOnlyInvocationContext) Context; }
    }

    public abstract void Proceed ();
  }
}