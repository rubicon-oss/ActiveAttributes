using System;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class MethodCopierTest : TestBase
  {
    private MethodCopier _copier;

    [SetUp]
    public void SetUp ()
    {
      _copier = new MethodCopier();
    }

    [Test]
    public void CopyMethod_Exists ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (1));
      var type = CreateType<DomainType> (methodInfo);

      var methodInfos = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).ToArray();
      var copiedMethodInfo = methodInfos.Where (x => x.Name.EndsWith ("Copy")).SingleOrDefault();

      Assert.That (copiedMethodInfo, Is.Not.Null);
    }

    [Test]
    public void CopyMethod_SameBody ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (1));
      var instance = CreateInstance<DomainType> (methodInfo);
      var type = instance.GetType();

      var methodInfos = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).ToArray();
      var copiedMethodInfo = methodInfos.Where (x => x.Name.EndsWith ("Copy")).SingleOrDefault();

      var result = copiedMethodInfo.Invoke (instance, new object[] { 10 });

      Assert.That (result, Is.EqualTo (10));
    }

    private T CreateInstance<T> (MethodInfo methodInfo)
    {
      var type = CreateType<T> (methodInfo);

      var instance = (T) Activator.CreateInstance (type);
      return instance;
    }

    private Type CreateType<T> (MethodInfo methodInfo)
    {
      var type = AssembleType<T> (
          mutableType =>
          {
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

            _copier.GetCopy (mutableMethod);
          });
      return type;
    }

    public class DomainType
    {
      public int Method (int i) { return i; }
    }
  }
}