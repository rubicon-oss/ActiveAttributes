using System;
using NUnit.Framework;
using PostSharp.Aspects;
using PostSharp.Extensibility;

[assembly: PostSharp_.Aspect (AttributeTargetTypes = "PostSharp.DomainType", AttributeTargetElements = MulticastTargets.Method)]

namespace PostSharp_
{
  [TestFixture]
  public class Test
  {
    [Test]
    public void name ()
    {
      var obj = new DomainType ();
      var obj1 = new DomainType ();

      for (var i = 0; i < 10; ++i)
      {
        obj.Method ();
        obj1.Method ();
      }
    }
  }

  [Serializable]
  public class Aspect : MethodInterceptionAspect
  {
    private int _counter;

    public override void OnInvoke (MethodInterceptionArgs args)
    {
      Console.WriteLine (_counter++);
      //Thread.Sleep (10);
      args.Proceed ();
      //Thread.Sleep (10);
    }
  }

  [Aspect (AttributeTargetTypes = "PostSharp.DomainType", AttributeTargetElements = MulticastTargets.Method)]
  public class DomainType
  {
    public void Method ()
    {
      //Thread.Sleep (10);
    }
  }
}