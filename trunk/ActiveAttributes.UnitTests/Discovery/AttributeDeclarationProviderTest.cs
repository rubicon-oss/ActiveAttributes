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
using ActiveAttributes.Advices;
using ActiveAttributes.Aspects;
using ActiveAttributes.Discovery;
using ActiveAttributes.Pointcuts;
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
    private AttributeDeclarationProvider _provider;
    private ICustomAttributeDataTransform _customAttributeDataTransformMock;
    private IClassDeclarationProvider _classDeclarationProviderMock;

    [SetUp]
    public void SetUp ()
    {
      _customAttributeDataTransformMock = MockRepository.GenerateStrictMock<ICustomAttributeDataTransform> ();
      _classDeclarationProviderMock = MockRepository.GenerateStrictMock<IClassDeclarationProvider> ();

      _provider = new AttributeDeclarationProvider (_classDeclarationProviderMock, _customAttributeDataTransformMock);
    }

    [Test]
    public void GetAdviceBuilders ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());

      var fakeAdviceBuilder1 = ObjectMother.GetAdviceBuilder();
      var fakeAdviceBuilder2 = ObjectMother.GetAdviceBuilder();
      var fakeAdviceBuilder3 = ObjectMother.GetAdviceBuilder();
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

      var result = _provider.GetAdviceBuilders (method).ToList();

      _customAttributeDataTransformMock.VerifyAllExpectations();
      _classDeclarationProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2, fakeAdviceBuilder3 }));
    }

    [Test]
    public void SkipNonAspectAttributes ()
    {
      var method = MethodInfo.GetCurrentMethod();

      Assert.That (() => _provider.GetAdviceBuilders (method).ToArray(), Throws.Nothing);
    }

    //[Test]
    //public void ThrowsForOverSpecification ()
    //{
    //  CheckThrows ("method", typeof (MemberNamePointcut), typeof (DomainType).GetMethods().Single (x => x.Name == "MemberNamePointcut"));
    //  CheckThrows ("method", typeof (ReturnTypePointcut), typeof (DomainType).GetMethods().Single (x => x.Name == "ReturnTypePointcut"));
    //}

    //private void CheckThrows (string memberType, Type type, MemberInfo member)
    //{
    //  var message = string.Format ("Pointcut of type {0} cannot be applied to a {1}", type.Name, memberType);
    //  Assert.That (() => _provider.GetAdviceBuilders (member).ToArray(), Throws.Exception.With.Message.EqualTo (message));
    //}

    class DomainType
    {
      [DomainAspect1 (AdvicePriority = 1)]
      [DomainAspect2 (AdvicePriority = 2, AdviceExecution = AdviceExecution.Around)]
      public void Method () {}

      [DomainAspect1 (MemberNameFilter = "MethodWithMemberNamePointcut")]
      public void MemberNamePointcut () {}

      [DomainAspect1 (MemberReturnTypeFilter = typeof (void))]
      public void ReturnTypePointcut () {}
    }

    [DomainAspect1 (ApplyToType = typeof (DomainTypeWithApplyToType))]
    class DomainTypeWithApplyToType {}

    [DomainAspect1 (ApplyToNamespace = "ActiveAttributes.*")]
    class DomainTypeWithApplyToNamespace {}

    class DomainAspect1Attribute : AspectAttributeBase { }

    class DomainAspect2Attribute : AspectAttributeBase { }
  }
}