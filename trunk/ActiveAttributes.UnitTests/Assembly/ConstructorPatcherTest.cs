using System;
using System.Collections.Generic;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class ConstructorPatcherTest : TestBase
  {
    private ConstructorPatcher _patcher;

    [SetUp]
    public override void SetUp ()
    {
      _patcher = new ConstructorPatcher();
    }

    [Test]
    public void Init_MethodInfo ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method()));
      var instance = CreateInstance<DomainType> (new CompileTimeAspect[0], methodInfo);

      Assert.That (instance._m_Method1_MethodInfo, Is.EqualTo (methodInfo));
    }

    private T CreateInstance<T> (IEnumerable<CompileTimeAspect> aspects, MethodInfo methodInfo) where T: DomainTypeBase
    {
      var methodInfoField = MemberInfoFromExpressionUtility.GetField (((T obj) => obj._m_Method1_MethodInfo));
      var delegateField = MemberInfoFromExpressionUtility.GetField (((T obj) => obj._m_Method1_Delegate));
      var staticAspectsField = MemberInfoFromExpressionUtility.GetField (((T obj) => obj._s_Method1_StaticAspects));
      var instanceAspectsField = MemberInfoFromExpressionUtility.GetField (((T obj) => obj._m_Method1_InstanceAspects));
      var fieldData = new FieldIntroducer.Data
                      {
                          MethodInfoField = methodInfoField,
                          DelegateField = delegateField,
                          StaticAspectsField = staticAspectsField,
                          InstanceAspectsField = instanceAspectsField
                      };

      var type = AssembleType<T> (
          mutableType =>
          {
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);
            _patcher.Patch (fieldData, aspects, mutableMethod);
          });

      return (T) Activator.CreateInstance (type);
    }

    public class DomainTypeBase
    {
      public MethodInfo _m_Method1_MethodInfo;
      public Action<DomainType> _m_Method1_Delegate;
      public AspectAttribute[] _s_Method1_StaticAspects;
      public AspectAttribute[] _m_Method1_InstanceAspects;
    }

    public class DomainType : DomainTypeBase
    {
      public void Method ()
      {
      }
    }
  }
}