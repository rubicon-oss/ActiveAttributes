using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class AspectTest
  {
    [Test]
    public void OnInvoke ()
    {
      var called = false;
      var invocation = new Invocation (new Action (() => { called = true; }));
      var aspect = new TestableAspect();

      aspect.OnInvoke (invocation);

      Assert.That (called, Is.True);
    }

    [Test]
    public void Clone ()
    {
      var aspect = new TestableAspect { Scope = AspectScope.Instance };
      var copy = (Aspect) aspect.Clone();

      Assert.That (copy.Scope, Is.EqualTo (AspectScope.Instance));
    }

    [Test]
    public void Serialize ()
    {
      var aspect = new TestableAspect
      {
        Scope = AspectScope.Instance,
        Priority = 10
      };
      var formatter = new BinaryFormatter();
      var memoryStream = new MemoryStream();

      formatter.Serialize (memoryStream, aspect);
      memoryStream.Position = 0;
      var copy = (Aspect) formatter.Deserialize (memoryStream);

      Assert.That (copy.Scope, Is.EqualTo (aspect.Scope));
      Assert.That (copy.Priority, Is.EqualTo (aspect.Priority));
    }

    [Serializable]
    private class TestableAspect : Aspect
    {
      public TestableAspect ()
      {
      }

      protected TestableAspect (SerializationInfo info, StreamingContext context)
          : base (info, context)
      {
      }

      public override void OnInvoke (Invocation invocation)
      {
        base.OnInvoke (invocation);
      }
    }
  }
}