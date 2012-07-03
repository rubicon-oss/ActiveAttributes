using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Invocations;

using NUnit.Framework;

using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class MethodPatcher2Test : TestBase
  {
    private MethodPatcher2 _patcher;
    private BindingFlags _bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance;
    private FieldIntroducer.Data _fieldData;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _patcher = new MethodPatcher2();
      
      var methodInfoField = MemberInfoFromExpressionUtility.GetField (((DomainTypeBase obj) => obj.MethodInfo));
      var delegateField = MemberInfoFromExpressionUtility.GetField (((DomainTypeBase obj) => obj.Delegate));
      var staticAspectsField = MemberInfoFromExpressionUtility.GetField ((() => DomainTypeBase.StaticAspects));
      var instanceAspectsField = MemberInfoFromExpressionUtility.GetField (((DomainTypeBase obj) => obj.InstanceAspects));
      _fieldData = new FieldIntroducer.Data
                      {
                          MethodInfoField = methodInfoField,
                          DelegateField = delegateField,
                          StaticAspectsField = staticAspectsField,
                          InstanceAspectsField = instanceAspectsField
                      };
    }

    [Ignore, Test]
    public void CopyMethod ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
      var instance = CreateInstance<DomainType> (new AspectAttribute[0], methodInfo);
      var type = instance.GetType();
      var copyMethodInfo = type.GetMethod ("_m_Method_Copy", _bindingFlags);

      Assert.That (copyMethodInfo, Is.Not.Null);
      Assert.That (copyMethodInfo.GetParameters(), Is.EqualTo (methodInfo.GetParameters()));
      Assert.That (copyMethodInfo.ReturnType, Is.EqualTo (methodInfo.ReturnType));
      Assert.That (copyMethodInfo.Invoke (instance, new object[0]), Is.EqualTo (10));
    }

    [Test]
    public void CallAspect ()
    {
      var aspects = new[] { new DomainAspectAttribute () };
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.Method ());
      var instance = CreateInstance<DomainType2> (aspects, methodInfo);

      instance.Method ();

      Assert.That (aspects[0].OnInterceptCalled, Is.True);
    }

    [Test]
    public void CallAspect_WithInvocation ()
    {
      var aspects = new[] { new DomainAspectAttribute () };
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.Method ());
      var instance = CreateInstance<DomainType2> (aspects, methodInfo);

      instance.Method ();

      Assert.That (aspects[0].Invocation, Is.Not.Null);
      Assert.That (aspects[0].Invocation, Is.TypeOf<ActionInvocation<DomainType2>> ());
    }

    [Test]
    public void CallAspect_WithInvocation_WithContext ()
    {
      var aspects = new[] { new DomainAspectAttribute () };
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType2 obj) => obj.Method ());
      var instance = CreateInstance<DomainType2> (aspects, methodInfo);

      instance.Method ();

      var ctx = aspects[0].Invocation.Context;
      Assert.That (ctx, Is.Not.Null);
      Assert.That (ctx, Is.TypeOf<ActionInvocationContext<DomainType2>>());
      Assert.That (ctx.MethodInfo, Is.EqualTo (methodInfo));
      Assert.That (ctx.Instance, Is.EqualTo (instance));
    }


    [Test]
    public void CallAspect_Proceeding ()
    {
      SkipDeletion();
      var aspects = new[] { new ProceedingDomainAspectAttribute () };
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType3 obj) => obj.Method ());
      var instance = CreateInstance<DomainType3> (aspects, methodInfo);

      instance.Method ();

      Assert.That (instance.MethodCalled, Is.True);
    }

    private T CreateInstance<T> (IEnumerable<AspectAttribute> aspects, MethodInfo methodInfo)
        where T: DomainTypeBase
    {
      var compileAspects = aspects.Select (x => new TypeArgsCompileTimeAspect (x.GetType(), null)).Cast<CompileTimeAspectBase>();
      var type = CreateType<T> (compileAspects, methodInfo);

      var instance = (T) Activator.CreateInstance (type);

      var aspectsField = _fieldData.InstanceAspectsField;
      aspectsField.SetValue (instance, aspects.ToArray());

      var methodInfoField = _fieldData.MethodInfoField;
      methodInfoField.SetValue (instance, methodInfo);

      var delegateField = _fieldData.DelegateField;
      var copyMethodInfo = instance.GetType().GetMethod ("_m_" + methodInfo.Name + "_Copy", _bindingFlags);
      var @delegate = Delegate.CreateDelegate (methodInfo.GetDelegateType(), instance, copyMethodInfo);
      delegateField.SetValue (instance, @delegate);

      return instance;
    }

    private Type CreateType<T> (IEnumerable<CompileTimeAspectBase> aspects, MethodInfo methodInfo) where T: DomainTypeBase
    {
      var type = AssembleType<T> (
          mutableType =>
          {
            var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

            _patcher.Patch (mutableMethod, _fieldData, aspects);
          });
      return type;
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
    }

    public class DomainType2 : DomainTypeBase
    {
      public virtual void Method () { }
    }

    public class DomainType3 : DomainTypeBase
    {
      public bool MethodCalled { get; private set; }
      public virtual void Method () { MethodCalled = true; }
    }

    public class DomainAspectAttribute : MethodInterceptionAspectAttribute
    {
      public bool OnInterceptCalled { get; private set; }

      public IInvocation Invocation { get; private set; }

      public override void OnIntercept (IInvocation invocation)
      {
        OnInterceptCalled = true;

        Invocation = invocation;
      }
    }
    public class ProceedingDomainAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Proceed();
      }
    }
  }
}