using System;
using System.Reflection;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Contexts.ArgumentCollection;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Collections;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class TypedAspectTest
  {
    [Test]
    public void InterceptTyped ()
    {
      var instance = ObjectFactory.Create<DomainType>();
      var arg0 = new DomainType.NestedDomainType { Name = "Fabian" };

      var result = instance.Method (arg0);

      Assert.AreEqual (result, "Stefan");
    }

    public class DomainType
    {
      public class NestedDomainType
      {
        public string Name { get; set; }
      }

      [DomainAspect]
      public virtual string Method (NestedDomainType obj)
      {
        return obj.Name;
      }
    }

    public class DomainAspectAttribute : MethodInterceptionAspectAttribute
    {
      public override bool Validate (MethodInfo method)
      {
        var parameters = method.GetParameters();

        if (parameters.Length != 1)
          return false;

        if (parameters[0].ParameterType != typeof (DomainType.NestedDomainType))
          return false;

        if (method.ReturnType != typeof (string))
          return false;

        return true;
      }

      public override void OnIntercept (IInvocation invocation)
      {
        var funcInvocationContext = (FuncInvocationContext<DomainType, DomainType.NestedDomainType, string>) invocation.Context;

        var arg0 = funcInvocationContext.Arg0;

        arg0.Name = "Stefan";

        invocation.Proceed();
      }
    }
  }
}