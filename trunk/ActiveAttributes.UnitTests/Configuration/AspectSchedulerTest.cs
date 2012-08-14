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
using System.Runtime.CompilerServices;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using NUnit.Framework;
using Remotion.Collections;

namespace ActiveAttributes.UnitTests.Configuration
{
  [TestFixture]
  public class AspectSchedulerTest
  {
    private AspectScheduler _scheduler;

    [SetUp]
    public void SetUp ()
    {
      _scheduler = new AspectScheduler(null);
    }

    //[Test]
    //public void AddRole ()
    //{
    //  _scheduler.AddAspectRole (typeof (Aspect1), "Role1");

    //  var roles = _scheduler._aspectRoles[typeof (Aspect1)];

    //  Assert.That (roles, Is.EqualTo (new[] { "Role1" }));
    //}

    //[Test]
    //public void AddRoles ()
    //{
    //  _scheduler.AddAspectRole (typeof (Aspect1), "Role1");
    //  _scheduler.AddAspectRole (typeof (Aspect1), "Role2");

    //  var roles = _scheduler._aspectRoles[typeof (Aspect1)];

    //  Assert.That (roles, Is.EqualTo (new[] { "Role1", "Role2" }));
    //}

    //[Test]
    //public void AddRolesMultipleTypes ()
    //{
    //  _scheduler.AddAspectRole (typeof (Aspect1), "Role1");
    //  _scheduler.AddAspectRole (typeof (Aspect1), "Role2");
    //  _scheduler.AddAspectRole (typeof (Aspect2), "Role2");
    //  _scheduler.AddAspectRole (typeof (Aspect2), "Role1");

    //  var roles1 = _scheduler._aspectRoles[typeof (Aspect1)];
    //  var roles2 = _scheduler._aspectRoles[typeof (Aspect2)];

    //  Assert.That (roles1, Is.EqualTo (new[] { "Role1", "Role2" }));
    //  Assert.That (roles2, Is.EqualTo (new[] { "Role2", "Role1" }));
    //}

    //[Test]
    //public void AddRolesThrowsForNonAspectType ()
    //{
    //  Assert.That (() => _scheduler.AddAspectRole (typeof (CompilerGeneratedAttribute), "Role"), Throws.ArgumentException);
    //}

    //[Test]
    //public void AddRolesThrowsForDuplicatedRoles ()
    //{
    //  _scheduler.AddAspectRole (typeof (Aspect1), "Role");
    //  Assert.That (() => _scheduler.AddAspectRole (typeof (Aspect1), "Role"), Throws.InvalidOperationException);
    //}



    public class Aspect1 : AspectAttribute { }
    public class Aspect2 : AspectAttribute { }
    public class Aspect3 : AspectAttribute { }
  }
}