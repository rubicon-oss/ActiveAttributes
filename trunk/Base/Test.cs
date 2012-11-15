using System;
using System.Threading;
using LinFu.AOP.Interfaces;
using NUnit.Framework;

namespace Base
{
  [TestFixture]
  public class Test
  {
    [Test]
    public void Run ()
    {
      var obj = CreateObject();

      for (var i = 0; i < 10; ++i)
      {
        obj.Method();
      }
    }

    private static DomainType CreateObject ()
    {
      var obj = new DomainType ();
      var modified = obj as IModifiableType;

      if (modified != null)
      {
        var provider = new SimpleAroundInvokeProvider (new AroundSpeakMethod (),
            c => c.TargetMethod.Name == "Method");

        modified.IsInterceptionEnabled = true;
        modified.AroundInvokeProvider = provider;
      }
      return obj;
    }

    public class DomainType
    {
      public virtual void Method ()
      {
        Thread.Sleep (10);
      }
    }

    public class AroundSpeakMethod : IAroundInvoke
    {
      public void AfterInvoke (IInvocationContext context, object returnValue)
      {
        Thread.Sleep (10);
      }

      public void BeforeInvoke (IInvocationContext context)
      {
        context.Arguments[0] = "advice";
        Thread.Sleep (10);
      }
    }

  }
}