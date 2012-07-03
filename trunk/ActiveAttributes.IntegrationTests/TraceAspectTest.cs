// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Diagnostics;
using System.Linq;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using Remotion.Logging;
using LogLevel = NLog.LogLevel;
using LogManager = NLog.LogManager;

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
      _instance.Method ("muh", 2);

      Assert.That (_target.Counter, Is.EqualTo (1));
      Assert.That (_target.LastMessage, Is.EqualTo ("Entry DomainType.Method(str={muh}, i={2})"));
    }


    public class DomainType
    {
      [TraceAspect]
      public virtual void Method (string str, int i)
      {
      }
    }


    public class TraceAspectAttribute : MethodBoundaryAspectAttribute
    {
      private readonly Logger _logger = LogManager.GetCurrentClassLogger();

      protected override void OnEntry (IReadOnlyInvocation invocation)
      {
        var ctx = invocation.Context;
        var parameterArgumentList = ctx.MethodInfo.GetParameters()
            .Zip (ctx.Arguments, (pi, arg) => pi.Name + "={" + arg.ToString() + "}");

        var callInformation = string.Format (
            "{0}.{1}({2})",
            ctx.MethodInfo.DeclaringType.Name,
            ctx.MethodInfo.Name,
            string.Join (", ", parameterArgumentList.ToArray()));

        _logger.Debug ("Entry " + callInformation);
      }
    }
  }
}