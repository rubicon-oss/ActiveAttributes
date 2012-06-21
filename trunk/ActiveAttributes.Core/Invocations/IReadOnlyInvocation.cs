using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public interface IReadOnlyInvocation
  {
    IReadOnlyInvocationContext Context { get; }
  }
}