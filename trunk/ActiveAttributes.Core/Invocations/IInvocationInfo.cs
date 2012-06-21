using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ActiveAttributes.Core.Invocations
{
  /// <summary>
  /// Provides basic information for invocations.
  /// </summary>
  public interface IInvocationInfo
  {
    MethodInfo MethodInfo { get; }

    object Instance { get; }

    ReadOnlyCollection<object> Arguments { get; }
  }

  public interface IInvocationInfo<out TInstance> : IInvocationInfo
  {
    new TInstance Instance { get; }
  }

  public interface IInvocationInfo<out TInstance, out TResult> : IInvocationInfo<TInstance>
  {
    new TResult ReturnValue { get; }
  }
}