using System;
using System.ComponentModel.Design;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Checked;
using ActiveAttributes.Core.Invocations;
using NLog.Internal;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.IntegrationTests
{
  public class CflowTest : TestBase
  {
    [Test]
    public void ConditionalCatch ()
    {
      var instance = ObjectFactory.Create<DomainType> ();

      Assert.That (() => instance.MethodWithInnerThrowing1(), Throws.Nothing);
      Assert.That (() => instance.MethodWithInnerThrowing2(), Throws.Exception);
    }

    public class DomainType
    {
      [CatchingAspect (ExecutionOf = "CflowTest\\+DomainType_Proxy1.MethodWithInnerThrowing1")]
      public virtual void MethodWithInnerThrowing1 ()
      {
        ThrowingMethod();
      }

      [CatchingAspect (ExecutionOf = "CflowTest\\+DomainType_Proxy1.MethodWithInnerThrowing1")]
      public virtual void MethodWithInnerThrowing2 ()
      {
        ThrowingMethod();
      }

      private void ThrowingMethod ()
      {
        throw new Exception();
      }
    }

    public class CatchingAspectAttribute : CflowMethodInterceptionAspectAttribute
    {
      protected override void OnCflowIntercept (IInvocation invocation)
      {
        try
        {
          invocation.Proceed();
        }
        catch (Exception)
        {
        }
      }
    }
  }
}