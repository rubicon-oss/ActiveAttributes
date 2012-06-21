using System;
using System.Reflection;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;

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
      public string Method (NestedDomainType obj)
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
        var typedInvocation = (FuncInvocation<object, DomainType.NestedDomainType, string>) invocation;

        DomainType.NestedDomainType arg0 = typedInvocation.Context.Arg0;

        arg0.Name = "Stefan";
      }
    }
  }
}