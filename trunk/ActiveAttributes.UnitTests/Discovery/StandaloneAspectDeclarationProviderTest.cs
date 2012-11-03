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
using System.ComponentModel.Design;
using System.Linq;
using ActiveAttributes.Core.Attributes.Aspects;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.Construction;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class StandaloneAspectDeclarationProviderTest
  {
    private ITypeDiscoveryService _typeDiscoveryServiceMock;
    private IStandaloneAdviceProvider _standaloneAdviceProviderMock;
    private StandaloneAspectDeclarationProvider _declarationProvider;

    [SetUp]
    public void SetUp ()
    {
      _typeDiscoveryServiceMock = MockRepository.GenerateStrictMock<ITypeDiscoveryService>();
      _standaloneAdviceProviderMock = MockRepository.GenerateStrictMock<IStandaloneAdviceProvider>();
      _declarationProvider = new StandaloneAspectDeclarationProvider (_typeDiscoveryServiceMock, _standaloneAdviceProviderMock);
    }

    [Test]
    public void GetDeclarations ()
    {
      var aspectType = typeof (DomainAspect);
      var fakeAdvice1 = ObjectMother2.GetAdvice();
      var fakeAdvice2 = ObjectMother2.GetAdvice();
      var fakeAdvices = new[] { fakeAdvice1, fakeAdvice2 };

      _typeDiscoveryServiceMock.Expect (x => x.GetTypes (typeof (IAspect), false)).Return (new[] { aspectType });
      _standaloneAdviceProviderMock.Expect (x => x.GetAdvices (aspectType)).Return (fakeAdvices);

      var result = _declarationProvider.GetDeclarations().Single();

      _typeDiscoveryServiceMock.VerifyAllExpectations();
      _standaloneAdviceProviderMock.VerifyAllExpectations();
      var constructionInfo = result.ConstructionInfo;
      Assert.That (constructionInfo, Is.TypeOf<TypeAspectConstructionInfo>());
      Assert.That (constructionInfo.ConstructorInfo.DeclaringType, Is.EqualTo (typeof (DomainAspect)));
      Assert.That (result.Advices, Is.EqualTo (fakeAdvices));
    }

    [Test]
    public void IgnoresAspectAttributes ()
    {
      var aspectType = typeof (DomainAspectAttribute);

      _typeDiscoveryServiceMock.Expect (x => x.GetTypes (typeof (IAspect), false)).Return (new[] { aspectType });

      Assert.That (() => _declarationProvider.GetDeclarations().ToArray(), Throws.Nothing);
    }

    class DomainAspect : IAspect {}

    class DomainAspectAttribute : AspectAttributeBase {}
  }
}