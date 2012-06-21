using System;

namespace ActiveAttributes.Core.Invocations
{
  public interface IInvocation : IInvocationInfo
  {
    void Proceed ();
  }
}