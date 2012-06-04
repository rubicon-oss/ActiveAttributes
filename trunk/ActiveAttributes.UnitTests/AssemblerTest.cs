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
    [Test]
    public void TipePipeTest ()
    {
      var type = AssembleType<DomainType> (
          mutableType =>
          {
            var method = mutableType.AllMutableMethods.First ();
            method.SetBody (
                ctx =>
                Expression.Constant (1));
          });

      var instance = (DomainType) Activator.CreateInstance (type);
      var result = instance.ReturnMethod ();

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Test2 ()
    {
      var type = AssembleType<DomainType> (new Assembler().ModifyType);

      var instance = (DomainType) Activator.CreateInstance (type);
      var result = instance.ReturnMethod ();

      Assert.That (result, Is.EqualTo (2));
    }

    [Test]
    public void name ()
    {
      var obj = new DomainType();
      var exp = Expression.Lambda (Expression.Call (Expression.Constant (obj), typeof (DomainType).GetMethods().First(), null));
      exp.Compile();
    }


    public class DomainType
    {
      public virtual int ReturnMethod ()
      {
        return 2;
      }
    }
  }
}