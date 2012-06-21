using System;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectInitializerTest : TestBase
  {
    private AspectInitializer _initializer;
    private DomainType _object;

    [SetUp]
    public void SetUp ()
    {
      _initializer = new AspectInitializer();
    }

    [Test]
    public void SimpleType ()
    {
      //_initializer.IntroduceInitialization (type, mutableMethod, aspects);
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            var aspects = new[] { new DomainAspectAttribute() };
            var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

            //_initializer.IntroduceInitialization (mutableType, mutableMethod, aspects);
          });
      var instance = (DomainType) Activator.CreateInstance (type);
      var fieldInfo = type.GetField ("_m_aspects_for_Method");

      Assert.That (fieldInfo, Is.Not.Null);
    }

    [Test]
    public void CheckOverloadedMethods ()
    {
      
    }

    public class DomainType
    {
      public DomainType ()
      {
      }

      public void Method ()
      {
      }
    }

    public class DomainAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        throw new NotImplementedException();
      }
    }
  }
}