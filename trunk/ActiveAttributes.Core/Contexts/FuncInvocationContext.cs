using System;
using System.Reflection;
using ActiveAttributes.Core.Contexts.ArgumentCollection;

namespace ActiveAttributes.Core.Contexts
{
  public class FuncInvocationContext<TInstance, TR> : IInvocationContext, IReadOnlyInvocationContext
  {
    private EmptyArgumentCollection _argumentCollection;

    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance)
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

    public TR ReturnValue { get; set; }

    object IReadOnlyInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
    }

    object IInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
      set { ReturnValue = (TR) value; }
    }
  }

  public class FuncInvocationContext<TInstance, TA0, TR> : IInvocationContext, IReadOnlyInvocationContext
  {
    private FuncArgumentCollection<TInstance, TA0, TR> _argumentCollection;

    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0)
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
      get { return _argumentCollection ?? (_argumentCollection = new FuncArgumentCollection<TInstance, TA0, TR> (this)); }
    }

    IArgumentCollection IInvocationContext.Arguments
    {
      get { return _argumentCollection ?? (_argumentCollection = new FuncArgumentCollection<TInstance, TA0, TR> (this)); }
    }

    public TR ReturnValue { get; set; }

    object IReadOnlyInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
    }

    object IInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
      set { ReturnValue = (TR) value; }
    }
  }
}