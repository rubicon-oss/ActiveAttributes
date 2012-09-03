//Sample license text.
using System;
using System.Collections.Generic;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.UseCases.Aspects
{

  public class CacheAspectAttribute : MethodInterceptionAspectAttribute
  {
    private DateTime _lastTime;

    public CacheAspectAttribute ()
    {
    }

    public override void OnIntercept (IInvocation invocation)
    {
      var diff = DateTime.Now - _lastTime;
      if (diff >= TimeSpan.FromSeconds (5))
        _lastTime = DateTime.Now;

      invocation.Context.ReturnValue = _lastTime;
    }
  }
}