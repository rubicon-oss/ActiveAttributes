using System;
using System.Reflection;
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class AssemblerTest : TestBase
  {
    private DomainType _instance;
    private Type _type;

    [SetUp]
    public void SetUp ()
    {
      var assembler = new Assembler ();
      _type = AssembleType<DomainType> (assembler.ModifyType);
      _instance = (DomainType) Activator.CreateInstance (_type);
    }

    [Test]
    public void Proceed_NonProceeding ()
    {
      _instance.NonProceedingMethod ();

      Assert.That (_instance.NonProceedingMethodCalled, Is.False);
    }

    [Test]
    public void Proceed_NonProceeding_ValueType ()
    {
      var result = _instance.NonProceedingMethodValueType();

      Assert.That (result, Is.EqualTo (0));
    }

    [Test]
    public void Proceed_Proceeding ()
    {
      _instance.ProceedingMethod ();

      Assert.That (_instance.ProceedingMethodCalled, Is.True);
    }

    [Test]
    public void Proceed_ModifyStringArgument ()
    {
      _instance.ModifyStringArgumentMethod ("a");

      Assert.That (_instance.ModifyStringArgumentMethodArgs, Is.EqualTo ("a_modified"));
    }

    [Test]
    public void Proceed_ModifyIntArgument ()
    {
      _instance.ModifyIntArgumentMethod (1);

      Assert.That (_instance.ModifyIntArgumentMethodArgs, Is.EqualTo (2));
    }

    [Test]
    public void Proceed_ModifyReturnValue ()
    {
      var result = _instance.ModifyReturnValueMethod();

      Assert.That (result, Is.EqualTo ("method_aspect"));
    }

    [Test]
    public void Proceed_ModifyMultiple ()
    {
      var result = _instance.ModifyMultipleMethod (1, "foo");

      Assert.That (result, Is.EqualTo ("2_foo#"));
    }

    [Test]
    public void Field_Introduction ()
    {
      var fieldInfo = _type.GetField ("tp<>__aspects_NonProceedingMethod", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That (fieldInfo, Is.Not.Null);
      var field = (Aspect[]) fieldInfo.GetValue (_instance);

      Assert.That (field[0], Is.TypeOf<NonProceedingAspect>());
    }

    //[Ignore, Test]
    //public void Field_Introduction_Properties ()
    //{
    //  var fieldInfo = _type.GetField ("_aspects_set_SomeProperty", BindingFlags.NonPublic | BindingFlags.Instance);
    //  Assert.That (fieldInfo, Is.Not.Null);
    //  var field = (Aspect[]) fieldInfo.GetValue (_instance);

    //  Assert.That (field[0], Is.TypeOf<PropertyAspect> ());
    //  _instance.SomeProperty = "muh";
    //}

    [Test]
    public void Field_OrderedByPriority ()
    {
      var result2 = _instance.ProceedAddMethod (1);
      Assert.That (result2, Is.EqualTo (1));

      var result1 = _instance.AddProceedMethod (1);
      Assert.That (result1, Is.EqualTo (11));
    }

    [Test]
    public void Proceed_TwiceProceeding ()
    {
      _instance.TwiceProceedingMethod();

      Assert.That (_instance.TwiceProceedingMethodCallCount, Is.EqualTo (2));
    }

    [Test]
    public void Proceed_AccessMethodInfo ()
    {
      var result = _instance.AccessMethodInfoMethod();

      Assert.That (result, Is.EqualTo ("AccessMethodInfoMethod"));
    }

    [Test]
    public void Proceed_AccessInstance ()
    {
      var result = _instance.AccessInstanceMethod();

      Assert.That (result, Is.SameAs (_instance));
    }

    [Test]
    public void Proceed_AccessTag ()
    {
      var result = _instance.AccessTagMethod(1);

      Assert.That (result, Is.EqualTo (2));
    }

    public class DomainType
    {

      public bool NonProceedingMethodCalled { get; private set; }
      [NonProceedingAspect]
      public virtual void NonProceedingMethod () { NonProceedingMethodCalled = true; }

      [NonProceedingAspect]
      public virtual int NonProceedingMethodValueType () { return 1; }

      public bool ProceedingMethodCalled { get; private set; }
      [ProceedingAspect]
      public virtual void ProceedingMethod () { ProceedingMethodCalled = true; }

      public string ModifyStringArgumentMethodArgs { get; private set; }
      [ModifyStringArgumentAspect]
      public virtual void ModifyStringArgumentMethod (string a) { ModifyStringArgumentMethodArgs = a; }

      public int ModifyIntArgumentMethodArgs { get; private set; }
      [ModifyIntArgumentAspect]
      public virtual void ModifyIntArgumentMethod (int a) { ModifyIntArgumentMethodArgs = a; }

      [ModifyReturnValueAspect]
      public virtual string ModifyReturnValueMethod () { return "method"; }

      [ModifyMultipleAspect]
      public virtual string ModifyMultipleMethod (int i, string str) { return i + str; }

      public int TwiceProceedingMethodCallCount { get; private set; }
      [ProceedingAspect]
      [ProceedingAspect]
      public virtual void TwiceProceedingMethod () { TwiceProceedingMethodCallCount++; }

      [Add10ToArgAspect (Priority = 1)]
      [ProceedingAspect (Priority = 2)]
      public virtual int ProceedAddMethod (int i) { return i; }

      [Add10ToArgAspect (Priority = 2)]
      [ProceedingAspect (Priority = 1)]
      public virtual int AddProceedMethod (int i) { return i; }

      [AccessMethodInfoAspect]
      public virtual string AccessMethodInfoMethod () { return string.Empty; }

      [AccessInstanceAspect]
      public virtual object AccessInstanceMethod () { return null; }

      [AccessTagAspect]
      public virtual int AccessTagMethod (int i) { return i; }

      //[PropertyAspect]
      //public virtual string SomeProperty { get; set; }
    }

    public class NonProceedingAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        Assert.That (invocation.Arguments, Is.Empty);
      }
    }

    public class ProceedingAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.Proceed ();
      }
    }

    public class ModifyStringArgumentAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.Arguments[0] = (string) invocation.Arguments[0] + "_modified";
        invocation.Proceed ();
      }
    }

    public class ModifyIntArgumentAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.Arguments[0] = (int) invocation.Arguments[0] + 1;
        invocation.Proceed ();
      }
    }

    public class ModifyReturnValueAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.Proceed();
        invocation.ReturnValue = invocation.ReturnValue + "_aspect";
      }
    }

    public class ModifyMultipleAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.Arguments[0] = (int) invocation.Arguments[0] + 1;
        invocation.Arguments[1] = "_" + (string) invocation.Arguments[1];

        invocation.Proceed();

        invocation.ReturnValue = invocation.ReturnValue + "#";
      }
    }

    public class Add10ToArgAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.Arguments[0] = (int) invocation.Arguments[0] + 10;
      }
    }

    public class AccessMethodInfoAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.ReturnValue = invocation.Method.Name;
      }
    }

    public class AccessInstanceAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.ReturnValue = invocation.Instance;
      }
    }

    public class AccessTagAspect : MethodBoundaryAspect
    {
      public override void OnEntry (Invocation invocation)
      {
        invocation.Tag = (int) invocation.Arguments[0] + 1;
      }

      public override void OnExit (Invocation invocation)
      {
        invocation.ReturnValue = (int) invocation.Tag;
      }
    }

    //public class PropertyAspect : PropertyInterceptionAspect
    //{
    //  public override void OnInvoke (Invocation invocation)
    //  {
    //    base.OnInvoke (invocation);
    //  }
    //}
  }
}