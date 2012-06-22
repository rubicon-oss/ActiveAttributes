using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public abstract class Invocation : IInvocation, IReadOnlyInvocation
  {
    private readonly Action _proceed;

    protected Invocation (Action proceed)
    {
      _proceed = proceed;
    }

    protected abstract IInvocationContext GetInvocationContext ();

    IInvocationContext IInvocation.Context
    {
      get { return GetInvocationContext(); }
    }

    IReadOnlyInvocationContext IReadOnlyInvocation.Context
    {
      get { return (IReadOnlyInvocationContext) GetInvocationContext(); }
    }

    public void Proceed ()
    {
      _proceed();
    }

    public override string ToString ()
    {
      return string.Format ("Context: {0}, Proceed: {1}", this.GetInvocationContext(), _proceed);
    }
  }
}