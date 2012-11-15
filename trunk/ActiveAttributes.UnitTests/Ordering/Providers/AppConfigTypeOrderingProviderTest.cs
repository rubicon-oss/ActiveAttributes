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
using ActiveAttributes.Configuration;
using ActiveAttributes.Ordering;
using ActiveAttributes.Ordering.Providers;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Ordering.Providers
{
  [TestFixture]
  public class AppConfigTypeOrderingProviderTest
  {
    private IConfigurationProvider _configurationProviderMock;
    private AppConfigTypeOrderingProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _configurationProviderMock = MockRepository.GenerateStrictMock<IConfigurationProvider> ();
      _provider = new AppConfigTypeOrderingProvider (_configurationProviderMock);
    }

    [Test]
    public void GetOrderings ()
    {
      var beforeType1 = ObjectMother.GetDeclaringType();
      var afterType1 = ObjectMother.GetDeclaringType();
      var beforeType2 = ObjectMother.GetDeclaringType();
      var afterType2 = ObjectMother.GetDeclaringType();

      var typeOrderingMock1 = MockRepository.GenerateStrictMock<TypeOrderingElement>();
      var typeOrderingMock2 = MockRepository.GenerateStrictMock<TypeOrderingElement> ();
      typeOrderingMock1.Expect (x => x.BeforeType).Return (beforeType1.FullName);
      typeOrderingMock1.Expect (x => x.AfterType).Return (afterType1.FullName);
      typeOrderingMock2.Expect (x => x.BeforeType).Return (beforeType2.FullName);
      typeOrderingMock2.Expect (x => x.AfterType).Return (afterType2.FullName);

      _configurationProviderMock.Expect (x => x.TypeOrderings).Return (new[] { typeOrderingMock1, typeOrderingMock2 });

      var result = _provider.GetOrderings().ToArray();

      _configurationProviderMock.VerifyAllExpectations();
      Assert.That (result, Has.Length.EqualTo (2).And.All.TypeOf<AdviceTypeOrdering>());
      var typeOrderings = result.OfType<AdviceTypeOrdering>().ToArray();
      Assert.That (typeOrderings[0].BeforeType, Is.EqualTo (beforeType1));
      Assert.That (typeOrderings[0].AfterType, Is.EqualTo (afterType1));
      Assert.That (typeOrderings[1].BeforeType, Is.EqualTo (beforeType2));
      Assert.That (typeOrderings[1].AfterType, Is.EqualTo (afterType2));
    }

    [Test]
    public void ThrowsForInvalidTypes ()
    {
      var typeOrderingMock1 = MockRepository.GenerateStrictMock<TypeOrderingElement>();
      var typeOrderingMock2 = MockRepository.GenerateStrictMock<TypeOrderingElement>();

      typeOrderingMock1.Expect (x => x.BeforeType).Return ("InvalidType");
      _configurationProviderMock.Expect (x => x.TypeOrderings).Return (new[] { typeOrderingMock1 });

      Assert.That (() => _provider.GetOrderings().ToArray(), Throws.TypeOf<TypeLoadException>());

      typeOrderingMock2.Expect (x => x.BeforeType).Return (GetType().Name);
      typeOrderingMock2.Expect (x => x.AfterType).Return ("InvalidType");
      _configurationProviderMock.Expect (x => x.TypeOrderings).Return (new[] { typeOrderingMock2 });

      Assert.That (() => _provider.GetOrderings().ToArray(), Throws.TypeOf<TypeLoadException>());
    }

    [Test]
    public void Resolution ()
    {
      var instances = SafeServiceLocator.Current.GetAllInstances<IAdviceOrderingProvider>();

      Assert.That (instances, Has.Some.TypeOf<AppConfigTypeOrderingProvider>());
    }
  }
}