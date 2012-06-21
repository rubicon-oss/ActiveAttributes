using System;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Contexts
{
  public interface IFuncInvocationContextBase<out TInstance, TR> : ITypedInvocationContext<TInstance>
  {
    new TR ReturnValue { get; set; }
  }

  public interface IFuncInvocationContext<out TInstance, TA0, TR> : IFuncInvocationContextBase<TInstance, TR>
  {
    TA0 Arg0 { get; set; }
  }
}