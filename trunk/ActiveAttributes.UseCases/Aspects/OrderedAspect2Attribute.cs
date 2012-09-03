//Sample license text.
using System;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.UseCases.Aspects
{
  public class OrderedAspect2Attribute : MethodInterceptionAspectAttribute
  {
    public override void OnIntercept (IInvocation invocation)
    {
      Console.WriteLine ("{0}", this);
      invocation.Proceed ();
    }
  }
}