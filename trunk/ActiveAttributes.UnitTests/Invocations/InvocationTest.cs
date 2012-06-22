using System;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Invocations
{
  [TestFixture]
  public class InvocationTest
  {
    [Test]
    public void NestedInvocation ()
    {
      var obj = new DomainType();
      var context = new ActionInvocationContext<object> (null, null);
      var outerCalled = false;
      var innerInvocation = new ActionInvocation<object> (context, obj.Method);
      var outerInvocation = new ActionInvocation<object> (
          context,
          invocation =>
          {
            outerCalled = true;
            invocation.Proceed();
          },
          innerInvocation);

      outerInvocation.Proceed();

      Assert.That (obj.MethodExecutionCounter, Is.EqualTo (1));
      Assert.That (outerCalled, Is.True);
    }

    public class DomainType
    {
      public int MethodExecutionCounter { get; private set; }

      public void Method ()
      {
        MethodExecutionCounter++;
      }
    }
  }
}