using System;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Invocations
{
  [TestFixture]
  public class IndexerSetInvocationTest
  {
    [Test]
    public void Initialization ()
    {
      var contextStub = MockRepository.GenerateStub<IndexerSetInvocationContext<object, int, int>> (null, null, null, null, null);
      var flag = false;
      var action = new Action<int, int> ((i, j) => flag = true);
      IInvocation invocation = new IndexerSetInvocation<object, int, int> (contextStub, action);

      invocation.Proceed ();

      Assert.That (invocation.Context, Is.SameAs (contextStub));
      Assert.That (flag, Is.True);
    }
  }
}