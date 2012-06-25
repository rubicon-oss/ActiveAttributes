// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class OuterInvocation : Invocation
  {
    private readonly Action<IInvocation> _innerIntercepting;
    private readonly IInvocation _innerInvocation;

    public OuterInvocation (IInvocationContext context, Action<IInvocation> innerIntercepting, IInvocation innerInvocation)
      : base(context)
    {
      _innerIntercepting = innerIntercepting;
      _innerInvocation = innerInvocation;
    }

    public override void Proceed ()
    {
      _innerIntercepting (_innerInvocation);
    }
  }
}