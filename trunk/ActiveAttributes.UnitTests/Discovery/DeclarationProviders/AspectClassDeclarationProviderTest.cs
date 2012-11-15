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
using ActiveAttributes.Discovery;
using ActiveAttributes.Discovery.DeclarationProviders;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace ActiveAttributes.UnitTests.Discovery.DeclarationProviders
{
  [TestFixture]
  public class AspectClassDeclarationProviderTest
  {
    private IClassDeclarationProvider _classDeclarationProviderMock;
    private AspectClassDeclarationProvider _provider;
    private IAspectTypesProvider _aspectTypesProviderMock;

    [SetUp]
    public void SetUp ()
    {
      _classDeclarationProviderMock = MockRepository.GenerateStrictMock<IClassDeclarationProvider> ();
      _aspectTypesProviderMock = MockRepository.GenerateStrictMock<IAspectTypesProvider>();
      _provider = new AspectClassDeclarationProvider (_aspectTypesProviderMock, _classDeclarationProviderMock);
    }

    [Test]
    public void GetDeclarations ()
    {
      var aspectTypes = new[] { typeof (DomainAspect1), typeof (DomainAspect2) };
      var fakeAdviceBuilder1 = ObjectMother.GetAdviceBuilder();
      var fakeAdviceBuilder2 = ObjectMother.GetAdviceBuilder();
      var fakeAdviceBuilder3 = ObjectMother.GetAdviceBuilder();

      _aspectTypesProviderMock.Expect (x => x.GetAspectClassTypes()).Return (aspectTypes);
      _classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect1)))
          .Return (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2 }.AsOneTime ());
      _classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect2)))
          .Return (new[] { fakeAdviceBuilder3 });

      var result = _provider.GetDeclarations().ToList();

      _aspectTypesProviderMock.VerifyAllExpectations ();
      _classDeclarationProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2, fakeAdviceBuilder3 }));
    }

    [Test]
    public void Resolution ()
    {
      var instances = SafeServiceLocator.Current.GetAllInstances<IAssemblyLevelDeclarationProvider>();

      Assert.That (instances, Has.Some.TypeOf<AspectClassDeclarationProvider>());
    }

    class DomainAspect1 : IAspect {}

    class DomainAspect2 : IAspect {}
  }
}