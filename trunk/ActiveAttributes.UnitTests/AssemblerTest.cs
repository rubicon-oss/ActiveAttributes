using System;
using System.Diagnostics;
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
    public void Proceed_NonProceedingAspect ()
    {
      _instance.NonProceedingMethod ();

      Assert.That (_instance.NonProceedingMethodCalled, Is.False);
    }

    [Test]
    public void Proceed_ProceedingAspect ()
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
      var fieldInfo = _type.GetField ("_aspects_NonProceedingMethod", BindingFlags.NonPublic | BindingFlags.Instance);
      Debug.Assert (fieldInfo != null, "fieldInfo != null");
      var field = (Aspect[]) fieldInfo.GetValue (_instance);

      Assert.That (field[0], Is.TypeOf<NonProceedingAspect>());
    }

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

    public class DomainType
    {

      public bool NonProceedingMethodCalled { get; private set; }
      [NonProceedingAspect]
      public virtual void NonProceedingMethod () { NonProceedingMethodCalled = true; }

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

      [Add10ToArgAspect(Priority = 1)]
      [ProceedingAspect(Priority = 2)]
      public virtual int ProceedAddMethod (int i) { return i; }

      [Add10ToArgAspect(Priority = 2)]
      [ProceedingAspect (Priority = 1)]
      public virtual int AddProceedMethod (int i) { return i; }
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
  }
}