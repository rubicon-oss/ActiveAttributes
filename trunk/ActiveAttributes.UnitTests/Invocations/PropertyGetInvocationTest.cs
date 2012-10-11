using System;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Invocations
{
  [TestFixture]
  public class PropertyGetInvocationTest
  {
    [Test]
    public void Initialization ()
    {
      var contextStub = MockRepository.GenerateStub<PropertyGetInvocationContext<object, int>> (null, null, null);
      var flag = false;
      var action = new Func<int> (() =>
                                      {
                                        flag = true;
                                        return 7;
                                      });
      IInvocation invocation = new PropertyGetInvocation<object, int> (contextStub, action);

      invocation.Proceed ();

      Assert.That (invocation.Context, Is.SameAs (contextStub));
      Assert.That (flag, Is.True);
    }
  }
}