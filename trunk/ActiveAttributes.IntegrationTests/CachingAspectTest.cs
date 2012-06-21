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
    private DomainType _instance;

    [Test]
    public void SetUp ()
    {
      _instance = ObjectFactory.Create<DomainType>();
    }

    [Test]
    public void Cache ()
    {
      var input = "a";

      var result1 = _instance.Method (input);
      var result2 = _instance.Method (input);

      Assert.AreEqual (_instance.MethodExecutionCounter, 1);
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
      private string _key;
      private string _value;

      public override void OnIntercept (Invocation invocation)
      {
        if (_key != (string) invocation.Arguments[0])
        {
          invocation.Proceed();
          _key = (string) invocation.Arguments[0];
          _value = (string) invocation.ReturnValue;
        }
        else
          invocation.ReturnValue = _value;
      }
    }
  }
}