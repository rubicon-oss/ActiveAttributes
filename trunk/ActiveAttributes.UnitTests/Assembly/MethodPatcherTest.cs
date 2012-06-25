using System;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
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

    [Ignore, Test]
    public void GetCopiedMethod ()
    {
      var fieldInfo = typeof (DomainType).GetField ("_m_aspects_for_Method");
      var type = CreateTypeWithAspectAttributes(fieldInfo);
      var instance = Activator.CreateInstance (type);

      var methodInfo = type.GetMethod("_m_Method", _instanceBindingFlags);
      Assert.That (methodInfo, Is.Not.Null);

      var result = methodInfo.Invoke (instance, new object[] { 1 });
      Assert.That (result, Is.EqualTo (2));
    }

    [Test]
    public void InvokesAspects ()
    {
      SkipDeletion();
      var fieldInfo = typeof(DomainType).GetField ("_m_aspects_for_Method");
      var type = CreateTypeWithAspectAttributes(fieldInfo);
      var instance = (DomainType) Activator.CreateInstance (type);
      var aspectsArray = (AspectAttribute[]) fieldInfo.GetValue (instance);

      instance.Method (1);

      var aspectAttribute = (DomainAspectAttribute) aspectsArray[0];
      Assert.That (aspectAttribute.OnInterceptCalled, Is.True);
      Assert.That (aspectAttribute.Invocation, Is.Not.Null);
      Assert.That (aspectAttribute.Invocation.Context.Arguments[0], Is.EqualTo (1));
    }

    private Type CreateTypeWithAspectAttributes (FieldInfo fieldInfo)
    {
      return AssembleType<DomainType> (
          mutableType =>
          {
            var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method (1));
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);
            _patcher.PatchMethod (mutableType, mutableMethod, fieldInfo, new AspectAttribute[] { new DomainAspectAttribute() });
          });
    }
  }

  public class DomainAspectAttribute : MethodInterceptionAspectAttribute
  {
    public bool OnInterceptCalled { get; private set; }
    public IInvocation Invocation { get; private set; }

    public override void OnIntercept (IInvocation invocation)
    {
      OnInterceptCalled = true;
      Invocation = invocation;
      var context = (FuncInvocationContext<DomainType, int, int>) invocation.Context;
    }
  }

  public class DomainType
  {
    public AspectAttribute[] _m_aspects_for_Method = new[] { new DomainAspectAttribute () };

    public virtual int Method (int i)
    {
      return i + 1;
    }
  }
}