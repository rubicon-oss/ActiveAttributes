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
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Infrastructure;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class StandaloneAdviceProviderTest
  {
    private StandaloneAdviceProvider _standaloneAdviceProvider;

    private IAdviceMerger _adviceMergerMock;
    private ICustomAttributeProviderToAdviceConverter _customAttributeConverterMock;

    [SetUp]
    public void SetUp ()
    {
      _adviceMergerMock = MockRepository.GenerateStrictMock<IAdviceMerger>();
      _customAttributeConverterMock = MockRepository.GenerateStrictMock<ICustomAttributeProviderToAdviceConverter>();
      _standaloneAdviceProvider = new StandaloneAdviceProvider (_adviceMergerMock, _customAttributeConverterMock);
    }

    [Test]
    public void name ()
    {
      var fakeTypeAdvice = ObjectMother2.GetAdvice();
      var fakeMethodAdvice = ObjectMother2.GetAdvice();
      var fakeMergedAdvice = ObjectMother2.GetAdvice();

      var aspectType = typeof (DomainAspect);
      var adviceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainAspect obj) => obj.AdviceMethod());

      _customAttributeConverterMock.Expect (x => x.GetAdvice (aspectType)).Return (fakeTypeAdvice);
      _customAttributeConverterMock.Expect (x => x.GetAdvice (adviceMethod)).Return (fakeMethodAdvice);
      _adviceMergerMock
          .Expect (x => x.Merge (null, null)).IgnoreArguments()
          .WhenCalled (x => CheckAdviceMergerArguments (x.Arguments, new[] { fakeTypeAdvice, fakeMethodAdvice }))
          .Return (fakeMergedAdvice);

      var result = _standaloneAdviceProvider.GetAdvices (aspectType).Single();

      _customAttributeConverterMock.VerifyAllExpectations();
      _adviceMergerMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeMergedAdvice));
    }

    private void CheckAdviceMergerArguments (object[] actual, object[] expected)
    {
      Assert.That (actual[0], Is.EqualTo (expected[0]).Or.EqualTo (expected[1]));
      Assert.That (actual[1], Is.EqualTo (expected[0]).Or.EqualTo (expected[1]));
      Assert.That (actual[0], Is.Not.EqualTo (actual[1]));
    }

    class DomainAspect : IAspect
    {
      public void AdviceMethod () {}

      private void NonAdviceMethod () {}
    }
  }
}