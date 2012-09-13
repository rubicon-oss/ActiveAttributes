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
using System.Collections.Generic;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Assembly.Configuration.Configurators;
using ActiveAttributes.Core.Assembly.Configuration.Rules;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly.Configurators
{
  [TestFixture]
  public class PresetAspectConfiguratorTest
  {
    private IAspectConfiguration _configuration;
    private IList<IOrderRule> _orderRules;
    private IDictionary<Type, string> _roles;
    private System.Reflection.Assembly _assembly;

    [SetUp]
    public void SetUp ()
    {
      _assembly = System.Reflection.Assembly.GetExecutingAssembly();
      _configuration = MockRepository.GenerateMock<IAspectConfiguration>();
      _orderRules = MockRepository.GenerateMock<IList<IOrderRule>>();
      _roles = MockRepository.GenerateMock<IDictionary<Type, string>>();

      _configuration.Expect (x => x.Rules).Return (_orderRules);
      _configuration.Expect (x => x.Roles).Return (_roles);

      string outRole;
      _roles.Expect (x => x.TryGetValue (typeof (DomainAspect1Attribute), out outRole)).Return (true).OutRef ("TestRole");
    }

    [Test]
    public void TypeOrderRules ()
    {
      new PresetAspectConfigurator (_assembly).Initialize (_configuration);

      var orderRule1 = new TypeOrderRule ("DomainAspect1Attribute", typeof (DomainAspect1Attribute), typeof (DomainAspect2Attribute));
      var orderRule2 = new TypeOrderRule ("DomainAspect1Attribute", typeof (DomainAspect3Attribute), typeof (DomainAspect1Attribute));
      _orderRules.AssertWasCalled (x => x.Add (orderRule1));
      _orderRules.AssertWasCalled (x => x.Add (orderRule2));
    }

    [Test]
    public void Role ()
    {
      new PresetAspectConfigurator (_assembly).Initialize (_configuration);

      _roles.AssertWasCalled (x => x.Add (typeof (DomainAspect1Attribute), "TestRole"));
    }

    [Test]
    public void RoleOrderRules ()
    {
      new PresetAspectConfigurator (_assembly).Initialize (_configuration);

      var orderRule1 = new RoleOrderRule ("DomainAspect1Attribute", "TestRole", "Role2", _configuration);
      var orderRule2 = new RoleOrderRule ("DomainAspect1Attribute", "Role3", "TestRole", _configuration);
      _orderRules.AssertWasCalled (x => x.Add (orderRule1));
      _orderRules.AssertWasCalled (x => x.Add (orderRule2));
    }

    [AspectRole ("TestRole")]
    [AspectOrdering (OrderPosition.Before, typeof (DomainAspect2Attribute))]
    [AspectOrdering (OrderPosition.After, typeof (DomainAspect3Attribute))]
    [AspectOrdering (OrderPosition.Before, "Role2")]
    [AspectOrdering (OrderPosition.After, "Role3")]
    private class DomainAspect1Attribute : AspectAttribute { }

    private class DomainAspect2Attribute : AspectAttribute { }

    private class DomainAspect3Attribute : AspectAttribute { }
  }
}