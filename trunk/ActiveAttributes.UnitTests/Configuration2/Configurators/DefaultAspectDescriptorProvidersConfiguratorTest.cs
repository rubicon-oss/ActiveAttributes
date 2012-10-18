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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Configuration2.AspectDescriptorProviders;
using ActiveAttributes.Core.Configuration2.Configurators;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Configuration2.Configurators
{
  [TestFixture]
  public class DefaultAspectDescriptorProvidersConfiguratorTest
  {
    [Test]
    public void AddsAllAspectDescriptorProviders ()
    {
      var activeAttributesConfigurationStub = MockRepository.GenerateStub<IActiveAttributesConfiguration>();
      var aspectDescriptorsMock = MockRepository.GenerateStrictMock<IList<IAspectDescriptorProvider>>();

      activeAttributesConfigurationStub
          .Stub (x => x.AspectDescriptorProviders)
          .Return (aspectDescriptorsMock);
      var expectedAspectDescriptorProviderTypes =
          new[]
          {
              typeof (TypeBasedAspectDescriptorProvider),
              typeof (PropertyBasedAspectDescriptorProvider),
              typeof (ParameterBasedAspectDescriptorProvider),
              typeof (MethodBasedAspectDescriptorProvider),
              typeof (InterfaceMethodBasedAspectDescriptorProvider),
              typeof (EventBasedAspectDescriptorProvider)
          };

      foreach (var expectedAspectDescriptorProviderType in expectedAspectDescriptorProviderTypes)
        aspectDescriptorsMock
            .Expect (x => x.Add (Arg<IAspectDescriptorProvider>.Matches (y => y.GetType() == expectedAspectDescriptorProviderType)));

      new DefaultAspectDescriptorProvidersConfigurator().Initialize (activeAttributesConfigurationStub);

      aspectDescriptorsMock.VerifyAllExpectations();
    }
  }
}