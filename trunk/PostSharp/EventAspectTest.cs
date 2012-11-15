using System;
using System.Runtime.Serialization;
using System.Threading;
using NUnit.Framework;
using PostSharp.Aspects;

namespace PostSharp_
{
  [TestFixture]
  public class EventAspectTest
  {
    [Test]
    public void name ()
    {
      new DomainType ();
      var obj = (DomainType) FormatterServices.GetUninitializedObject (typeof (DomainType));

      Assert.That (() => obj.Bla += () => { }, Throws.Exception);
    }
    
    public class DomainType
    {

      [EventAspect]
      public virtual event ThreadStart Bla;

      public void method()
      {
        Bla();
      }
    }

    [Serializable]
    public class EventAspect : EventInterceptionAspect, IInstanceScopedAspect
    {

      public override void OnAddHandler (EventInterceptionArgs args)
      {
        throw new Exception();
      }

      public object CreateInstance (AdviceArgs adviceArgs)
      {
        return this;
      }

      public void RuntimeInitializeInstance ()
      {
      }
    }
  }
}