// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;

namespace ActiveAttributes.Core
{
  [AttributeUsage(AttributeTargets.Property)]
  public class PropertyInterceptionAspect : Aspect
  {
    public virtual void OnSet (Invocation invocation)
    {
      invocation.Proceed();
    }

    public virtual void OnGet (Invocation invocation)
    {
      invocation.Proceed();
    }
  }
}