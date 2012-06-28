using System;
using System.Reflection;

using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Invocations;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Utilities;


namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  // ReSharper disable PossibleNullReferenceException
  public class AspectPreparerTest : TestBase
  {
    private AspectPreparer _preparer;

    private BindingFlags _instanceBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic;
    private BindingFlags _staticBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _preparer = new AspectPreparer();
    }

    [Test]
    public void AddInstanceField ()
    {
      var aspectsAttributes = new[] { new DomainAspect1Attribute { Scope = AspectScope.Instance } };
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);

      var fieldInfo = type.GetField ("_m_aspects_for_Method", _instanceBindingFlags);
      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
    }


    [Test]
    public void AddStaticField ()
    {
      var aspectsAttributes = new[] { new DomainAspect1Attribute { Scope = AspectScope.Static } };
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);

      var fieldInfo = type.GetField ("_s_aspects_for_Method", _staticBindingFlags);
      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo.Attributes & FieldAttributes.Private, Is.EqualTo (FieldAttributes.Private));
      Assert.That (fieldInfo.Attributes & FieldAttributes.Static, Is.EqualTo (FieldAttributes.Static));
    }

    [Test]
    public void InitStaticField_Empty ()
    {
      var aspectsAttributes = new AspectAttribute[0];
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = type.GetField ("_s_aspects_for_Method", _staticBindingFlags);
      var field = (AspectAttribute[]) fieldInfo.GetValue (instance);

      Assert.That (field, Is.Not.Null);
    }

    [Test]
    public void InitStaticField_WithElements ()
    {
      var aspectsAttributes = new AspectAttribute[]
                              { 
                                new DomainAspect1Attribute { Scope = AspectScope.Static },
                                new DomainAspect2Attribute { Scope = AspectScope.Static }
                              };
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = type.GetField ("_s_aspects_for_Method", _staticBindingFlags);
      var field = (AspectAttribute[]) fieldInfo.GetValue (instance);

      Assert.That (field.Length, Is.EqualTo (2));
      Assert.That (field, Has.Some.InstanceOf<DomainAspect1Attribute>());
      Assert.That (field, Has.Some.InstanceOf<DomainAspect2Attribute>());
    }

    [Test]
    public void InitStaticField_OnlyOnce ()
    {
      var aspectsAttributes = new[] { new DomainAspect1Attribute { Scope = AspectScope.Static } };
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = type.GetField ("_s_aspects_for_Method", _staticBindingFlags);
      var fieldBefore = (AspectAttribute[]) fieldInfo.GetValue (instance);
      instance = (DomainType) Activator.CreateInstance (type);
      var fieldAfter = (AspectAttribute[]) fieldInfo.GetValue (instance);

      Assert.That (fieldBefore, Is.SameAs (fieldAfter));
    }

    [Test]
    public void InitStaticField_MultipleConstructors ()
    {
      var aspectsAttributes = new[] { new DomainAspect1Attribute { Scope = AspectScope.Static } };
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);
      var instance = (DomainType) Activator.CreateInstance (type, "arg");

      var fieldInfo = type.GetField ("_s_aspects_for_Method", _staticBindingFlags);
      var field = (AspectAttribute[]) fieldInfo.GetValue (instance);

      Assert.That (field, Is.Not.Null);
    }

    [Test]
    public void InitInstanceField_Empty ()
    {
      var aspectsAttributes = new AspectAttribute[0];
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = type.GetField ("_m_aspects_for_Method", _instanceBindingFlags);
      var field = (AspectAttribute[]) fieldInfo.GetValue (instance);

      Assert.That (field, Is.Not.Null);
    }

    [Test]
    public void InitInstanceField_WithElements ()
    {
      var aspectsAttributes = new AspectAttribute[]
                              { 
                                new DomainAspect1Attribute { Scope = AspectScope.Instance },
                                new DomainAspect2Attribute { Scope = AspectScope.Instance }
                              };
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = type.GetField ("_m_aspects_for_Method", _instanceBindingFlags);
      var field = (AspectAttribute[]) fieldInfo.GetValue (instance);

      Assert.That (field.Length, Is.EqualTo (2));
      Assert.That (field, Has.Some.InstanceOf<DomainAspect1Attribute> ());
      Assert.That (field, Has.Some.InstanceOf<DomainAspect2Attribute> ());
    }

    [Test]
    public void InitInstanceField_WithElementsFromStatic ()
    {
      var aspectsAttributes = new AspectAttribute[]
                              { 
                                new DomainAspect1Attribute { Scope = AspectScope.Static },
                                new DomainAspect1Attribute { Scope = AspectScope.Static }
                              };
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var instanceFieldInfo = type.GetField ("_m_aspects_for_Method", _instanceBindingFlags);
      var instanceField = (AspectAttribute[]) instanceFieldInfo.GetValue (instance);

      var staticFieldInfo = type.GetField ("_s_aspects_for_Method", _staticBindingFlags);
      var staticField = (AspectAttribute[]) staticFieldInfo.GetValue (instance);

      Assert.That (instanceField[0], Is.SameAs (staticField[0]));
      Assert.That (instanceField[1], Is.SameAs (staticField[1]));
    }

    private Type CreateTypeWithAspectAttributes (AspectAttribute[] aspectAttributes)
    {
      return AssembleType<DomainType> (
          mutableType =>
          {
            var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method ());
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

            _preparer.PrepareAspects (mutableType, mutableMethod, aspectAttributes);
          });
    }

    [UsedImplicitly]
    public class DomainType
    {
      public DomainType () { }

      public DomainType (string arg) { }

      public void Method () { }
    }

    [UsedImplicitly]
    public class DomainAspect1Attribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        throw new NotImplementedException ();
      }
    }

    [UsedImplicitly]
    public class DomainAspect2Attribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        throw new NotImplementedException ();
      }
    }
  }
  // ReSharper restore PossibleNullReferenceException
}