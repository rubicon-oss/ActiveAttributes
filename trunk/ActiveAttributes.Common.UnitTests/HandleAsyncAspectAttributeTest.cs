using System;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.Common.UnitTests
{
  [TestFixture]
  public class HandleAsyncAspectAttributeTest
  {
    [Test]
    public void ExecutesAsync ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      instance.Method();

      Assert.That (instance.Flag, Is.False);
    }

    public class DomainType
    {
      public bool Flag { get; set; }

      [HandleAsyncAspect]
      public virtual void Method ()
      {
        Thread.Sleep (10);
        Flag = true;
      }
    }
  }
}