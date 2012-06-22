using System;
using System.Collections.ObjectModel;
using System.Reflection;
using ActiveAttributes.Core.Contexts.ArgumentCollection;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Contexts
{
  public class ActionInvocationContext<TInstance> : IInvocationContext, IReadOnlyInvocationContext
  {
    private EmptyArgumentCollection _argumentCollection;

    public ActionInvocationContext (MethodInfo methodInfo, TInstance instance)
    {
      MethodInfo = methodInfo;
      Instance = instance;
    }

    public MethodInfo MethodInfo { get; private set; }

    public TInstance Instance { get; private set; }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    object IInvocationContext.Instance
    {
      get { return Instance; }
    }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new EmptyArgumentCollection()); }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new EmptyArgumentCollection()); }
    }

    public object ReturnValue { get; set; }
  }

  public class ActionInvocationContext<TInstance, TA0> : IInvocationContext, IReadOnlyInvocationContext
  {
    private ActionArgumentCollection<TInstance, TA0> _argumentCollection;

    public ActionInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0)
    {
      MethodInfo = methodInfo;
      Instance = instance;
      Arg0 = arg0;
    }

    public MethodInfo MethodInfo { get; private set; }

    public TInstance Instance { get; private set; }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    object IInvocationContext.Instance
    {
      get { return Instance; }
    }

    public TA0 Arg0 { get; set; }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new ActionArgumentCollection<TInstance, TA0> (this)); }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new ActionArgumentCollection<TInstance, TA0> (this)); }
    }

    public object ReturnValue { get; set; }
  }
}