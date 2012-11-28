using System;
using System.Runtime.Serialization;
using System.Threading;
using NUnit.Framework;
using PostSharp.Aspects;
using PostSharp.Aspects.Advices;

namespace PostSharp_
{
  [TestFixture]
  public class EventAspectTest
  {
    [Test]
    public void name ()
    {
      var obj = new DomainType ();

      //PostSharp.Aspects.OnMethodBoundaryAspect

      obj.Bla += ObjOnBla;
      obj.Bla += ObjOnBla;
      obj.Bla += ObjOnBla2;
      obj.Bla -= ObjOnBla;
      obj.Bla -= ObjOnBla;
      obj.Bla -= ObjOnBla2;
      obj.method();
      //Assert.That (() => obj.Bla += (sender, args) => {}, Throws.Exception);
    }

    private void ObjOnBla2 (object sender, EventArgs eventArgs)
    {
      Console.WriteLine ("kuh");
    }

    private void ObjOnBla (object sender, EventArgs eventArgs)
    {
      Console.WriteLine ("muh");
    }

    public class DomainType
    {
      private EventHandler _bla;

      [EventAspect]
      public virtual event EventHandler Bla
      {
        [MethodIntercept]
        add
        {
          _bla += value;
        }
        remove
        {
          _bla -= value;
        }
      }

      public void method()
      {
        _bla (null, null);
      }
    }


    [Serializable]
    public class EventAspect : EventInterceptionAspect, IInstanceScopedAspect
    {

      public override void OnAddHandler (EventInterceptionArgs args)
      {
        PostSharp.Aspects.LocationInterceptionArgs x;
        args.ProceedAddHandler();
      }

      public object CreateInstance (AdviceArgs adviceArgs)
      {
        return this;
      }

      public void RuntimeInitializeInstance ()
      {
      }
    }

    [Serializable]
    public class MethodIntercept : MethodInterceptionAspect
    {
      public override void OnInvoke (MethodInterceptionArgs args)
      {
        base.OnInvoke (args);
      }
    }
  }
}