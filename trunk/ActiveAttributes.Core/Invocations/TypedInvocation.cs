using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public abstract class TypedInvocation<TInstance> : Invocation, ITypedInvocation<TInstance>
  {
    protected TypedInvocation (ITypedInvocationContext<TInstance> context, Action proceed)
        : base (context, proceed) { }

    public new ITypedInvocationContext<TInstance> Context
    {
      get { return (ITypedInvocationContext<TInstance>) base.Context; }
    }
  }
}