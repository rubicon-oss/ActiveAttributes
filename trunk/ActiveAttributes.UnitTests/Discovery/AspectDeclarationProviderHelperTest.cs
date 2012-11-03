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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Attributes.Aspects;
using ActiveAttributes.Core.Discovery;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class AspectDeclarationProviderHelperTest
  {
    private AspectDeclarationProviderHelper _aspectDeclarationProviderHelper;
    private ICustomAttributeDataToAdviceConverter _customAttributeDataToAdviceConverterMock;
    private IStandaloneAdviceProvider _standaloneAdviceProviderMock;
    private IAdviceMerger _adviceMergerMock;


    [SetUp]
    public void SetUp ()
    {
      _customAttributeDataToAdviceConverterMock = MockRepository.GenerateStrictMock<ICustomAttributeDataToAdviceConverter>();
      _standaloneAdviceProviderMock = MockRepository.GenerateStrictMock<IStandaloneAdviceProvider>();
      _adviceMergerMock = MockRepository.GenerateStrictMock<IAdviceMerger>();

      _aspectDeclarationProviderHelper = new Core.Discovery.AspectDeclarationProviderHelper (
          _standaloneAdviceProviderMock, _customAttributeDataToAdviceConverterMock, _adviceMergerMock);
    }

    [Test]
    public void GetAdvices ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());

      var fakeAttributeAdvice1 = ObjectMother2.GetAdvice();
      var fakeAttributeAdvice2 = ObjectMother2.GetAdvice();

      var fakeTypeAdvice1 = ObjectMother2.GetAdvice();
      var fakeTypeAdvice2 = ObjectMother2.GetAdvice();
      var fakeTypeAdvice3 = ObjectMother2.GetAdvice();
      var fakeTypeAdvices1 = new[] { fakeTypeAdvice1, fakeTypeAdvice2 };
      var fakeTypeAdvices2 = new[] { fakeTypeAdvice3 };

      var fakeMergedAdvice1 = ObjectMother2.GetAdvice();
      var fakeMergedAdvice2 = ObjectMother2.GetAdvice();
      var fakeMergedAdvice3 = ObjectMother2.GetAdvice();

      _customAttributeDataToAdviceConverterMock
          .Expect (x => x.GetAdvice (Arg<IEnumerable<ICustomAttributeNamedArgument>>.Matches (y => (int) y.Single().Value == 1)))
          .Return (fakeAttributeAdvice1);
      _standaloneAdviceProviderMock
          .Expect (x => x.GetAdvices (typeof (DomainAspect1Attribute)))
          .Return (fakeTypeAdvices1);
      _adviceMergerMock
          .Expect (x => x.Merge (null, null)).IgnoreArguments()
          .WhenCalled (x => CheckAdviceMergerArguments (x.Arguments, new object[] { fakeAttributeAdvice1, fakeTypeAdvice1 }))
          .Return (fakeMergedAdvice1);
      _adviceMergerMock
          .Expect (x => x.Merge (null, null)).IgnoreArguments ()
          .WhenCalled (x => CheckAdviceMergerArguments (x.Arguments, new object[] { fakeAttributeAdvice1, fakeTypeAdvice2 }))
          .Return (fakeMergedAdvice2);
      _customAttributeDataToAdviceConverterMock
          .Expect (x => x.GetAdvice (Arg<IEnumerable<ICustomAttributeNamedArgument>>.Matches (y => (int) y.Single().Value == 2)))
          .Return (fakeAttributeAdvice2);
      _standaloneAdviceProviderMock
          .Expect (x => x.GetAdvices (typeof (DomainAspect2Attribute)))
          .Return (fakeTypeAdvices2);
      _adviceMergerMock
          .Expect (x => x.Merge (null, null)).IgnoreArguments ()
          .WhenCalled (x => CheckAdviceMergerArguments (x.Arguments, new object[] { fakeAttributeAdvice2, fakeTypeAdvice3 }))
          .Return (fakeMergedAdvice3);

      var result = _aspectDeclarationProviderHelper.GetAspectDeclarations (method).ToList();

      _customAttributeDataToAdviceConverterMock.VerifyAllExpectations();
      _standaloneAdviceProviderMock.VerifyAllExpectations();
      // TODO need to ToList() the advices within AspectDeclaration class
      _adviceMergerMock.VerifyAllExpectations();

      Assert.That (result, Has.Count.EqualTo (2));
      Assert.That (result[0].ConstructionInfo.ConstructorInfo.DeclaringType, Is.EqualTo (typeof (DomainAspect1Attribute)));
      Assert.That (result[0].Advices, Is.EqualTo (new[] { fakeMergedAdvice1, fakeMergedAdvice2 }));
      Assert.That (result[1].ConstructionInfo.ConstructorInfo.DeclaringType, Is.EqualTo (typeof (DomainAspect2Attribute)));
      Assert.That (result[1].Advices, Is.EqualTo (new[] { fakeMergedAdvice3 }));
    }

    [Test]
    public void SkipNonAspectAttributes ()
    {
      var method = MethodInfo.GetCurrentMethod();

      Assert.That (() => _aspectDeclarationProviderHelper.GetAspectDeclarations (method).ToArray(), Throws.Nothing);
    }

    private void CheckAdviceMergerArguments (object[] actual, object[] expected)
    {
      Assert.That (actual[0], Is.SameAs (expected[0]).Or.SameAs (expected[1]));
      Assert.That (actual[1], Is.SameAs (expected[0]).Or.SameAs (expected[1]));
      Assert.That (actual[0], Is.Not.EqualTo (actual[1]));
    }

    class DomainType
    {
      [DomainAspect1 (AdvicePriority = 1)]
      [DomainAspect2 (AdvicePriority = 2)]
      public void Method () {}
    }

    class DomainAspect1Attribute : AspectAttributeBase {}

    class DomainAspect2Attribute : AspectAttributeBase {}
  }
}