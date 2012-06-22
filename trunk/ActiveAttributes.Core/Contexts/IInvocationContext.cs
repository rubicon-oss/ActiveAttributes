using System;
using System.Reflection;
using ActiveAttributes.Core.Contexts.ArgumentCollection;

namespace ActiveAttributes.Core.Contexts
{
  public interface IInvocationContext
  {
    MethodInfo MethodInfo { get; }
    object Instance { get; }
    IArgumentCollection Arguments { get; }
    object ReturnValue { get; set; }
  }
}