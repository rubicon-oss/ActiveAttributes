using System;
using System.Linq;
using ActiveAttributes.Core;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class LoggingAspectsTest
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
    public void SimpleLogging ()
    {
      _instance.SimpleLoggedMethod();

      Assert.That (_target.Counter, Is.EqualTo (2));
      Assert.That (_target.LastMessage, Is.EqualTo ("Exit SimpleLoggedMethod"));
    }

    [Test]
    public void AdvancedLogging ()
    {
      _instance.AdvancedLoggedMethod ("test", 19);

      Assert.That (_target.LastMessage,
        Is.EqualTo ("ActiveAttributes.IntegrationTests.LoggingAspectsTest+DomainType::AdvancedLoggedMethod({test}, {19})"));
    }


    public class DomainType
    {
      [LoggingAspect]
      public virtual void SimpleLoggedMethod ()
      {
      }

      [AdvancedLoggingAspect]
      public virtual void AdvancedLoggedMethod (string str, int i)
      {
      }
    }

    public class LoggingAspectAttribute : MethodBoundaryAspectAttribute
    {
      private Logger _logger = LogManager.GetCurrentClassLogger();

      public override void OnEntry (Invocation invocation)
      {
        _logger.Debug ("Entry " + invocation.Method.Name);
      }

      public override void OnExit (Invocation invocation)
      {
        _logger.Debug ("Exit " + invocation.Method.Name);
      }
    }

    public class AdvancedLoggingAspectAttribute : MethodBoundaryAspectAttribute
    {
      private Logger _logger = LogManager.GetCurrentClassLogger();

      public override void OnEntry (Invocation invocation)
      {
        _logger.Debug (
            "{0}::{1}({2})",
            invocation.Method.DeclaringType,
            invocation.Method.Name,
            string.Join (", ", invocation.Arguments.Select (x => "{" + x.ToString() + "}").ToArray()));
      }
    }
  }
}