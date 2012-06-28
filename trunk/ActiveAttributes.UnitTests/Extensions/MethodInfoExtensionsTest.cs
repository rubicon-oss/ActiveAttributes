using System;
using ActiveAttributes.Core.Extensions;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Extensions
{
  [TestFixture]
  public class MutableMethodInfoExtensionsTest
  {
    [Test]
    public void GetDelegateType_SimpleMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SimpleMethod ()));
      var result = methodInfo.GetDelegateType ();

      Assert.That (result, Is.EqualTo (typeof (Action)));
    }

    [Test]
    public void GetDelegateType_ArgMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ArgMethod ("a")));
      var result = methodInfo.GetDelegateType ();

      Assert.That (result, Is.EqualTo (typeof (Action<string>)));
    }

    [Test]
    public void GetDelegateType_ReturnMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ReturnMethod ()));
      var result = methodInfo.GetDelegateType ();

      Assert.That (result, Is.EqualTo (typeof (Func<string>)));
    }

    [Test]
    public void GetDelegateType_MixedMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.MixedMethod ("a", 1)));
      var result = methodInfo.GetDelegateType ();

      Assert.That (result, Is.EqualTo (typeof (Func<string, int, object>)));
    }

    [Test]
    public void GetDelegateType_AddInstanceType ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.MixedMethod ("a", 1)));
      var result = methodInfo.GetDelegateType (methodInfo.DeclaringType);

      Assert.That (result, Is.EqualTo (typeof (Func<DomainType, string, int, object>)));
    }

    public class DomainType
    {
      public void SimpleMethod () { }
      public void ArgMethod (string a) { }
      public string ReturnMethod () { return default (string); }
      public object MixedMethod (string a, int b) { return default (object); }
    }
  }
}