using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ActiveAttributes.Core.Contexts
{
  public interface IReadOnlyInvocationContext
  {
    MethodInfo MethodInfo { get; }
    object Instance { get; }
    ReadOnlyCollection<object> Arguments { get; }
    object ReturnValue { get; }
  }
}