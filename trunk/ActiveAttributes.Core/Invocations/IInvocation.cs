using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public interface IInvocation
  {
    IInvocationContext Context { get; }
    void Proceed ();
  }
}