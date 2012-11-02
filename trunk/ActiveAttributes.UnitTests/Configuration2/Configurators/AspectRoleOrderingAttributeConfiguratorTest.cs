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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Configuration2.Configurators;
using ActiveAttributes.Core.Configuration2.CustomAttributes;
using ActiveAttributes.Core.Infrastructure.Orderings;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Configuration2.Configurators
{
  [TestFixture]
  public class AspectRoleOrderingAttributeConfiguratorTest
  {
    private IActiveAttributesConfiguration _activeAttributesConfigurationStub;
    private IList<IAdviceOrdering> _aspectOrderingRulesMock;
    private IDictionary<Type, string> _aspectRolesMock;

    [SetUp]
    public void SetUp ()
    {
      _activeAttributesConfigurationStub = MockRepository.GenerateStub<IActiveAttributesConfiguration> ();
      _aspectOrderingRulesMock = MockRepository.GenerateMock<IList<IAdviceOrdering>> ();
      _aspectRolesMock = MockRepository.GenerateStrictMock<IDictionary<Type, string>> ();
      _activeAttributesConfigurationStub
          .Stub (x => x.AspectOrderingRules)
          .Return (_aspectOrderingRulesMock);
      _activeAttributesConfigurationStub
          .Stub (x => x.AspectRoles)
          .Return (_aspectRolesMock);
    }

    [Test]
    public void ConvertsAttributesToRules ()
    {
      var aspectType = typeof (Domain1AspectAttribute);
      string aspectRole;
      _aspectRolesMock
          .Expect (x => x.TryGetValue (aspectType, out aspectRole))
          .Return (true)
          .OutRef ("Role0");

      new AspectRoleOrderingAttributeConfigurator (new[] { aspectType }).Initialize (_activeAttributesConfigurationStub);

      var expectedRule1 = new RoleOrdering ("AspectTypeOrderingAttributeConfigurator", "Role0", "Role1", _activeAttributesConfigurationStub);
      var expectedRule2 = new RoleOrdering ("AspectTypeOrderingAttributeConfigurator", "Role0", "Role2", _activeAttributesConfigurationStub);
      _aspectOrderingRulesMock.AssertWasCalled (x => x.Add (expectedRule1));
      _aspectOrderingRulesMock.AssertWasCalled (x => x.Add (expectedRule2));
    }

    [Test]
    public void RespectsOrderPosition ()
    {
      var aspectType = typeof (Domain2AspectAttribute);
      string aspectRole;
      _aspectRolesMock
          .Expect (x => x.TryGetValue (aspectType, out aspectRole))
          .Return (true)
          .OutRef ("Role0");

      new AspectRoleOrderingAttributeConfigurator (new[] { aspectType }).Initialize (_activeAttributesConfigurationStub);

      var expectedRule = new RoleOrdering ("AspectTypeOrderingAttributeConfigurator", "Role1", "Role0", _activeAttributesConfigurationStub);
      _aspectOrderingRulesMock.AssertWasCalled (x => x.Add (expectedRule));
    }

    [Test]
    [ExpectedException(ExpectedMessage = "TODO (low) exception text")]
    public void ThrowsForAspectsWithoutRole ()
    {
      var aspectType = typeof (Domain2AspectAttribute);
      string aspectRole;
      _aspectRolesMock
          .Expect (x => x.TryGetValue (aspectType, out aspectRole))
          .Return (false);

      new AspectRoleOrderingAttributeConfigurator (new[] { aspectType }).Initialize (_activeAttributesConfigurationStub);
    }

    [AspectRoleOrdering (Position.Before, "Role1", "Role2")]
    class Domain1AspectAttribute : AspectAttribute {}

    [AspectRoleOrdering (Position.After, "Role1")]
    class Domain2AspectAttribute : AspectAttribute { }
  }
}