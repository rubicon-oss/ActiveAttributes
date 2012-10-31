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

using System;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Interception.Invocations;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class MethodBoundaryTest : TestBase
  {
    private DebugTarget _target;
    private DomainType _instance;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _target = new DebugTarget ();
      _target.Layout = new SimpleLayout ("${message}");
      var rule = new LoggingRule ("*", LogLevel.Trace, _target);

      LogManager.Configuration = new LoggingConfiguration ();
      LogManager.Configuration.AddTarget ("target", _target);
      LogManager.Configuration.LoggingRules.Add (rule);
      LogManager.Configuration.Reload ();
      LogManager.ReconfigExistingLoggers ();

      _instance = ObjectFactory.Create<DomainType>();
    }

    [Test]
    public void OnEntry ()
    {
      _instance.OnEntryMethod ();

      Assert.That (_target.LastMessage, Is.EqualTo ("Entered OnEntryMethod"));
    }

    [Test]
    public void OnExit ()
    {
      _instance.OnExitMethod ();

      Assert.That (_target.LastMessage, Is.EqualTo ("Exited OnExitMethod"));
    }

    [Test]
    public void OnSuccess ()
    {
      _instance.OnSuccessMethod ();

      Assert.That (_target.LastMessage, Is.EqualTo ("Succeeded OnSuccessMethod with 10"));
    }

    [Test]
    public void OnException ()
    {
      try
      {
        _instance.OnExceptionMethod ();
      }
      catch (Exception)
      {
      }

      Assert.That (_target.LastMessage, Is.StringStarting ("OnExceptionMethod has thrown an System.Exception: test"));
    }

    public class DomainType
    {
      [OnEntryAspect]
      public virtual void OnEntryMethod () { }

      [OnExitAspect]
      public virtual void OnExitMethod () { }

      [OnSuccessAspect]
      public virtual int OnSuccessMethod () { return 10; }

      [OnExceptionAspect]
      public virtual void OnExceptionMethod () { throw new Exception ("test"); }
    }

    public class BaseAspectAttribute : MethodBoundaryAspectAttribute
    {
      protected readonly Logger Logger = LogManager.GetCurrentClassLogger ();
    }

    public class OnEntryAspectAttribute : BaseAspectAttribute
    {
      protected override void OnEntry (IReadOnlyInvocation invocationInfo)
      {
        Logger.Trace ("Entered " + invocationInfo.Context.MethodInfo.Name);
      }
    }

    public class OnExitAspectAttribute : BaseAspectAttribute
    {
      protected override void OnExit (IReadOnlyInvocation invocationInfo)
      {
        Logger.Trace ("Exited " +invocationInfo.Context.MethodInfo.Name);
      }
    }

    public class OnSuccessAspectAttribute : BaseAspectAttribute
    {
      protected override void OnSuccess (IReadOnlyInvocation invocationInfo, object returnValue)
      {
        Logger.Info ("Succeeded " + invocationInfo.Context.MethodInfo.Name + " with " + returnValue);
      }
    }

    public class OnExceptionAspectAttribute : BaseAspectAttribute
    {
      protected override void OnException (IReadOnlyInvocation invocationInfo, Exception exception)
      {
        Logger.Warn (invocationInfo.Context.MethodInfo.Name + " has thrown an " + exception);
      }
    }
  }
}