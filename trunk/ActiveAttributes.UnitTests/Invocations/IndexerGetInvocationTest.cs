using System;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Invocations
{
  [TestFixture]
  public class IndexerGetInvocationTest
  {
    [Test]
    public void Initialization ()
    {
      var contextStub = MockRepository.GenerateStub<IndexerGetInvocationContext<object, int, int>> (null, null, null, null);
      var flag = false;
      var action = new Func<int, int> (i =>
                                       {
                                         flag = true;
                                         return 7;
                                       });
      IInvocation invocation = new IndexerGetInvocation<object, int, int> (contextStub, action);

      invocation.Proceed ();

      Assert.That (invocation.Context, Is.SameAs (contextStub));
      Assert.That (flag, Is.True);
    }
  }
}