using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class OuterInvocation : Invocation
  {
    private readonly IInvocationContext _context;
    private readonly Action<IInvocation> _innerIntercepting;
    private readonly IInvocation _innerInvocation;

    public OuterInvocation (IInvocationContext context, Action<IInvocation> innerIntercepting, IInvocation innerInvocation)
    {
      _context = context;
      _innerIntercepting = innerIntercepting;
      _innerInvocation = innerInvocation;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      _innerIntercepting (_innerInvocation);
    }
  }
}