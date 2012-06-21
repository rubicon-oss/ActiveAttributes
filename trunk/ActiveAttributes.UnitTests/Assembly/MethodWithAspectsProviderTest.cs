using System;
using System.Diagnostics;
using System.Linq;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class MethodWithAspectsProviderTest
  {
    #region Setup/Teardown

    [SetUp]
    public void SetUp ()
    {
      _provider = new MethodWithAspectsProvider();
    }

    #endregion

    private MethodWithAspectsProvider _provider;

    private class DomainType1
    {
      [DomainMethodAspect]
      public void Method () {}
    }

    private class DomainType2
    {
      [DomainPropertyAspect]
      public string Property { get; private set; }
    }

    private class DomainType3
    {
      public string Property { [DomainMethodAspect]
      get; private set; }
    }

    private class DomainMethodAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (Invocation invocation)
      {
        throw new NotImplementedException();
      }
    }

    private class DomainPropertyAspectAttribute : PropertyInterceptionAspectAttribute
    {
      public override void InterceptGet (IInvocation invocation)
      {
        throw new NotImplementedException();
      }

      public override void InterceptSet (IInvocation invocation)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void GetMethodsWithAspects ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType1));

      var result = _provider.GetMethodsWithAspects (mutableType).GetEnumerator();

      //Assert.That (result.MoveNext(), Is.True);
      result.MoveNext();
      Assert.That (result.Current != null);
      Assert.That (result.Current.Item1.Name, Is.EqualTo ("Method"));
      Assert.That (result.Current.Item2.Count(), Is.EqualTo (1));
      Assert.That (result.Current.Item2.First(), Is.TypeOf<DomainMethodAspectAttribute>());
      Assert.That (result.MoveNext(), Is.False);
    }

    [Test]
    public void GetMethodsWithAspects_CompilerGenerated_MethodAspect ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType3));

      var result = _provider.GetMethodsWithAspects (mutableType).GetEnumerator();

      result.MoveNext();
      Assert.That (result.Current != null);
      Assert.That (result.Current.Item1.Name, Is.EqualTo ("get_Property"));
      Assert.That (result.Current.Item2.Count(), Is.EqualTo (1));
      Assert.That (result.Current.Item2.First(), Is.TypeOf<DomainMethodAspectAttribute>());
      Assert.That (result.MoveNext(), Is.False);
    }

    [Test]
    public void GetMethodsWithAspects_CompilerGenerated_PropertyAspect ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType2));

      var result = _provider.GetMethodsWithAspects (mutableType).GetEnumerator();

      result.MoveNext();
      Assert.That (result.Current != null);
      Assert.That (result.Current.Item1.Name, Is.EqualTo ("get_Property"));
      Assert.That (result.Current.Item2.Count(), Is.EqualTo (1));
      Assert.That (result.Current.Item2.First(), Is.TypeOf<DomainPropertyAspectAttribute>());
      result.MoveNext();
      Assert.That (result.Current != null);
      Assert.That (result.Current.Item1.Name, Is.EqualTo ("set_Property"));
      Assert.That (result.Current.Item2.Count(), Is.EqualTo (1));
      Assert.That (result.Current.Item2.First(), Is.TypeOf<DomainPropertyAspectAttribute>());
      Assert.That (result.MoveNext(), Is.False);
    }

    [Test]
    public void GetMethodsWithAspects_NotNull ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType1));

      var result = _provider.GetMethodsWithAspects (mutableType);

      Assert.That (result, Is.Not.Null);
    }
  }
}