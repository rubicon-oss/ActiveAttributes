using System;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Invocations
{
  [TestFixture]
  public class PropertySetInvocationTest
  {
    [Test]
    public void Initialization ()
    {
      var contextStub = MockRepository.GenerateStub<PropertySetInvocationContext<object, int>> (null, null, null, null);
      var flag = false;
      var action = new Action<int> (i => flag = true);
      IInvocation invocation = new PropertySetInvocation<object, int> (contextStub, action);

      invocation.Proceed();

      Assert.That (invocation.Context, Is.SameAs (contextStub));
      Assert.That (flag, Is.True);
    }
  }
}