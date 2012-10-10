using System;
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.Common.UnitTests
{
  [TestFixture]
  public class CatchExceptionAspectAttributeTest
  {
    [Test]
    public void CatchesException ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      Assert.That (() => instance.CatchableThrow1 (), Throws.Nothing);
      Assert.That (() => instance.CatchableThrow2 (), Throws.Nothing);
    }

    [Test]
    public void RethrowsException ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      Assert.That (() => instance.UncatchableThrow (), Throws.Exception);
    }

    [Test]
    public void Proceeds ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      instance.Method();

      Assert.That (instance.MethodExecuted, Is.True);
    }

    public class DomainType
    {
      [CatchExceptionAspect]
      public virtual void CatchableThrow1 ()
      {
        throw new Exception();
      }

      [CatchExceptionAspect(typeof(Exception))]
      public virtual void CatchableThrow2 ()
      {
        throw new InvalidOperationException ();
      }

      [CatchExceptionAspect(typeof(InvalidOperationException))]
      public virtual void UncatchableThrow ()
      {
        throw new Exception ();
      }

      public bool MethodExecuted { get; private set; }

      [CatchExceptionAspect]
      public virtual void Method ()
      {
        MethodExecuted = true;
      }
    }
  }
}