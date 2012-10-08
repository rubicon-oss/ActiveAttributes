using System;
using System.Linq;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Providers;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.UnitTests.Assembly.Providers
{
  [TestFixture]
  public class InterfaceMethodAspectProviderTest
  {
    private InterfaceMethodAspectProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new InterfaceMethodAspectProvider();
    }

    [Test]
    public void Normal ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());

      var result = _provider.GetAspects (method).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.Matches<IAspectDescriptor> (desc => desc.AspectType == typeof (InheritingAspectAttribute)));
    }

    [Test]
    public void Inherited ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method2());

      var result = _provider.GetAspects (method).ToArray();

      Assert.That (result, Has.Length.EqualTo (2));
      Assert.That (result, Has.All.Matches<IAspectDescriptor> (desc => desc.AspectType == typeof (InheritingAspectAttribute)));
    }


    private interface IDomainInterface
    {
      [InheritingAspect]
      [NotInheritingAspect]
      void Method ();

      [InheritingAspect]
      void Method2 ();
    }

    interface IOverlappingInterface
    {
      [InheritingAspect]
      void Method2 ();
    }

    interface IDerivedInterface : IOverlappingInterface { }

    class DomainType : IDomainInterface, IDerivedInterface
    {
      public void Method () { }
      public void Method2 () { }
    }

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    class InheritingAspectAttribute : Core.Aspects.AspectAttribute { }

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    class NotInheritingAspectAttribute : Core.Aspects.AspectAttribute { }
  }
}