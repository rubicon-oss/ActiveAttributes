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
using System.Reflection;
using ActiveAttributes.Core.AdviceInfo;
using ActiveAttributes.Core.Aspects;
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

      var fakeAdviceBuilder1 = ObjectMother2.GetAdviceBuilder();
      var fakeAdviceBuilder2 = ObjectMother2.GetAdviceBuilder();
      var fakeAdviceBuilder3 = ObjectMother2.GetAdviceBuilder();
      var fakeTypeAdviceBuilders1 = new[] { fakeAdviceBuilder1, fakeAdviceBuilder2 };
      var fakeTypeAdviceBuilders2 = new[] { fakeAdviceBuilder3 };
      var fakeAdviceBuilders1 = new IAdviceBuilder[0];
      var fakeAdviceBuilders2 = new IAdviceBuilder[0];

      _classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect1Attribute)))
          .Return (fakeAdviceBuilders1.AsOneTime());
      _customAttributeDataTransformMock
          .Expect (x => x.UpdateAdviceBuilders (Arg<ICustomAttributeData>.Matches (y => y.NamedArguments.Count == 1), Arg.Is (fakeAdviceBuilders1)))
          .Return (fakeTypeAdviceBuilders1);
      _classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect2Attribute)))
          .Return (fakeAdviceBuilders2.AsOneTime());
      _customAttributeDataTransformMock
          .Expect (x => x.UpdateAdviceBuilders (Arg<ICustomAttributeData>.Matches (y => y.NamedArguments.Count == 2), Arg.Is (fakeAdviceBuilders2)))
          .Return (fakeTypeAdviceBuilders2);

      var result = _attributeDeclarationProvider.GetAdviceBuilders (method).ToList();

      _customAttributeDataTransformMock.VerifyAllExpectations();
      _classDeclarationProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2, fakeAdviceBuilder3 }));
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