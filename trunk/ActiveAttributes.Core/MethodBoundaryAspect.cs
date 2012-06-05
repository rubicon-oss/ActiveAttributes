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
      OnEntry();

      try
      {
        base.OnInvoke (invocation);
        OnSuccess ();
      }
      catch (Exception)
      {
        OnException ();
      }
      finally
      {
        OnExit();
      }
    }

    public virtual void OnEntry () { }
    public virtual void OnExit () { }
    public virtual void OnException () { }
    public virtual void OnSuccess () { }
  }
}