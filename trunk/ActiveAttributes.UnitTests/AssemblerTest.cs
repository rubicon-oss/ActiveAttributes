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
    public void Proceed_ProceedingAspect ()
    {
      _instance.Method1 ();

      Assert.That (_instance.Method1Called, Is.True);
    }

    [Test]
    public void Proceed_NonProceedingAspect ()
    {
      _instance.Method2 ();

      Assert.That (_instance.Method2Called, Is.False);
    }

    [Test, Ignore]
    public void Proceed_ModifiedArgument ()
    {
      //_instance.Method3 ("a");

      Assert.That (_instance.Method3Argument, Is.EqualTo ("a_modified"));
    }

    public class DomainType
    {
      public bool Method1Called { get; private set; }
      public bool Method2Called { get; private set; }
      public string Method3Argument { get; private set; }

      [ProceedingAspect]
      public virtual void Method1 () { Method1Called = true; }

      [NonProceedingAspect]
      public virtual void Method2 () { Method2Called = true; }

      //[ModifyArgumentAspect]
      //public virtual void Method3 (string a) { Method3Argument = a; }
    }

    public class ProceedingAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        Assert.That (invocation.Arguments, Is.Empty);
        invocation.Proceed();
      }
    }

    public class NonProceedingAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
      }
    }

    public class ModifyArgumentAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.Arguments[0] = (string) invocation.Arguments[0] + "_modified";
      }
    }
  }
}