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
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Configuration2.Configurators;
using ActiveAttributes.Core.Configuration2.CustomAttributes;
using ActiveAttributes.Core.Configuration2.Rules;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Configuration2.Configurators
{
  [TestFixture]
  public class AspectTypeOrderingAttributeConfiguratorTest
  {
    [Test]
    public void ConvertsAttributesToRules ()
    {
      var activeAttributesConfigurationStub = MockRepository.GenerateStub<IActiveAttributesConfiguration>();
      var aspectOrderingRulesMock = MockRepository.GenerateMock<IList<IAspectOrderingRule>>();
      activeAttributesConfigurationStub
          .Stub (x => x.AspectOrderingRules)
          .Return (aspectOrderingRulesMock);

      var aspectType = typeof (Domain1AspectAttribute);
      new AspectTypeOrderingAttributeConfigurator (new[] { aspectType }).Initialize (activeAttributesConfigurationStub);

      var expectedRule1 = new TypeOrderingRule (
          "AspectTypeOrderingAttributeConfigurator", typeof (Domain1AspectAttribute), typeof (Domain2AspectAttribute));
      var expectedRule2 = new TypeOrderingRule (
          "AspectTypeOrderingAttributeConfigurator", typeof (Domain1AspectAttribute), typeof (Domain3AspectAttribute));
      aspectOrderingRulesMock.AssertWasCalled (x => x.Add (expectedRule1));
      aspectOrderingRulesMock.AssertWasCalled (x => x.Add (expectedRule2));
    }

    [Test]
    public void RespectsOrderPosition ()
    {
      var activeAttributesConfigurationStub = MockRepository.GenerateStub<IActiveAttributesConfiguration> ();
      var aspectOrderingRulesMock = MockRepository.GenerateMock<IList<IAspectOrderingRule>> ();
      activeAttributesConfigurationStub
          .Stub (x => x.AspectOrderingRules)
          .Return (aspectOrderingRulesMock);

      var aspectType = typeof (Domain2AspectAttribute);
      new AspectTypeOrderingAttributeConfigurator (new[] { aspectType }).Initialize (activeAttributesConfigurationStub);

      var expectedRule = new TypeOrderingRule (
          "AspectTypeOrderingAttributeConfigurator", typeof (Domain1AspectAttribute), typeof (Domain2AspectAttribute));
      aspectOrderingRulesMock.AssertWasCalled (x => x.Add (expectedRule));
      
    }

    [AspectTypeOrdering(Position.Before, typeof(Domain2AspectAttribute), typeof(Domain3AspectAttribute))]
    class Domain1AspectAttribute : AspectAttribute {}
    
    [AspectTypeOrdering (Position.After, typeof(Domain1AspectAttribute))]
    class Domain2AspectAttribute : AspectAttribute {}

    class Domain3AspectAttribute : AspectAttribute {}
  }
}