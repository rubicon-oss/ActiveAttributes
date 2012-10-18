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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Configuration;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.TypeAssembly;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class MethodAssemblerTest
  {
    [Test]
    public void name ()
    {
      var mockRepository = new MockRepository();

      var configurationMock = mockRepository.StrictMock<IConfiguration>();
      var fieldIntroducerMock = mockRepository.StrictMock<IFieldIntroducer>();
      var giveMeSomeNameMock = mockRepository.StrictMock<IGiveMeSomeName>();
      var schedulerMock = mockRepository.StrictMock<IScheduler>();
      var methodCopierMock = mockRepository.StrictMock<IMethodCopier>();
      var constructorPatcherMock = mockRepository.StrictMock<IConstructorPatcher>();
      var factoryMock = mockRepository.StrictMock<IFactory>();

      var aspectProviderMock = mockRepository.StrictMock<IMethodLevelDescriptorProvider>();
      var aspectProviders = new[] { aspectProviderMock };
      var descriptorsFake = new IDescriptor[0];
      var fieldDataFake = new FieldInfoContainer();

      var methodGeneratorMock = mockRepository.StrictMock<IExpressionGenerator>();
      var typeGeneratorMock = mockRepository.StrictMock<IExpressionGenerator>();
      var methodDescriptorStub = MockRepository.GenerateStub<IDescriptor>();
      var typeDescriptorMock = MockRepository.GenerateStub<IDescriptor>();
      var methodGenerators = new[] { methodGeneratorMock };
      var typeGenerators = new[] { typeGeneratorMock };
      var schedulerTuple =
          new[]
          {
              Tuple.Create (methodDescriptorStub, methodGeneratorMock),
              Tuple.Create (typeDescriptorMock, typeGeneratorMock)
          };
      var sortedGenerators = new IExpressionGenerator[0];

      var mutableType = MutableTypeObjectMother.Create();
      var method = mutableType.GetMethods().First();
      var mutableMethod = mutableType.GetOrAddMutableMethod (method);
      var copiedMethodFake = MutableMethodInfoObjectMother.Create();
      var typeProviderFake = MockRepository.GenerateStub<ITypeProvider>();
      var methodPatcherMock = mockRepository.StrictMock<IMethodPatcher>();

      // Arrange
      configurationMock
          .Expect (x => x.DescriptorProviders)
          .Return (aspectProviders);
      aspectProviderMock
          .Expect (x => x.GetDescriptors (method))
          .Return (descriptorsFake);
      fieldIntroducerMock
          .Expect (x => x.IntroduceMethodFields (mutableType, method))
          .Return (fieldDataFake);
      giveMeSomeNameMock
          .Expect (x => x.IntroduceExpressionGenerators (mutableType, descriptorsFake, fieldDataFake))
          .Return (methodGenerators);
      methodGeneratorMock
          .Expect (x => x.Descriptor)
          .Return (methodDescriptorStub);
      typeGeneratorMock
          .Expect (x => x.Descriptor)
          .Return (typeDescriptorMock).Repeat.AtLeastOnce();
      typeDescriptorMock
          .Expect (x => x.Matches(method))
          .Return (true);
      schedulerMock
          .Expect (
              x => x.GetOrdered (
                  Arg<IEnumerable<Tuple<IDescriptor, IExpressionGenerator>>>
                      .Matches (y => y.SetEquals (schedulerTuple))))
          .Return (sortedGenerators);
      methodCopierMock
          .Expect (x => x.GetCopy (mutableMethod))
          .Return (copiedMethodFake);
      constructorPatcherMock
          .Expect (x => x.AddReflectionAndDelegateInitialization (mutableMethod, fieldDataFake, copiedMethodFake));
      factoryMock
          .Expect (x => x.GetTypeProvider (method))
          .Return (typeProviderFake);
      factoryMock
          .Expect (x => x.GetMethodPatcher (mutableMethod, fieldDataFake, sortedGenerators, typeProviderFake))
          .Return (methodPatcherMock);
      methodPatcherMock
          .Expect (x => x.AddMethodInterception());

      mockRepository.ReplayAll();

      // Act
      var assembler = new MethodAssembler (
          configurationMock, fieldIntroducerMock, giveMeSomeNameMock, schedulerMock, methodCopierMock, constructorPatcherMock, factoryMock);
      assembler.ModifyMethod (mutableType, method, typeGenerators);

      // Assert
      mockRepository.VerifyAll();
    }

    // TODO test name
    [Test]
    public void ProceedOnlyIfAny_TypeGenerator ()
    {
      var configurationStub = MockRepository.GenerateStub<IConfiguration>();
      var fieldIntroducerStub = MockRepository.GenerateStub<IFieldIntroducer>();
      var giveMeSomeNameStub = MockRepository.GenerateStub<IGiveMeSomeName>();
      var schedulerStub = MockRepository.GenerateStub<IScheduler>();
      var methodCopierStub = MockRepository.GenerateStub<IMethodCopier>();
      var constructorPatcherStub = MockRepository.GenerateStub<IConstructorPatcher>();
      var factoryStub = MockRepository.GenerateStub<IFactory>();

      var mutableType = MutableTypeObjectMother.Create();
      var method = mutableType.GetMethods().First();

      var expressionGeneratorMock = MockRepository.GenerateStrictMock<IExpressionGenerator>();
      var descriptorMock = MockRepository.GenerateStrictMock<IDescriptor>();
      var methodPatcherMock = MockRepository.GenerateStrictMock<IMethodPatcher>();

      // Arrange
      configurationStub
          .Stub (x => x.DescriptorProviders)
          .Return (new IDescriptorProvider[0]);
      giveMeSomeNameStub
          .Stub (x => x.IntroduceExpressionGenerators (null, null, new FieldInfoContainer()))
          .IgnoreArguments()
          .Return (Enumerable.Empty<IExpressionGenerator>());
      factoryStub
          .Stub (x => x.GetMethodPatcher (null, new FieldInfoContainer(), null, null))
          .IgnoreArguments()
          .Return (methodPatcherMock);

      expressionGeneratorMock
          .Expect (x => x.Descriptor)
          .Return (descriptorMock);
      descriptorMock
          .Expect (x => x.Matches(null))
          .IgnoreArguments()
          .Return (false);

      // Act
      var assembler = new MethodAssembler (
          configurationStub, fieldIntroducerStub, giveMeSomeNameStub, schedulerStub, methodCopierStub, constructorPatcherStub, factoryStub);
      assembler.ModifyMethod (mutableType, method, new[] { expressionGeneratorMock });

      // Assert
      expressionGeneratorMock.VerifyAllExpectations();
      descriptorMock.VerifyAllExpectations();
    }

    // TODO test name
    [Test]
    public void ProceedOnlyIfAny_MethodGenerator ()
    {
      var configurationStub = MockRepository.GenerateStub<IConfiguration>();
      var fieldIntroducerStub = MockRepository.GenerateStub<IFieldIntroducer>();
      var giveMeSomeNameStub = MockRepository.GenerateStub<IGiveMeSomeName>();
      var schedulerStub = MockRepository.GenerateStub<IScheduler>();
      var methodCopierStub = MockRepository.GenerateStub<IMethodCopier>();
      var constructorPatcherStub = MockRepository.GenerateStub<IConstructorPatcher>();
      var factoryStub = MockRepository.GenerateStub<IFactory>();

      var mutableType = MutableTypeObjectMother.Create();
      var method = mutableType.GetMethods().First();

      var expressionGeneratorMock = MockRepository.GenerateStrictMock<IExpressionGenerator>();
      var descriptorMock = MockRepository.GenerateStrictMock<IDescriptor>();
      var methodPatcherMock = MockRepository.GenerateStrictMock<IMethodPatcher>();

      // Arrange
      configurationStub
          .Stub (x => x.DescriptorProviders)
          .Return (new IDescriptorProvider[0]);
      giveMeSomeNameStub
          .Stub (x => x.IntroduceExpressionGenerators (null, null, new FieldInfoContainer()))
          .IgnoreArguments()
          .Return (Enumerable.Empty<IExpressionGenerator>());
      factoryStub
          .Stub (x => x.GetMethodPatcher (null, new FieldInfoContainer(), null, null))
          .IgnoreArguments()
          .Return (methodPatcherMock);
      
      // Act
      var assembler = new MethodAssembler (
          configurationStub, fieldIntroducerStub, giveMeSomeNameStub, schedulerStub, methodCopierStub, constructorPatcherStub, factoryStub);
      assembler.ModifyMethod (mutableType, method, new IExpressionGenerator[0]);

      // Assert
      expressionGeneratorMock.VerifyAllExpectations();
      descriptorMock.VerifyAllExpectations();
    }
  }
}