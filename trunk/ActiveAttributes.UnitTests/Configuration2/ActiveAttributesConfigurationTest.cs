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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Infrastructure.Orderings;
using NUnit.Framework;
using Remotion.Collections;

namespace ActiveAttributes.UnitTests.Configuration2
{
  [TestFixture]
  public class ActiveAttributesConfigurationTest
  {
    [Test]
    public void Initialization ()
    {
      var configuration = new ActiveAttributesConfiguration ();

      //Assert.That (configuration.AspectDescriptorProviders, Is.TypeOf<List<IAspectDescriptorProvider>>());
      Assert.That (configuration.AspectOrderingRules, Is.TypeOf<List<IAdviceOrdering>> ());
      Assert.That (configuration.AspectRoles, Is.TypeOf<Dictionary<Type, string>> ());
    }

    [Test]
    public void Lock ()
    {
      var configuration = new ActiveAttributesConfiguration();
      configuration.Lock();

      Assert.That (configuration.IsLocked, Is.True);
      //Assert.That (configuration.AspectDescriptorProviders, Is.TypeOf<ReadOnlyCollection<IAspectDescriptorProvider>> ());
      Assert.That (configuration.AspectOrderingRules, Is.TypeOf<ReadOnlyCollection<IAdviceOrdering>> ());
      Assert.That (configuration.AspectRoles, Is.TypeOf<ReadOnlyDictionary<Type, string>> ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Configuration is already locked.")]
    public void ThrowsForMultipleLock ()
    {
      var configuration = new ActiveAttributesConfiguration ();
      configuration.Lock ();
      configuration.Lock ();
    }
  }
}