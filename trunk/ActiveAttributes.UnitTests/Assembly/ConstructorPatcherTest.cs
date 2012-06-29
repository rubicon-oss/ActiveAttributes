using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;

using Microsoft.Scripting.Ast;

using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class ConstructorPatcherTest : TestBase
  {
    private ConstructorPatcher _patcher;
    private MethodInfo _methodInfo;
    private MethodInfo _copiedMethodInfo;

    [SetUp]
    public override void SetUp ()
    {
      _patcher = new ConstructorPatcher();
      _methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      _copiedMethodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method_Copy ()));
    }

    [Test]
    public void Init_MethodInfo ()
    {
      var instance = CreateInstance<DomainType> (new CompileTimeAspect[0], _methodInfo, _copiedMethodInfo);

      Assert.That (instance._m_Method1_MethodInfo, Is.EqualTo (_methodInfo));
    }

    [Test]
    public void Init_Delegate ()
    {
      var instance = CreateInstance<DomainType> (new CompileTimeAspect[0], _methodInfo, _copiedMethodInfo);

      Assert.That (instance._m_Method1_Delegate, Is.EqualTo (new Action (instance.Method_Copy)));
    }

    [Test]
    public void Init_InstanceAspects_Single ()
    {
      var compileTimeAspects = GetCompileTimeAspects (_methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, _methodInfo, _copiedMethodInfo);

      Assert.That (instance._m_Method1_InstanceAspects, Is.Not.Null);
      Assert.That (instance._m_Method1_InstanceAspects, Has.Length.EqualTo (1));
      Assert.That (instance._m_Method1_InstanceAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
    }

    [Test]
    public void Init_InstanceAspects_Multiple ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method2 ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (instance._m_Method1_InstanceAspects, Is.Not.Null);
      Assert.That (instance._m_Method1_InstanceAspects, Has.Length.EqualTo (2));
      Assert.That (instance._m_Method1_InstanceAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
    }

    private T CreateInstance<T> (IEnumerable<CompileTimeAspect> aspects, MethodInfo methodInfo, MethodInfo copiedMethod)
      where T: DomainTypeBase
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
            var mutableCopy = mutableType.GetOrAddMutableMethod (copiedMethod);

            _patcher.Patch (fieldData, aspects, mutableMethod, mutableCopy);
          });

      return (T) Activator.CreateInstance (type);
    }

    private CompileTimeAspect[] GetCompileTimeAspects (MethodInfo methodInfo)
    {
      var customAttributeData = CustomAttributeData.GetCustomAttributes (methodInfo);
      var compileTimeAspects = customAttributeData.Select (x => new CompileTimeAspect (x)).ToArray();
      return compileTimeAspects;
    }

    public class DomainTypeBase
    {
      public MethodInfo _m_Method1_MethodInfo;
      public Action _m_Method1_Delegate;
      public AspectAttribute[] _s_Method1_StaticAspects;
      public AspectAttribute[] _m_Method1_InstanceAspects;
    }

    public class DomainType : DomainTypeBase
    {
      [DomainAspect]
      public virtual void Method () { }

      public virtual void Method_Copy () { }

      [DomainAspect]
      [DomainAspect]
      public virtual void Method2 () { }
    }

    public class DomainAspectAttribute : AspectAttribute
    {
    }
  }
}