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
    public void Proceed_ModifyArguments ()
    {
      SkipDeletion();

      _instance.ModifyArgumentsMethod ("a");

      Assert.That (_instance.ModifyArgumentMethodArgs, Is.EqualTo ("a_modified"));
    }

    [Test]
    public void Proceed_ModifyReturnValue ()
    {
      var result = _instance.ModifyReturnValueMethod();

      Assert.That (result, Is.EqualTo ("aspect"));
    }

    [Test]
    public void name ()
    {
      var type = AssembleType<DomainType> (
          mt =>
          {
            var method = mt.AllMutableMethods.Where (x => x.Name == "ModifyReturnValueMethod").Single();
            method.SetBody (
                ctx =>
                Expression.Constant ("bla", typeof (string)));
          });

      var obj = (DomainType) Activator.CreateInstance (type);
      var result = obj.ModifyReturnValueMethod();
      Assert.That (result, Is.EqualTo ("bla"));
    }

    public class DomainType
    {
      public bool NonProceedingMethodCalled { get; private set; }
      public bool ProceedingMethodCalled { get; private set; }
      public string ModifyArgumentMethodArgs { get; private set; }

      [NonProceedingAspect]
      public virtual void NonProceedingMethod () { NonProceedingMethodCalled = true; }

      [ProceedingAspect]
      public virtual void ProceedingMethod () { ProceedingMethodCalled = true; }

      [ModifyArgumentsAspect]
      public virtual void ModifyArgumentsMethod (string a) { ModifyArgumentMethodArgs = a; }

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

    public class ModifyArgumentsAspect : Aspect
    {
      public override void OnInvoke (Invocation invocation)
      {
        invocation.Arguments[0] = (string) invocation.Arguments[0] + "_modified";
        invocation.Proceed();
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