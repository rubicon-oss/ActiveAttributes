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
using ActiveAttributes.Discovery;
using ActiveAttributes.Discovery.DeclarationProviders;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace ActiveAttributes.UnitTests.Discovery.DeclarationProviders
{
  [TestFixture]
  public class AspectClassDeclarationProviderTest
  {
    private ITypeDiscoveryService _typeDiscoveryServiceMock;
    private IClassDeclarationProvider _classDeclarationProviderMock;
    private AspectClassDeclarationProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _typeDiscoveryServiceMock = MockRepository.GenerateStrictMock<ITypeDiscoveryService> ();
      _classDeclarationProviderMock = MockRepository.GenerateStrictMock<IClassDeclarationProvider> ();
      _provider = new AspectClassDeclarationProvider (_typeDiscoveryServiceMock, _classDeclarationProviderMock);
    }

    [Test]
    public void GetDeclarations ()
    {
      var aspectTypes = new[] { typeof (DomainAspect1), typeof (DomainAspect2) };
      var fakeAdviceBuilder1 = ObjectMother.GetAdviceBuilder();
      var fakeAdviceBuilder2 = ObjectMother.GetAdviceBuilder();
      var fakeAdviceBuilder3 = ObjectMother.GetAdviceBuilder();

      _typeDiscoveryServiceMock.Expect (x => x.GetTypes (typeof (IAspect), false)).Return (aspectTypes);
      _classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect1)))
          .Return (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2 }.AsOneTime ());
      _classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect2)))
          .Return (new[] { fakeAdviceBuilder3 });

      var result = _provider.GetDeclarations().ToList();

      _typeDiscoveryServiceMock.VerifyAllExpectations();
      _classDeclarationProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2, fakeAdviceBuilder3 }));
    }

    [Test]
    public void GetDeclarations_IgnoreInterfaces ()
    {
      var aspectTypes = new[] { typeof (IAspect) };
      _typeDiscoveryServiceMock.Expect (x => x.GetTypes (typeof (IAspect), false)).Return (aspectTypes);

      Assert.That (() => _provider.GetDeclarations().ToList(), Throws.Nothing);
    }

    [Test]
    public void GetDeclarations_NoParameterlessConstructor ()
    {
      var aspectTypes = new[] { typeof(DomainAspect3) };
      _typeDiscoveryServiceMock.Expect (x => x.GetTypes (typeof (IAspect), false)).Return (aspectTypes);

      var message = string.Format ("Cannot create an object of type '{0}' without parameterless constructor.", typeof (DomainAspect3).Name);
      Assert.That (() => _provider.GetDeclarations().ToList(), Throws.InvalidOperationException.With.Message.EqualTo (message));
    }

    class DomainAspect1 : IAspect {}

    class DomainAspect2 : IAspect {}

    class DomainAspect3 : IAspect
    {
      public DomainAspect3 (string arg) {}
    }
  }
}