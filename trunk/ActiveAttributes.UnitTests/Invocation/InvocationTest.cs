using System;
using ActiveAttributes.Core.Invocation;
using Microsoft.Scripting.Ast;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Invocation
{
  [TestFixture]
  public class InvocationTest
  {
    [Test]
    public void Proceed_Nested ()
    {
      var counter = 0;
      var @delegate = new Action (() => counter++);

      var innerInvocation = new ActionInvocation (null, null, null, @delegate);
      var outerInvocation = new ActionInvocation (innerInvocation, null, null, null);

      outerInvocation.Proceed();

      Assert.That (counter, Is.EqualTo (1));
      Assert.That (outerInvocation.Arguments, Is.EqualTo (innerInvocation.Arguments));
    }

    [Test]
    public void name ()
    {
      var type = Expression.GetDelegateType (new[] { typeof (int), typeof (void) });
      Assert.That (type, Is.EqualTo (typeof (Action<int>)));

      type = Expression.GetDelegateType (new[] { typeof (int), typeof (int) });
      Assert.That (type, Is.EqualTo (typeof (Func<int, int>)));
    }
  }
}