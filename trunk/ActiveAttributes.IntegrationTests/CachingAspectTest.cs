using System;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Contexts;
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

      // TODO: Move to utility
      //var assemblyFileName = instance.GetType ().Module.ScopeName;
      //((AssemblyBuilder) instance.GetType().Assembly).Save (assemblyFileName);
    }

    // TODO: Remove
    //[Test]
    //public void name ()
    //{
    //  var method = GetType().GetMethod ("Foo");
    //  Func<CachingAspectTest, int, int> action = (Func<CachingAspectTest, int, int>) Delegate.CreateDelegate (typeof (Func<CachingAspectTest, int, int>), method);
      
    //  var result = action (this, 13);
    //  Assert.That (result, Is.EqualTo (30));
    //}

    //public int Foo (int i)
    //{
    //  return 17 + i;
    //}

    public class DomainType
    {
      public int MethodExecutionCounter { get; private set; }

      [CacheAspect]
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
        var context = invocation.Context;
        if (_key != context.Arguments[0])
        {
          invocation.Proceed();
          _key = context.Arguments[0];
          _value = context.ReturnValue;
        }
        else
          context.ReturnValue = _value;
      }
    }
  }
}