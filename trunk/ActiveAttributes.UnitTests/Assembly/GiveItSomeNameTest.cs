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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Done;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class GiveItSomeNameTest
  {
    private AspectAttribute[] _instanceField;
    private static AspectAttribute[] _staticField;

    [Test]
    //[Ignore]
    public void name ()
    {
      var factoryMock = MockRepository.GenerateStrictMock<IFactory>();
      var expressionGeneratorFactoryMock = MockRepository.GenerateStrictMock<IExpressionGeneratorFactory>();
      var constructorPatcherMock = MockRepository.GenerateStrictMock<IConstructorPatcher>();

      var fieldDataFake =
          new FieldInfoContainer
          {
              InstanceAspectsField = NormalizingMemberInfoFromExpressionUtility.GetField (() => _instanceField),
              StaticAspectsField = NormalizingMemberInfoFromExpressionUtility.GetField (() => _staticField)
          };

      var instanceArrayAccessorStub = MockRepository.GenerateStub<IFieldWrapper>();
      var staticArrayAccessorStub = MockRepository.GenerateStub<IFieldWrapper>();
      var aspectDescriptors = new[] { MockRepository.GenerateStub<IAspectDescriptor>() };
      var aspectGenerators = new IExpressionGenerator[0];
      var mutableType = MutableTypeObjectMother.CreateForExistingType();

      factoryMock
          .Expect (x => x.GetAccessor (fieldDataFake.InstanceAspectsField))
          .Return (instanceArrayAccessorStub);
      factoryMock
          .Expect (x => x.GetAccessor (fieldDataFake.StaticAspectsField))
          .Return (staticArrayAccessorStub);

      expressionGeneratorFactoryMock
          .Expect (x => x.GetExpressionGenerators (instanceArrayAccessorStub, staticArrayAccessorStub, aspectDescriptors))
          .Return (aspectGenerators);

      constructorPatcherMock
          .Expect (x => x.AddAspectInitialization (mutableType, staticArrayAccessorStub, instanceArrayAccessorStub, aspectGenerators));

      var assembler = new GiveItSomeName (factoryMock, expressionGeneratorFactoryMock, constructorPatcherMock);
      var result = assembler.IntroduceExpressionGenerators (mutableType, aspectDescriptors, fieldDataFake);

      Assert.That (result, Is.EquivalentTo (aspectGenerators));
      factoryMock.VerifyAllExpectations();
      expressionGeneratorFactoryMock.VerifyAllExpectations();
      constructorPatcherMock.VerifyAllExpectations();
    }
  }
}