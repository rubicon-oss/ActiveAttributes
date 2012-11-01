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
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Configuration2;
using NUnit.Framework;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly.Old
{
  [TestFixture]
  public class TypeAssemblerTest
  {
    [Test]
    public void name ()
    {
      var mockRepository = new MockRepository();

      var fieldIntroducerMock = mockRepository.StrictMock<IFieldIntroducer>();
      var giveMeSomeNameMock = mockRepository.StrictMock<IGiveMeSomeName>();
      var methodAssemblerMock = mockRepository.StrictMock<IMethodAssembler>();
      var mutableType = MutableTypeObjectMother.CreateForExistingType();
      var aspectProviderMock = mockRepository.StrictMock<ITypeLevelAspectDescriptorProvider>();
      var aspectDescriptorProviders = new[] { aspectProviderMock };
      var descriptorsFake = new IAspectDescriptor[0];
      var fieldDataFake = new FieldInfoContainer();
      var generatorsFake = new IExpressionGenerator[0];

      // Arrange
      aspectProviderMock
          .Expect (x => x.GetDescriptors (mutableType))
          .Return (descriptorsFake);
      fieldIntroducerMock
          .Expect (x => x.IntroduceTypeFields (mutableType))
          .Return (fieldDataFake);
      giveMeSomeNameMock
          .Expect (x => x.IntroduceExpressionGenerators (mutableType, descriptorsFake, fieldDataFake))
          .Return (generatorsFake);
      foreach (var method in mutableType.GetMethods())
      {
        methodAssemblerMock
            .Expect (x => x.ModifyMethod (mutableType, method, generatorsFake));
      }
      mockRepository.ReplayAll();

      // Act
      var assembler = new TypeAssembler (aspectDescriptorProviders, fieldIntroducerMock, giveMeSomeNameMock, methodAssemblerMock);
      assembler.ModifyType (mutableType);

      // Assert
      mockRepository.VerifyAll();
    }
  }
}