using System;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

using NUnit.Framework;

using Remotion.FunctionalProgramming;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class TraceAspectTest
  {
    private DebugTarget _target;
    private DomainType _instance;

    [SetUp]
    public void SetUp ()
    {
      _target = new DebugTarget();
      _target.Layout = new SimpleLayout ("${message}");
      var rule = new LoggingRule ("*", LogLevel.Trace, _target);

      LogManager.Configuration = new LoggingConfiguration();
      LogManager.Configuration.AddTarget ("target", _target);
      LogManager.Configuration.LoggingRules.Add (rule);
      LogManager.Configuration.Reload();
      LogManager.ReconfigExistingLoggers();

      _instance = ObjectFactory.Create<DomainType>();
    }

    [Test]
    public void Trace ()
    {
      _instance.Method("muh", 2);

      Assert.That (_target.Counter, Is.EqualTo (1));
      Assert.That (_target.LastMessage, Is.EqualTo ("Entry DomainType.Method(str={muh}, i={2})"));
    }


    public class DomainType
    {
      [TraceAspect]
      public virtual void Method (string str, int i) {}
    }

    public class TraceAspectAttribute : MethodBoundaryAspectAttribute
    {
      private Logger _logger = LogManager.GetCurrentClassLogger();

      protected override void OnEntry (IReadOnlyInvocation invocation)
      {
        var ctx = invocation.Context;
        var parameterArgumentList = ctx.MethodInfo.GetParameters()
            .Zip (ctx.Arguments, (pi, arg) => pi.Name + "={" + arg.ToString() + "}");

        var callInformation = string.Format ("{0}.{1}({2})",
                                             ctx.MethodInfo.DeclaringType.Name,
                                             ctx.MethodInfo.Name,
                                             string.Join (", ", parameterArgumentList.ToArray()));

        _logger.Debug ("Entry " + callInformation);
      }
    }
  }
}