using System;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class CompileTimeAspectTest
  {
    [Test]
    public void Initialization ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method1()));

      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single();

      var result = new CustomDataCompileTimeAspect (customData);

      Assert.That (result.Scope, Is.EqualTo (AspectScope.Instance));
      Assert.That (result.Priority, Is.EqualTo (10));
      Assert.That (result.AspectType, Is.EqualTo (typeof (DomainAspectAttribute)));
      Assert.That (result.ConstructorInfo, Is.EqualTo (customData.Constructor));
      Assert.That (result.ConstructorArguments, Is.EqualTo (customData.ConstructorArguments));
      Assert.That (result.NamedArguments, Is.EqualTo (customData.NamedArguments));
    }

    [Test]
    public void Initialization_NoData ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method3 ()));

      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();

      var result = new CustomDataCompileTimeAspect (customData);

      Assert.That (result.Scope, Is.EqualTo (AspectScope.Static));
      Assert.That (result.Priority, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException), ExpectedMessage = "CustomAttributeData must be from an AspectAttribute")]
    public void name ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method2 ()));

      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();

      new CustomDataCompileTimeAspect (customData);
    }

    public class DomainType
    {
      [DomainAspect(Scope = AspectScope.Instance, Priority = 10)]
      public void Method1 () { }

      [DomainNonAspect]
      public void Method2 () { }

      [DomainAspect]
      public void Method3 () { }
    }

    public class DomainAspectAttribute : AspectAttribute
    {
    }

    public class DomainNonAspectAttribute : Attribute
    {
    }
  }
}