using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public abstract class Invocation : IInvocation
  {
    private readonly IInvocationContext _context;
    private readonly Action _proceed;

    protected Invocation (IInvocationContext context, Action proceed)
    {
      _context = context;
      _proceed = proceed;
    }

    IReadOnlyInvocationContext IReadOnlyInvocation.Context
    {
      get { return _context; }
    }

    public IInvocationContext Context
    {
      get { return _context; }
    }

    public void Proceed ()
    {
      _proceed();
    }

    public override string ToString ()
    {
      return string.Format ("Context: {0}, Proceed: {1}", _context, _proceed);
    }
  }
}