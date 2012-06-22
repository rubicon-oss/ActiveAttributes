using System;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Aspects
{
  [TestFixture]
  public class MethodPatcherTest : TestBase
  {
    private MethodPatcher _patcher;

    private BindingFlags _instanceBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic;

    [SetUp]
    public void SetUp ()
    {
      _patcher = new MethodPatcher();
    }

    [Test]
    public void GetCopiedMethod ()
    {
      var type = CreateTypeWithAspectAttributes();
      var instance = Activator.CreateInstance (type);

      var methodInfo = type.GetMethod("_m_Method", _instanceBindingFlags);
      Assert.That (methodInfo, Is.Not.Null);

      var result = methodInfo.Invoke (instance, new object[] { 1 });
      Assert.That (result, Is.EqualTo (2));
    }

    private Type CreateTypeWithAspectAttributes ()
    {
      return AssembleType<DomainType> (
          mutableType =>
          {
            var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (1));
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);
            _patcher.PatchMethod (mutableType, mutableMethod, null);
          });
    }

    public class DomainType
    {
      public int Method (int i)
      {
        return i + 1;
      }
    }

  }
}