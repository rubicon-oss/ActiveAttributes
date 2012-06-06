using System;
using System.Diagnostics;
using System.Reflection;
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class AssemblerTest_Validation : TestBase
  {
    [Test]
    public void Validating_ValidMethod ()
    {
      var assembler = new Assembler ();
      var type = AssembleType<ValidDomainType> (assembler.ModifyType);
      var instance = (ValidDomainType) Activator.CreateInstance (type);
    }

    [Test]
    [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Method 'InvalidMethod' is not valid for aspect of type 'ValidatingAspect'")]
    public void Validating_InvalidMethod ()
    {

      var assembler = new Assembler ();
      var type = AssembleType<InvalidDomainType> (assembler.ModifyType);
      var instance = (InvalidDomainType) Activator.CreateInstance (type);
    }

    public class ValidDomainType
    {
      [ValidatingAspect]
      public virtual void ValidMethod () { }
    }

    public class InvalidDomainType
    {
      [ValidatingAspect]
      public virtual void InvalidMethod () { }
    }

    public class ValidatingAspect : Aspect
    {
      public override bool Validate (MethodInfo method)
      {
        return method.Name.EndsWith ("ValidMethod");
      }
    }
  }
}