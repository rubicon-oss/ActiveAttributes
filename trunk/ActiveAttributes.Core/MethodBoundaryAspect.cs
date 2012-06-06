// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;

namespace ActiveAttributes.Core
{
  public class MethodBoundaryAspect : Aspect
  {
    public override void OnInvoke (Invocation invocation)
    {
      OnEntry (invocation);

      try
      {
        base.OnInvoke (invocation);
        OnSuccess (invocation);
      }
      catch (Exception)
      {
        OnException (invocation);
      }
      finally
      {
        OnExit (invocation);
      }
    }

    public virtual void OnEntry (Invocation invocation) { }
    public virtual void OnExit (Invocation invocation) { }
    public virtual void OnException (Invocation invocation) { }
    public virtual void OnSuccess (Invocation invocation) { }
  }
}