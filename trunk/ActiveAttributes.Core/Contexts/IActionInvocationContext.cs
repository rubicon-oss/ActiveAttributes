using System;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Contexts
{
  public interface IActionInvocationContext<out TInstance> : ITypedInvocationContext<TInstance>
  {
  }

  public interface IActionInvocationContext<out TInstance, TA0> : ITypedInvocationContext<TInstance>
  {
    TA0 Arg0 { get; set; }
  }
}