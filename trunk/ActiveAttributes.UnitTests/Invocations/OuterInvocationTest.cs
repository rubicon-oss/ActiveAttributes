using System;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Invocations
{
  [TestFixture]
  public class OuterInvocationTest
  {
    [Test]
    public void Initialization ()
    {
      var contextStub = MockRepository.GenerateStub<IInvocationContext> ();
      IInvocation proceededInnerInvocation = null;
      var innerInterception = new Action<IInvocation> (i => proceededInnerInvocation = i);
      var innerInvocation = MockRepository.GenerateMock<IInvocation> ();
      IInvocation invocation = new OuterInvocation (contextStub, innerInterception, innerInvocation);

      invocation.Proceed ();

      Assert.That (invocation.Context, Is.SameAs (contextStub));
      Assert.That (proceededInnerInvocation, Is.SameAs (innerInvocation));
    }
  }
}