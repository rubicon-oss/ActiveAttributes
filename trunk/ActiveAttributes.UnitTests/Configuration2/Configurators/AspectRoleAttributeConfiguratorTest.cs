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
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Configuration2.Configurators
{
  [TestFixture]
  public class AspectRoleAttributeConfiguratorTest
  {
    [Test]
    public void ConvertsAttributeToRole ()
    {
      var activeAttributesConfigurationStub = MockRepository.GenerateStub<IActiveAttributesConfiguration> ();
      var aspectRolesMock = MockRepository.GenerateMock<IDictionary<Type, string>> ();
      activeAttributesConfigurationStub
          .Stub (x => x.AspectRoles)
          .Return (aspectRolesMock);

      var aspectType = typeof (Domain1AspectAttribute);
      var roleAttributeConfigurator = new AspectRoleAttributeConfigurator (new[] { aspectType });
      roleAttributeConfigurator.Initialize (activeAttributesConfigurationStub);

      aspectRolesMock.AssertWasCalled (x => x.Add (aspectType, "Role1"));
    }

    [AspectRole("Role1")]
    class Domain1AspectAttribute : AspectAttribute { }
  }
}