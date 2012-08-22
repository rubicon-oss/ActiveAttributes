//Sample license text.

using System;
using System.Collections.Generic;
using System.Linq;
using ActiveAttributes.Common.Validation;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.Common.UnitTests
{
  [TestFixture]
  public class ValidationAspectAttributeTest
  {
    [SetUp]
    public void SetUp ()
    {
      Validator.Invocations = new List<Invocation>();
    }
    [Test]
    public void Validate ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method ((null)));
      var invocationContext = new FuncInvocationContext<DomainClass, object, object> (methodInfo, null, 11);
      var invocation = new FuncInvocation<DomainClass, object, object> (invocationContext, x => (int) x + 1);
      var validationAspect = new ValidationAspectAttribute();

      validationAspect.OnIntercept (invocation);

      var invocations = Validator.Invocations.ToArray();
      Assert.That (invocations, Has.Length.EqualTo (2));
      Assert.That (invocations[0].ParamName, Is.EqualTo ("obj"));
      Assert.That (invocations[0].Object, Is.EqualTo (11));
      Assert.That (invocations[1].ParamName, Is.EqualTo ("return"));
      Assert.That (invocations[1].Object, Is.EqualTo (12));
    }

    [Test]
    public void ValidateVoid ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method2 ((null)));
      var invocationContext = new ActionInvocationContext<DomainClass, object> (methodInfo, null, 11);
      var invocation = new ActionInvocation<DomainClass, object> (invocationContext, x => { });
      var validationAspect = new ValidationAspectAttribute ();

      validationAspect.OnIntercept (invocation);

      var invocations = Validator.Invocations.ToArray ();
      Assert.That (invocations, Has.Length.EqualTo (1));
      Assert.That (invocations[0].ParamName, Is.EqualTo ("obj"));
      Assert.That (invocations[0].Object, Is.EqualTo (11));
    }

    [Test]
    public void ValidateNotNullFirst ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method3 ((null)));
      var invocationContext = new ActionInvocationContext<DomainClass, object> (methodInfo, null, 1);
      var invocation = new ActionInvocation<DomainClass, object> (invocationContext, x => { });
      var validationAspect = new ValidationAspectAttribute ();

      validationAspect.OnIntercept (invocation);

      Assert.That (Validator.Invocations.ToArray(), Has.Length.EqualTo (2));
    }

    [Test]
    public void ValidateNotNullFirstThrows ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method3 ((null)));
      var invocationContext = new ActionInvocationContext<DomainClass, object> (methodInfo, null, null);
      var invocation = new ActionInvocation<DomainClass, object> (invocationContext, x => { });
      var validationAspect = new ValidationAspectAttribute ();

      Assert.That (() => validationAspect.OnIntercept (invocation), Throws.TypeOf<ArgumentNullException>());
    }

    private class DomainClass
    {
      [return: Validator]
      public object Method ([Validator] object obj)
      {
        return 12;
      }

      public void Method2 ([Validator] object obj)
      {
      }

      public void Method3 ([Validator, NotNull, Validator] object obj)
      {
      }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
    private class Validator : ValidatorBase
    {
      public override void Validate (string paramName, object obj)
      {
        var type = obj.GetType();
        Invocations.Add (new Invocation { ParamName = paramName, Object = obj });
      }

      public static IList<Invocation> Invocations { get; set; }
    }

    private class Invocation
    {
      public string ParamName;
      public object Object;
    }
  }
}