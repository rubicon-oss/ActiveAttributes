using System;
using System.Collections.ObjectModel;
using System.Reflection;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Contexts
{
  public struct FuncInvocationContext<TInstance, TA0, TR> : IFuncInvocationContext<TInstance, TA0, TR>
  {
    public FuncInvocationContext (MethodInfo methodInfo, TInstance instance, TA0 arg0, TR returnValue)
        : this()
    {
      MethodInfo = methodInfo;
      Instance = instance;
      Arg0 = arg0;
      ReturnValue = returnValue;
    }

    public MethodInfo MethodInfo { get; private set; }
    public TInstance Instance { get; private set; }
    public TA0 Arg0 { get; set; }
    public TR ReturnValue { get; set; }

    object IReadOnlyInvocationContext.Instance { get { return Instance; }
    }

    ReadOnlyCollection<object> IReadOnlyInvocationContext.Arguments
    {
      get { return new ReadOnlyCollection<object> (((IInvocationContext)this).Arguments); } // ????
    }

    object IReadOnlyInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
    }

    object[] IInvocationContext.Arguments
    {
      get { return new object[] { Arg0 }; }
    }

    object IInvocationContext.ReturnValue
    {
      get { return ReturnValue; }
      set { ReturnValue = (TR) value; }
    }
  }
}