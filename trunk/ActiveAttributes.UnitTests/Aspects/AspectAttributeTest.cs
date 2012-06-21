using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Aspects
{
  [TestFixture]
  public class AspectAttributeTest
  {
    [Serializable]
    private class TestableAspectAttribute : MethodInterceptionAspectAttribute
    {
      public TestableAspectAttribute () {}

      protected TestableAspectAttribute (SerializationInfo info, StreamingContext context)
          : base (info, context) {}

      public override void OnIntercept (IInvocation invocation)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void Clone ()
    {
      var aspect = new TestableAspectAttribute { Scope = AspectScope.Instance };
      var copy = (MethodInterceptionAspectAttribute) aspect.Clone();

      Assert.That (copy.Scope, Is.EqualTo (AspectScope.Instance));
    }

    [Test]
    public void Serialize ()
    {
      var aspect = new TestableAspectAttribute
                   {
                       Scope = AspectScope.Instance,
                       Priority = 10
                   };
      var formatter = new BinaryFormatter();
      var memoryStream = new MemoryStream();

      formatter.Serialize (memoryStream, aspect);
      memoryStream.Position = 0;
      var copy = (MethodInterceptionAspectAttribute) formatter.Deserialize (memoryStream);

      Assert.That (copy.Scope, Is.EqualTo (aspect.Scope));
      Assert.That (copy.Priority, Is.EqualTo (aspect.Priority));
    }
  }
}