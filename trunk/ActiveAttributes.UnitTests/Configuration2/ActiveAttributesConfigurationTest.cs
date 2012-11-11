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
using ActiveAttributes.Configuration2;
using ActiveAttributes.Discovery.DeclarationProviders;
using ActiveAttributes.Ordering;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Configuration2
{
  [TestFixture]
  public class ActiveAttributesConfigurationTest
  {
    [Test]
    public void Initialization ()
    {
      var configuration = new ActiveAttributesConfiguration();

      Assert.That (configuration.DeclarationProviders, Is.TypeOf<List<IDeclarationProvider>>());
      Assert.That (configuration.Orderings, Is.TypeOf<List<AdviceOrderingBase>>());
    }

    [Test]
    public void Lock ()
    {
      var configuration = new ActiveAttributesConfiguration();
      configuration.Lock();

      Assert.That (configuration.IsLocked, Is.True);
      Assert.That (configuration.DeclarationProviders, Is.TypeOf<ReadOnlyCollection<IDeclarationProvider>>());
      Assert.That (configuration.Orderings, Is.TypeOf<ReadOnlyCollection<AdviceOrderingBase>>());
    }

    [Test]
    public void ThrowsForMultipleLock ()
    {
      var configuration = new ActiveAttributesConfiguration();
      configuration.Lock();
      Assert.That (() => configuration.Lock(), Throws.InvalidOperationException.With.Message.EqualTo ("Configuration is already locked."));
    }
  }
}