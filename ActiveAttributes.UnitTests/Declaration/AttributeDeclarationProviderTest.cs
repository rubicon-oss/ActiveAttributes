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
using ActiveAttributes.Aspects2;
using ActiveAttributes.Declaration;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Declaration
{
  [TestFixture]
  public class AttributeDeclarationProviderTest
  {
    private AttributeDeclarationProvider _provider;

    private ICustomAttributeDataTransform _customAttributeDataTransformMock;
    private IClassDeclarationProvider _classDeclarationProviderMock;

    [SetUp]
    public void SetUp ()
    {
      _classDeclarationProviderMock = MockRepository.GenerateStrictMock<IClassDeclarationProvider> ();
      _customAttributeDataTransformMock = MockRepository.GenerateStrictMock<ICustomAttributeDataTransform> ();

      _provider = new AttributeDeclarationProvider (_classDeclarationProviderMock, _customAttributeDataTransformMock);
    }

    [Test]
    public void GetAdviceBuilders ()
    {
      var customAttributeData = ObjectMother.GetCustomAttributeData (typeof (AspectAttributeBase));
      var fakeAdviceBuilders1 = new IAdviceBuilder[0];
      var fakeAdviceBuilder1 = ObjectMother.GetAdviceBuilder ();
      var fakeAdviceBuilder2 = ObjectMother.GetAdviceBuilder ();
      var fakeClassAdviceBuilders = new[] { fakeAdviceBuilder1, fakeAdviceBuilder2 };

      _classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (AspectAttributeBase)))
          .Return (fakeAdviceBuilders1.AsOneTime ());
      _customAttributeDataTransformMock
          .Expect (x => x.UpdateAdviceBuilders (customAttributeData, fakeAdviceBuilders1))
          .Return (fakeClassAdviceBuilders);

      var result = _provider.GetAdviceBuilders (customAttributeData).ToList();

      _classDeclarationProviderMock.VerifyAllExpectations();
      _customAttributeDataTransformMock.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2 }));
    }

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IAttributeDeclarationProvider>();

      Assert.That (instance, Is.TypeOf<AttributeDeclarationProvider>());
    }
  }
}