//Sample license text.

using System;
using System.Collections.Generic;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.UseCases.Aspects
{

  public class CacheAspectAttribute : MethodInterceptionAspectAttribute
  {
    private readonly IDictionary<DateTime, DateTime> _cache;

    public CacheAspectAttribute ()
    {
      _cache = new Dictionary<DateTime, DateTime>();
    }

    public override void OnIntercept (IInvocation invocation)
    {
      var key = (DateTime) invocation.Context.Arguments[0];
      var value = default (DateTime);

      if (!_cache.TryGetValue (key, out value))
      {
        invocation.Proceed ();
        value = (DateTime) invocation.Context.ReturnValue;
        _cache.Add (key, value);
      }
      
      invocation.Context.ReturnValue = value;
    }
  }
}