using System;
using System.Collections.ObjectModel;
using System.Reflection;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Contexts
{
  public struct ActionInvocationContext<TInstance> : IActionInvocationContext<TInstance>
  {
    public ActionInvocationContext (MethodInfo methodInfo, TInstance instance)
      : this ()
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

    ReadOnlyCollection<object> IReadOnlyInvocationContext.Arguments
    {
      get { return new ReadOnlyCollection<object> (((IInvocationContext) this).Arguments); } // ????
    }

    object IReadOnlyInvocationContext.ReturnValue
    {
      get { return null; }
    }

    object[] IInvocationContext.Arguments
    {
      get { return new object[0]; }
    }

    object IInvocationContext.ReturnValue
    {
      get { return null; }
      set { throw new NotSupportedException (); }
    }
  }

  public struct ActionInvocationContext<TInstance, TA0> : IActionInvocationContext<TInstance, TA0>
  {
    public ActionInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0)
        : this()
    {
      MethodInfo = methodInfo;
      Instance = instance;
      Arg0 = arg0;
    }

    public MethodInfo MethodInfo { get; private set; }
    public TInstance Instance { get; private set; }
    public TA0 Arg0 { get; set; }

    object IReadOnlyInvocationContext.Instance
    {
      get { return Instance; }
    }

    ReadOnlyCollection<object> IReadOnlyInvocationContext.Arguments
    {
      get { return new ReadOnlyCollection<object> (((IInvocationContext) this).Arguments); } // ????
    }

    object IReadOnlyInvocationContext.ReturnValue
    {
      get { return null; }
    }

    object[] IInvocationContext.Arguments
    {
      get { return new object[] { Arg0 }; }
    }

    object IInvocationContext.ReturnValue
    {
      get { return null; }
      set { throw new NotSupportedException(); }
    }
  }
}