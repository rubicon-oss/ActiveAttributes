using System;
using System.Reflection;

using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectPreparerTest : TestBase
  {
    private AspectPreparer _preparer;

    private BindingFlags _instanceBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic;
    private BindingFlags _staticBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic;

    [SetUp]
    public void SetUp ()
    {
      _preparer = new AspectPreparer();
    }

    [Test]
    public void IntroduceField_OneStatic ()
    {
      var aspectsAttributes = new[] { new DomainAspect1Attribute () };
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = instance.GetType ().GetField ("_s_aspects_for_Method", _staticBindingFlags);
      Assert.That (fieldInfo != null);

      var staticAspectsArray = (AspectAttribute[]) fieldInfo.GetValue (instance);
      Assert.That (staticAspectsArray.Length, Is.EqualTo (1));

      fieldInfo = instance.GetType ().GetField ("_m_aspects_for_Method", _instanceBindingFlags);
      Assert.That (fieldInfo != null);

      var allMethodAspectsArray = ((AspectAttribute[]) fieldInfo.GetValue (instance));
      Assert.That (allMethodAspectsArray.Length, Is.EqualTo (1));
    }

    [Test]
    public void IntroduceField_MultipleStatic ()
    {
      var aspectsAttributes = new AspectAttribute[] { new DomainAspect1Attribute (), new DomainAspect2Attribute () };
      var type = CreateTypeWithAspectAttributes (aspectsAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = instance.GetType ().GetField ("_s_aspects_for_Method", _staticBindingFlags);
      Assert.That (fieldInfo != null);

      var staticAspectsArray = (AspectAttribute[]) fieldInfo.GetValue (instance);
      Assert.That (staticAspectsArray.Length, Is.EqualTo (2));
      Assert.That (staticAspectsArray[0], Is.TypeOf<DomainAspect1Attribute> ());
      Assert.That (staticAspectsArray[1], Is.TypeOf<DomainAspect2Attribute> ());

      fieldInfo = instance.GetType ().GetField ("_m_aspects_for_Method", _instanceBindingFlags);
      Assert.That (fieldInfo != null);

      var allMethodAspectsArray = ((AspectAttribute[]) fieldInfo.GetValue (instance));
      Assert.That (allMethodAspectsArray.Length, Is.EqualTo (2));
      Assert.That (allMethodAspectsArray[0], Is.TypeOf<DomainAspect1Attribute> ());
      Assert.That (allMethodAspectsArray[1], Is.TypeOf<DomainAspect2Attribute> ());
    }

    [Test]
    public void IntroduceField_OneInstance ()
    {
      var aspectAttributes = new AspectAttribute[] { new DomainAspect1Attribute { Scope = AspectScope.Instance } };
      var type = CreateTypeWithAspectAttributes (aspectAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = instance.GetType ().GetField ("_m_aspects_for_Method", _instanceBindingFlags);
      Assert.That (fieldInfo != null);

      var allMethodAspectsArray = (AspectAttribute[]) fieldInfo.GetValue (instance);
      Assert.That (allMethodAspectsArray.Length, Is.EqualTo (1));

      fieldInfo = instance.GetType ().GetField ("_s_aspects_for_Method", _staticBindingFlags);
      Assert.That (fieldInfo != null);

      var staticAspectsArray = ((AspectAttribute[]) fieldInfo.GetValue (instance));
      Assert.That (staticAspectsArray.Length, Is.EqualTo (0));
    }

    [Test]
    public void IntroduceField_MultipleInstance ()
    {
      var aspectAttributes = new AspectAttribute[]
                             {
                                 new DomainAspect1Attribute { Scope = AspectScope.Instance },
                                 new DomainAspect2Attribute { Scope = AspectScope.Instance }
                             };
      var type = CreateTypeWithAspectAttributes (aspectAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = instance.GetType().GetField ("_m_aspects_for_Method", _instanceBindingFlags);
      Assert.That (fieldInfo != null);

      var allMethodAspectsArray = (AspectAttribute[]) fieldInfo.GetValue (instance);
      Assert.That (allMethodAspectsArray.Length, Is.EqualTo (2));
      Assert.That (allMethodAspectsArray[0], Is.TypeOf<DomainAspect1Attribute>());
      Assert.That (allMethodAspectsArray[1], Is.TypeOf<DomainAspect2Attribute>());

      fieldInfo = instance.GetType ().GetField ("_s_aspects_for_Method", _staticBindingFlags);
      Assert.That (fieldInfo != null);

      var staticAspectsArray = ((AspectAttribute[]) fieldInfo.GetValue (instance));
      Assert.That (staticAspectsArray.Length, Is.EqualTo (0));
    }

    [Test]
    public void IntroduceField_MultipleMixed ()
    {
      var aspectAttributes = new AspectAttribute[]
                             {
                                 new DomainAspect1Attribute { Scope = AspectScope.Static },
                                 new DomainAspect2Attribute { Scope = AspectScope.Instance },
                             };
      var type = CreateTypeWithAspectAttributes (aspectAttributes);
      var instance = (DomainType) Activator.CreateInstance (type);

      var fieldInfo = instance.GetType ().GetField ("_s_aspects_for_Method", _staticBindingFlags);
      Assert.That (fieldInfo != null);

      var staticAspectsArray = ((AspectAttribute[]) fieldInfo.GetValue (instance));
      Assert.That (staticAspectsArray.Length, Is.EqualTo (1));
      Assert.That (staticAspectsArray[0], Is.TypeOf<DomainAspect1Attribute>());

      fieldInfo = instance.GetType ().GetField ("_m_aspects_for_Method", _instanceBindingFlags);
      Assert.That (fieldInfo != null);

      var allMethodAspectsArray = (AspectAttribute[]) fieldInfo.GetValue (instance);
      Assert.That (allMethodAspectsArray.Length, Is.EqualTo (2));
      Assert.That (allMethodAspectsArray[0], Is.TypeOf<DomainAspect1Attribute> ());
      Assert.That (allMethodAspectsArray[1], Is.TypeOf<DomainAspect2Attribute> ());
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

    public class DomainType
    {
      public void Method ()
      {
      }
    }

    public class DomainAspect1Attribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        throw new NotImplementedException ();
      }
    }

    public class DomainAspect2Attribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        throw new NotImplementedException ();
      }
    }
  }
}