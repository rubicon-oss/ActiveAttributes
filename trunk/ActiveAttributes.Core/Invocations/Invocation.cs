using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public abstract class Invocation : IInvocation, IReadOnlyInvocation
  {
    private readonly IInvocationContext _context;

    protected Invocation (IInvocationContext context)
    {
      _context = context;
    }

    // TODO: Make abstract, store context in derived classes
    public IInvocationContext Context
    {
      get { return _context; }
    }

    IReadOnlyInvocationContext IReadOnlyInvocation.Context
    {
      get { return (IReadOnlyInvocationContext) Context; }
    }

    public abstract void Proceed ();
  }
}