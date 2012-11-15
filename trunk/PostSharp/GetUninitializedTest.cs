using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using PostSharp.Aspects;

namespace PostSharp_
{
  [TestFixture]
  public class GetUninitializedTest
  {
    [Test]
    public void ThrowsNot ()
    {
      //var obj = new DomainType ();
      //var obj2 = new DomainType ();
      var obj = (DomainType) FormatterServices.GetUninitializedObject (typeof (DomainType));
      var obj2 = (DomainType) FormatterServices.GetUninitializedObject (typeof (DomainType));

      Assert.That (obj.Method(), Is.Not.EqualTo (obj2.Method()));
    }

    private class DomainType
    {
      [DomainAspect]
      public string Method ()
      {
        throw new Exception();
      } 
    }


    [Serializable]
    public class DomainAspect : MethodInterceptionAspect, IInstanceScopedAspect
    {
      private readonly string _guid = Guid.NewGuid().ToString();

      public override void OnInvoke (MethodInterceptionArgs args)
      {
        args.ReturnValue = _guid;
      }

      public object CreateInstance (AdviceArgs adviceArgs)
      {
        return new DomainAspect();
      }

      public void RuntimeInitializeInstance ()
      {

      }
    }
  }
}