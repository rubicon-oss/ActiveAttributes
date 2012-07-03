using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration;
using JetBrains.Annotations;
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
      DomainTypeBase.StaticAspects = null;
    }

    [Test]
    public void Init_MethodInfo ()
    {
      SkipDeletion();
      var instance = CreateInstance<DomainType> (new CustomDataCompileTimeAspect[0], _methodInfo, _copiedMethodInfo);

      Assert.That (instance.MethodInfo, Is.EqualTo (_methodInfo));
    }

    [Test]
    public void Init_Delegate ()
    {
      var instance = CreateInstance<DomainType> (new CustomDataCompileTimeAspect[0], _methodInfo, _copiedMethodInfo);

      Assert.That (instance.Delegate, Is.EqualTo (new Action (instance.Method_Copy)));
    }

    [Test]
    public void Init_InstanceAspects_Empty ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleInstanceAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (instance.InstanceAspects, Is.Not.Null);
    }

    [Test]
    public void Init_InstanceAspects_Single ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleInstanceAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (instance.InstanceAspects, Has.Length.EqualTo (1));
      Assert.That (instance.InstanceAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
    }

    [Test]
    public void Init_InstanceAspects_Multiple ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.MultiInstanceAspectsMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (instance.InstanceAspects, Has.Length.EqualTo (2));
      Assert.That (instance.InstanceAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
      Assert.That (instance.InstanceAspects[0], Is.Not.SameAs (instance.InstanceAspects[1]));
    }

    [Test]
    public void Init_StaticAspects_Empty ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleStaticAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (DomainTypeBase.StaticAspects, Is.Not.Null);
    }

    [Test]
    public void Init_StaticAspects_Single ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleStaticAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (DomainTypeBase.StaticAspects, Has.Length.EqualTo (1));
      Assert.That (DomainTypeBase.StaticAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
      Assert.That (DomainTypeBase.StaticAspects[0], Is.SameAs (instance.InstanceAspects[0]));
    }

    [Test]
    public void Init_StaticAspects_Multiple ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.MultiStaticAspectsMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      Assert.That (DomainTypeBase.StaticAspects, Has.Length.EqualTo (2));
      Assert.That (DomainTypeBase.StaticAspects, Has.All.InstanceOf<DomainAspectAttribute> ());
      Assert.That (DomainTypeBase.StaticAspects[0], Is.SameAs (instance.InstanceAspects[0]));
      Assert.That (DomainTypeBase.StaticAspects[1], Is.SameAs (instance.InstanceAspects[1]));
    }

    [Test]
    public void Init_StaticAspects_OnlyOnce ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.SingleStaticAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);
      var before = DomainTypeBase.StaticAspects;
      CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);
      var after = DomainTypeBase.StaticAspects;

      Assert.That (after, Is.SameAs (before));
    }

    [Test]
    public void Init_Aspects_CtorElementArguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.CtorElementArgAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      var ctorArgAspect = (CtorArgsDomainAspectAttribute) instance.InstanceAspects[0];
      Assert.That (ctorArgAspect.ElementArg, Is.EqualTo ("a"));
    }

    [Test]
    public void Init_Aspects_NamedElementArguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.NamedElementArgAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      var namedArgAspect = (NamedArgsDomainAspectAttribute) instance.InstanceAspects[0];
      Assert.That (namedArgAspect.ElementArg, Is.EqualTo ("a"));
      Assert.That (namedArgAspect.Priority, Is.EqualTo (10));
    }

    [Test]
    public void Init_Aspects_CtorArrayArguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.CtorArrayArgAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      var ctorArgAspect = (CtorArgsDomainAspectAttribute) instance.InstanceAspects[0];
      Assert.That (ctorArgAspect.ArrayArg, Is.EqualTo (new[] { "a" }));
    }

    [Test]
    public void Init_Aspects_NamedArrayArguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.NamedArrayArgAspectMethod ()));
      var compileTimeAspects = GetCompileTimeAspects (methodInfo);
      var instance = CreateInstance<DomainType> (compileTimeAspects, methodInfo, _copiedMethodInfo);

      var namedArgAspect = (NamedArgsDomainAspectAttribute) instance.InstanceAspects[0];
      Assert.That (namedArgAspect.ArrayArg, Is.EqualTo (new[] { "a" }));
    }

    [Test]
    public void Patch_MultipleConstructors ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.Method ()));
      var ctorArgs = new object[] { "a" };
      var instance = CreateInstance<DomainType2> (new CustomDataCompileTimeAspect[0], methodInfo, methodInfo, ctorArgs);
      var instance2 = CreateInstance<DomainType2> (new CustomDataCompileTimeAspect[0], methodInfo, methodInfo);

      Assert.That (instance.MethodInfo, Is.Not.Null);
      Assert.That (instance.Delegate, Is.Not.Null);
      Assert.That (DomainType2.StaticAspects, Is.Not.Null);
      Assert.That (instance.InstanceAspects, Is.Not.Null);

      Assert.That (instance2.MethodInfo, Is.Not.Null);
      Assert.That (instance2.Delegate, Is.Not.Null);
      Assert.That (DomainType2.StaticAspects, Is.Not.Null);
      Assert.That (instance2.InstanceAspects, Is.Not.Null);
    }

    private T CreateInstance<T> (IEnumerable<CompileTimeAspectBase> aspects, MethodInfo methodInfo, MethodInfo copiedMethod, params object[] args)
      where T: DomainTypeBase
    {
      var methodInfoField = MemberInfoFromExpressionUtility.GetField (((T obj) => obj.MethodInfo));
      var delegateField = MemberInfoFromExpressionUtility.GetField (((T obj) => obj.Delegate));
      var staticAspectsField = MemberInfoFromExpressionUtility.GetField (((T obj) => DomainTypeBase.StaticAspects));
      var instanceAspectsField = MemberInfoFromExpressionUtility.GetField (((T obj) => obj.InstanceAspects));
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

      return (T) Activator.CreateInstance (type, args);
    }

    private CustomDataCompileTimeAspect[] GetCompileTimeAspects (MethodInfo methodInfo)
    {
      var customAttributeData = CustomAttributeData.GetCustomAttributes (methodInfo);
      var compileTimeAspects = customAttributeData.Select (x => new CustomDataCompileTimeAspect (x)).ToArray();
      return compileTimeAspects;
    }

    public abstract class DomainTypeBase
    {
      public MethodInfo MethodInfo;
      public Action Delegate;
      public static AspectAttribute[] StaticAspects;
      public AspectAttribute[] InstanceAspects;
    }

    public class DomainType : DomainTypeBase
    {
      public virtual void Method () { }

      public virtual void Method_Copy () { }

      [DomainAspect (Scope = AspectScope.Instance)]
      public virtual void SingleInstanceAspectMethod () { }

      [DomainAspect (Scope = AspectScope.Instance)]
      [DomainAspect (Scope = AspectScope.Instance)]
      public virtual void MultiInstanceAspectsMethod () { }

      [DomainAspect (Scope = AspectScope.Static)]
      public virtual void SingleStaticAspectMethod () { }

      [DomainAspect (Scope = AspectScope.Static)]
      [DomainAspect (Scope = AspectScope.Static)]
      public virtual void MultiStaticAspectsMethod () { }

      [CtorArgsDomainAspect ("a")]
      public virtual void CtorElementArgAspectMethod () { }

      [NamedArgsDomainAspect (ElementArg = "a", Priority = 10)]
      public virtual void NamedElementArgAspectMethod () { }

      [CtorArgsDomainAspect (new[] { "a" })]
      public virtual void CtorArrayArgAspectMethod () { }

      [NamedArgsDomainAspect (ArrayArg = new[] { "a" })]
      public virtual void NamedArrayArgAspectMethod () { }
    }

    [UsedImplicitly]
    public class DomainType2 : DomainTypeBase
    {
      public DomainType2 ()
      {
      }
      public DomainType2 (string arg)
      {
      }

      public virtual void Method () { }
    }

    public class DomainAspectAttribute : AspectAttribute
    {
    }

    public class CtorArgsDomainAspectAttribute : AspectAttribute
    {
      public string ElementArg { get; set; }
      public string[] ArrayArg { get; set; }

      public CtorArgsDomainAspectAttribute (string elementArg)
      {
        ElementArg = elementArg;
      }

      public CtorArgsDomainAspectAttribute (string[] arrayArg)
      {
        ArrayArg = arrayArg;
      }
    }

    public class NamedArgsDomainAspectAttribute : AspectAttribute
    {
      public string ElementArg { get; set; }
      public string[] ArrayArg { get; set; }

      public NamedArgsDomainAspectAttribute ()
      {
      }
    }
  }
}