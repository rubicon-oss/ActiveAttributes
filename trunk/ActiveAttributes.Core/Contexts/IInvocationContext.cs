using System;

namespace ActiveAttributes.Core.Contexts
{
  public interface IInvocationContext : IReadOnlyInvocationContext
  {
    new object[] Arguments { get; }
    new object ReturnValue { get; set; }
  }
}