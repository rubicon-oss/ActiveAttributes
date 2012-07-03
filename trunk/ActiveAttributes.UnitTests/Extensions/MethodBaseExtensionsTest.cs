using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Remotion.Utilities;
using ActiveAttributes.Core.Extensions;

namespace ActiveAttributes.UnitTests.Extensions
{
  [TestFixture]
  public class MethodBaseExtensionsTest
  {
    [Test]
    public void IsCompilerGenerated_ReturnsTrue ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.CompilerGeneratedMethod()));

      var result = methodInfo.IsCompilerGenerated();

      Assert.That (result, Is.True);
    }

    [Test]
    public void IsCompilerGenerated_ReturnsFalse ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.NotCompilerGeneratedMethod()));

      var result = methodInfo.IsCompilerGenerated();

      Assert.That (result, Is.False);
    }

    private class DomainType
    {
      [CompilerGenerated]
      public void CompilerGeneratedMethod () { }

      public void NotCompilerGeneratedMethod () { }
    }
  }
}