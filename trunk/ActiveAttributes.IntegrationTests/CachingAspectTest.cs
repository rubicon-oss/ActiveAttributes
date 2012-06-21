using System;

using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class CachingAspectTest
  {
    [Test]
    public void Cache ()
    {
      var input = "a";

      var instance = ObjectFactory.Create<DomainType>();
      var result1 = instance.Method (input);
      var result2 = instance.Method (input);

      Assert.AreEqual (instance.MethodExecutionCounter, 1);
      Assert.AreEqual (result1, result2);
    }

    public class DomainType
    {
      public int MethodExecutionCounter { get; private set; }

      [CacheAspect]
      public virtual string Method (string arg)
      {
        MethodExecutionCounter++;
        return arg + "_computed";
      }
    }

    public class CacheAspectAttribute : MethodInterceptionAspectAttribute
    {
      private object _key;
      private object _value;

      public override void OnIntercept (IInvocation invocation)
      {
        if (_key != invocation.Context.Arguments[0])
        {
          invocation.Proceed();
          _key = invocation.Context.Arguments[0];
          _value = invocation.Context.ReturnValue;
        }
        else
          invocation.Context.ReturnValue = _value;
      }
    }
  }
}