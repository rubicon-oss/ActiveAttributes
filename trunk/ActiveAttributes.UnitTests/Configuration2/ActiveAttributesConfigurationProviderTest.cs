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
using System.Linq;
using ActiveAttributes.Configuration2;
using ActiveAttributes.Configuration2.Configurators;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Configuration2
{
  [TestFixture]
  public class ActiveAttributesConfigurationProviderTest
  {
    [Test]
    public void CallsConfigurators ()
    {
      var configuratorMock = MockRepository.GenerateMock<IActiveAttributesConfigurator>();
      SetupServiceLocator (configuratorMock);

      var result = new ActiveAttributesConfigurationProvider().GetConfiguration();

      configuratorMock.AssertWasCalled (x => x.Initialize (result));
      Assert.That (result, Is.TypeOf<ActiveAttributesConfiguration>().And.Not.Null);
    }

    [Test]
    public void CallsAppConfigConfiguratorFirst ()
    {
      var mockRepository = new MockRepository();
      var firstConfiguratorMock = mockRepository.StrictMock<IActiveAttributesConfigurator>();
      var appConfigConfiguratorMock = mockRepository.StrictMock<AppConfigConfigurator>();
      var lastConfiguratorMock = mockRepository.StrictMock<IActiveAttributesConfigurator>();
      SetupServiceLocator (firstConfiguratorMock, appConfigConfiguratorMock, lastConfiguratorMock);

      using (mockRepository.Ordered())
      {
        appConfigConfiguratorMock.Expect (x => x.Initialize (null)).IgnoreArguments();
        firstConfiguratorMock.Expect (x => x.Initialize (null)).IgnoreArguments();
        lastConfiguratorMock.Expect (x => x.Initialize (null)).IgnoreArguments();
      }
      mockRepository.ReplayAll();

      var configurationProvider = new ActiveAttributesConfigurationProvider();
      configurationProvider.GetConfiguration();

      mockRepository.VerifyAll();
    }

    [Test]
    public void DontConfigureIfLocked ()
    {
      var firstConfiguratorMock = MockRepository.GenerateStrictMock<IActiveAttributesConfigurator>();
      var secondConfiguratorMock = MockRepository.GenerateStrictMock<IActiveAttributesConfigurator>();
      SetupServiceLocator (firstConfiguratorMock, secondConfiguratorMock);

      firstConfiguratorMock
          .Expect (x => x.Initialize (Arg<IActiveAttributesConfiguration>.Is.Anything))
          .WhenCalled (x => ((IActiveAttributesConfiguration) x.Arguments[0]).Lock());

      Assert.That (() => new ActiveAttributesConfigurationProvider().GetConfiguration(), Throws.Nothing);
      firstConfiguratorMock.VerifyAllExpectations();
    }

    private void SetupServiceLocator (params IActiveAttributesConfigurator[] configurators)
    {
      var locator = new DefaultServiceLocator();
      locator.Register (typeof (IActiveAttributesConfigurator), configurators.Select (x => new Func<object> (() => x)));
      ServiceLocator.SetLocatorProvider (() => locator);
    }
  }
}