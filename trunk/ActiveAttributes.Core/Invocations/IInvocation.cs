using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public interface IInvocation : IReadOnlyInvocation
  {
    new IInvocationContext Context { get; }
    void Proceed ();
  }
}