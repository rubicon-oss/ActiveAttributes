using System;
using System.Reflection;
using ActiveAttributes.Core.Contexts.ArgumentCollection;

namespace ActiveAttributes.Core.Contexts
{
  public interface IReadOnlyInvocationContext
  {
    MethodInfo MethodInfo { get; }
    object Instance { get; }
    IReadOnlyArgumentCollection Arguments { get; }
    object ReturnValue { get; }
  }
}