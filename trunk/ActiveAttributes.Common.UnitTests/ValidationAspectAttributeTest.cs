//Sample license text.

using System;
using ActiveAttributes.Core;
using NUnit.Framework;
using XValidation;
using XValidation.Validations;

namespace ActiveAttributes.Common.UnitTests
{
  [TestFixture]
  public class ValidationAspectAttributeTest
  {
    [Test]
    public void Validate ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      instance.Method (new object());

      Assert.That (DomainValidation.ValidationCalled, Is.True);
    }

    public class DomainType
    {
      [ValidationAspect]
      public virtual void Method ([DomainValidation] object obj)
      {
      }
    }

    private class DomainValidation : Attribute, IValidationAttribute
    {
      public static bool ValidationCalled { get; private set; }

      public IValidation Validation
      {
        get { return new LambdaValidation<object> ((o, s) => ValidationCalled = true); }
      }
    }
  }
}