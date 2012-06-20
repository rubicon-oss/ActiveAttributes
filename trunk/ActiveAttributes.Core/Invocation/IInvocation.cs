using System;

namespace ActiveAttributes.Core.Invocation
{
  public interface IInvocation : IInvocationInfo
  {
    void Proceed ();
  }
}