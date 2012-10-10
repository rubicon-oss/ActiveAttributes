using System;
using System.Reflection;
using ActiveAttributes.Core.Contexts;
using NUnit.Framework;

namespace ActiveAttributes.Common.UnitTests
{
  [TestFixture]
  public class ValidationContextTest
  {
    [Test]
    public void Initialization ()
    {
      var method = (MethodInfo) MethodInfo.GetCurrentMethod();
      var invocationContext = new FuncInvocationContext<ValidationContextTest, string, int> (method, this, "123") { ReturnValue = 12 };

      var validationContext = new ValidationContext (invocationContext);

      Assert.That (validationContext.Method, Is.SameAs (method));
      Assert.That (validationContext.Instance, Is.SameAs (this));
      Assert.That (validationContext.Arguments, Is.EquivalentTo (new[] { "123" }));
      Assert.That (validationContext.ReturnValue, Is.EqualTo (12));
    }
  }
}