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
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class ClassDeclarationProviderTest
  {
    private ClassDeclarationProvider _classDeclarationProvider;

    private ICustomAttributeProviderTransform _customAttributeConverterMock;

    [SetUp]
    public void SetUp ()
    {
      _customAttributeConverterMock = MockRepository.GenerateStrictMock<ICustomAttributeProviderTransform> ();
      _classDeclarationProvider = new ClassDeclarationProvider (_customAttributeConverterMock);
    }

    [Test]
    public void GetAdviceBuilders ()
    {
      var fakeTypeAdviceBuilder = ObjectMother.GetAdviceBuilder();
      var fakeMethodAdviceBuilder = ObjectMother.GetAdviceBuilder();

      var aspectType = typeof (DomainAspect);
      var adviceMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainAspect obj) => obj.AdviceMethod ());

      _customAttributeConverterMock.Expect (x => x.GetAdviceBuilder (aspectType)).Return (fakeTypeAdviceBuilder);
      _customAttributeConverterMock.Expect (x => x.GetAdviceBuilder (adviceMethod, fakeTypeAdviceBuilder)).Return (fakeMethodAdviceBuilder);

      var result = _classDeclarationProvider.GetAdviceBuilders (aspectType).Single();

      _customAttributeConverterMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeMethodAdviceBuilder));
    }

    class DomainAspect : IAspect
    {
      public void AdviceMethod () { }

      private void NonAdviceMethod () { }
    }
  }
}