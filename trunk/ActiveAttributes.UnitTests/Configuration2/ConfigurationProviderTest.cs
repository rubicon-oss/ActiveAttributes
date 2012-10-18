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
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Configuration2.Configurators;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Checked
{
  [TestFixture]
  public class ConfigurationProviderTest
  {
    [Test]
    public void CallsConfigurators ()
    {
      var configuratorMock = MockRepository.GenerateMock<IActiveAttributesConfigurator>();
      var configurationProvider = new ActiveAttributeConfigurationProvider (new[] { configuratorMock });

      var result = configurationProvider.GetConfiguration();

      configuratorMock.AssertWasCalled (x => x.Initialize (result));
    }

    [Test]
    public void CallsAppConfigConfiguratorFirst ()
    {
      var mockRepository = new MockRepository();
      var appConfigConfiguratorMock = mockRepository.StrictMock<AppConfigConfigurator>();
      var otherConfiguratorMock = mockRepository.StrictMock<IActiveAttributesConfigurator>();

      // Arrange
      using (mockRepository.Ordered())
      {
        appConfigConfiguratorMock
            .Expect (x => x.Initialize (null))
            .IgnoreArguments();
        otherConfiguratorMock
            .Expect (x => x.Initialize (null))
            .IgnoreArguments();
      }
      mockRepository.ReplayAll();

      // Act
      var configurationProvider = new ActiveAttributeConfigurationProvider (new[] { otherConfiguratorMock, appConfigConfiguratorMock });
      configurationProvider.GetConfiguration();

      // Assert
      mockRepository.VerifyAll();
    }

    [Test]
    public void CallOnlyIfConfigIsNotLocked ()
    {
      var firstConfiguratorMock = MockRepository.GenerateStrictMock<IActiveAttributesConfigurator>();
      var secondConfiguratorMock = MockRepository.GenerateStrictMock<IActiveAttributesConfigurator>();

      firstConfiguratorMock
          .Expect (x => x.Initialize (null))
          .IgnoreArguments()
          .WhenCalled (x => ((IActiveAttributesConfiguration) x.Arguments[0]).Lock());

      var configurationProvider = new ActiveAttributeConfigurationProvider (new[] { firstConfiguratorMock, secondConfiguratorMock });
      configurationProvider.GetConfiguration();
    }
  }
}