using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;
using TypePipe.IntegrationTests;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class AssemblerTest : TestBase
  {
    private DomainType _instance;

    [SetUp]
    public void SetUp ()
    {
      var assembler = new Assembler ();
      var type = AssembleType<DomainType> (assembler.ModifyType);
      _instance = (DomainType) Activator.CreateInstance (type);
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

      Assert.That (result, Is.EqualTo ("aspect"));
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
    }

    public class NonProceedingAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
      }
    }

    public class ProceedingAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        Assert.That (invocation.Arguments, Is.Empty);
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
        invocation.ReturnValue = "aspect";
      } 
    }
  }
}