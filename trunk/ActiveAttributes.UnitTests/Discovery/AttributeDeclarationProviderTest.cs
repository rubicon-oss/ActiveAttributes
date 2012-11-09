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
using ActiveAttributes.Core.AdviceInfo;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.Construction;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class AttributeDeclarationProviderTest
  {
    private AttributeDeclarationProvider _attributeDeclarationProvider;
    private ICustomAttributeDataTransform _customAttributeDataTransformMock;
    private IClassDeclarationProvider _classDeclarationProviderMock;

    [SetUp]
    public void SetUp ()
    {
      _customAttributeDataTransformMock = MockRepository.GenerateStrictMock<ICustomAttributeDataTransform> ();
      _classDeclarationProviderMock = MockRepository.GenerateStrictMock<IClassDeclarationProvider> ();

      _attributeDeclarationProvider = new AttributeDeclarationProvider (_classDeclarationProviderMock, _customAttributeDataTransformMock);
    }

    [Test]
    public void GetAdvices ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());

      var fakeAttributeAdviceBuilder1 = ObjectMother2.GetAdviceBuilder();
      var fakeAttributeAdviceBuilder2 = ObjectMother2.GetAdviceBuilder();

      var typeAdviceBuilderMock1 = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var typeAdviceBuilderMock2 = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var typeAdviceBuilderMock3 = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var fakeTypeAdviceBuilders1 = new[] { typeAdviceBuilderMock1, typeAdviceBuilderMock2 };
      var fakeTypeAdviceBuilders2 = new[] { typeAdviceBuilderMock3 };

      IConstruction construction1 = null;
      IConstruction construction2 = null;
      _customAttributeDataTransformMock
          .Expect (x => x.GetAdviceBuilder (Arg<ICustomAttributeData>.Matches (y => y.NamedArguments.Count == 1)))
          .Return (fakeAttributeAdviceBuilder1);
      _customAttributeDataTransformMock
          .Expect (x => x.GetAdviceBuilder (Arg<ICustomAttributeData>.Matches (y => y.NamedArguments.Count == 2)))
          .Return (fakeAttributeAdviceBuilder2);
      _classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect1Attribute), fakeAttributeAdviceBuilder1))
          .Return (fakeTypeAdviceBuilders1.AsOneTime());
      _classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect2Attribute), fakeAttributeAdviceBuilder2))
          .Return (fakeTypeAdviceBuilders2.AsOneTime());
      typeAdviceBuilderMock1
          .Expect (x => x.UpdateConstruction (Arg<IConstruction>.Matches (y => y.ConstructorInfo.DeclaringType == typeof (DomainAspect1Attribute))))
          .WhenCalled (x => construction1 = (IConstruction) x.Arguments[0])
          .Return (typeAdviceBuilderMock1);
      typeAdviceBuilderMock2
          .Expect (x => x.UpdateConstruction (Arg<IConstruction>.Matches (y => y.ConstructorInfo.DeclaringType == typeof (DomainAspect1Attribute))))
          .WhenCalled (x => construction2 = (IConstruction) x.Arguments[0])
          .Return (typeAdviceBuilderMock2);
      typeAdviceBuilderMock3
          .Expect (x => x.UpdateConstruction (Arg<IConstruction>.Matches (y => y.ConstructorInfo.DeclaringType == typeof (DomainAspect2Attribute))))
          .Return (typeAdviceBuilderMock3);

      var result = _attributeDeclarationProvider.GetAdviceBuilders (method).ToList();

      _customAttributeDataTransformMock.VerifyAllExpectations();
      _classDeclarationProviderMock.VerifyAllExpectations();
      typeAdviceBuilderMock1.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { typeAdviceBuilderMock1, typeAdviceBuilderMock2, typeAdviceBuilderMock3 }));
      Assert.That (construction1, Is.SameAs (construction2));
    }

    [Test]
    public void SkipNonAspectAttributes ()
    {
      var method = MethodInfo.GetCurrentMethod();

      Assert.That (() => _attributeDeclarationProvider.GetAdviceBuilders (method).ToArray(), Throws.Nothing);
    }

    class DomainType
    {
      [DomainAspect1 (AdvicePriority = 1)]
      [DomainAspect2 (AdvicePriority = 2, AdviceExecution = AdviceExecution.Around)]
      public void Method () { }
    }

    class DomainAspect1Attribute : AspectAttributeBase { }

    class DomainAspect2Attribute : AspectAttributeBase { }
  }
}