using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ActiveAttributes.Core.Invocation
{
  public abstract class Invocation : IInvocation, IInvocationInfo
  {
    protected readonly IInvocation InnerInvocation;

    protected Invocation (IInvocation innerInvocation, MethodInfo methodInfo, object instance, params object[] arguments)
    {
      MethodInfo = methodInfo;
      InnerInvocation = innerInvocation;
      Instance = instance;
      Arguments = arguments;
    }

    public object[] Arguments { get; private set; }

    #region IInvocation Members

    public MethodInfo MethodInfo { get; private set; }
    public object Instance { get; private set; }

    // virtual ?
    public object ReturnValue { get; protected set; }

    ReadOnlyCollection<object> IInvocationInfo.Arguments
    {
      get { return new ReadOnlyCollection<object> (Arguments); }
    }

    public abstract void Proceed ();

    #endregion
  }

  public abstract class Invocation<TInstance> : Invocation, IInvocationInfo<TInstance>
  {
    protected Invocation (IInvocation innerInvocation, MethodInfo methodInfo, TInstance instance)
        : base (innerInvocation, methodInfo, instance) {}

    #region IInvocationInfo<TInstance> Members

    public new TInstance Instance
    {
      get { return (TInstance) base.Instance; }
    }

    #endregion
  }
}