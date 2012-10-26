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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration2;
using NUnit.Framework;
using Ninject;
using Ninject.Parameters;
using Remotion.Collections;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class MethodAssemblerTest
  {
    private RhinoMocksKernel _kernel;

    private IGiveMeSomeName _giveMeSomeName;
    private IMethodLevelAspectDescriptorProvider _aspectDescriptorProvider;
    
    [Test]
    public void asdg ()
    {
      var kernel = new RhinoMocksKernel()
          .WithStrict (out _giveMeSomeName)
          .WithStrict (out _aspectDescriptorProvider)
          .With (new[] { _aspectDescriptorProvider });

      var objToReturn = new IAspectDescriptor[0];
      _aspectDescriptorProvider
          .Expect (x => x.GetDescriptors (null))
          .IgnoreArguments()
          .Return (objToReturn);
      _giveMeSomeName
          .Expect (
              x => x.IntroduceExpressionGenerators (
                  Arg<MutableType>.Is.Equal (_mutableType),
                  Arg<IEnumerable<IAspectDescriptor>>.Is.Equal (objToReturn),
                  Arg<FieldInfoContainer>.Is.Anything))
          .Return (new IExpressionGenerator[0]);

      var assembler = kernel.Get<MethodAssembler>();
      assembler.ModifyMethod (_mutableType, _methodInfo, new IExpressionGenerator[0]);

      kernel.VerifyAll ();
    }


    [Test]
    [Ignore]
    public void name ()
    {
      var mockRepository = new MockRepository();

      var configurationMock = mockRepository.StrictMock<IActiveAttributesConfiguration>();
      var fieldIntroducerMock = mockRepository.StrictMock<IFieldIntroducer>();
      var giveMeSomeNameMock = mockRepository.StrictMock<IGiveMeSomeName>();
      var schedulerMock = mockRepository.StrictMock<IAspectSorter>();
      var methodCopierMock = mockRepository.StrictMock<IMethodCopier>();
      var constructorPatcherMock = mockRepository.StrictMock<IConstructorPatcher>();
      var factoryMock = mockRepository.StrictMock<IFactory>();

      var aspectProviderMock = mockRepository.StrictMock<IMethodLevelAspectDescriptorProvider>();
      var aspectProviders = new[] { aspectProviderMock };
      var descriptorsFake = new IAspectDescriptor[0];
      var fieldDataFake = new FieldInfoContainer();

      var methodGeneratorMock = mockRepository.StrictMock<IExpressionGenerator>();
      var typeGeneratorMock = mockRepository.StrictMock<IExpressionGenerator>();
      var methodDescriptorStub = MockRepository.GenerateStub<IAspectDescriptor>();
      var typeDescriptorMock = MockRepository.GenerateStub<IAspectDescriptor>();
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
          .Expect (x => x.AspectDescriptorProviders)
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
          .Expect (x => x.AspectDescriptor)
          .Return (methodDescriptorStub);
      typeGeneratorMock
          .Expect (x => x.AspectDescriptor)
          .Return (typeDescriptorMock).Repeat.AtLeastOnce();
      typeDescriptorMock
          .Expect (x => x.Matches (method))
          .Return (true);
      schedulerMock
          .Expect (
              x => x.Sort (
                  Arg<IEnumerable<Tuple<IAspectDescriptor, IExpressionGenerator>>>
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

      //// Act
      //var assembler = new MethodAssembler (
      //    configurationMock, fieldIntroducerMock, giveMeSomeNameMock, schedulerMock, methodCopierMock, constructorPatcherMock, factoryMock);
      //assembler.ModifyMethod (mutableType, method, typeGenerators);

      //// Assert
      //mockRepository.VerifyAll();
    }

    // TODO test name
    [Test]
    [Ignore]
    public void ProceedOnlyIfAny_TypeGenerator ()
    {
      var configurationStub = MockRepository.GenerateStub<IActiveAttributesConfiguration>();
      var fieldIntroducerStub = MockRepository.GenerateStub<IFieldIntroducer>();
      var giveMeSomeNameStub = MockRepository.GenerateStub<IGiveMeSomeName>();
      var schedulerStub = MockRepository.GenerateStub<IAspectSorter>();
      var methodCopierStub = MockRepository.GenerateStub<IMethodCopier>();
      var constructorPatcherStub = MockRepository.GenerateStub<IConstructorPatcher>();
      var factoryStub = MockRepository.GenerateStub<IFactory>();

      var mutableType = MutableTypeObjectMother.Create();
      var method = mutableType.GetMethods().First();

      var expressionGeneratorMock = MockRepository.GenerateStrictMock<IExpressionGenerator>();
      var descriptorMock = MockRepository.GenerateStrictMock<IAspectDescriptor>();
      var methodPatcherMock = MockRepository.GenerateStrictMock<IMethodPatcher>();

      // Arrange
      configurationStub
          .Stub (x => x.AspectDescriptorProviders)
          .Return (new IAspectDescriptorProvider[0]);
      giveMeSomeNameStub
          .Stub (x => x.IntroduceExpressionGenerators (null, null, new FieldInfoContainer()))
          .IgnoreArguments()
          .Return (Enumerable.Empty<IExpressionGenerator>());
      factoryStub
          .Stub (x => x.GetMethodPatcher (null, new FieldInfoContainer(), null, null))
          .IgnoreArguments()
          .Return (methodPatcherMock);

      expressionGeneratorMock
          .Expect (x => x.AspectDescriptor)
          .Return (descriptorMock);
      descriptorMock
          .Expect (x => x.Matches (null))
          .IgnoreArguments()
          .Return (false);

      //// Act
      //var assembler = new MethodAssembler (
      //    configurationStub, fieldIntroducerStub, giveMeSomeNameStub, schedulerStub, methodCopierStub, constructorPatcherStub, factoryStub);
      //assembler.ModifyMethod (mutableType, method, new[] { expressionGeneratorMock });

      //// Assert
      //expressionGeneratorMock.VerifyAllExpectations();
      //descriptorMock.VerifyAllExpectations();
    }

    // TODO test name
    [Test]
    [Ignore]
    public void ProceedOnlyIfAny_MethodGenerator ()
    {
      var configurationStub = MockRepository.GenerateStub<IActiveAttributesConfiguration>();
      var fieldIntroducerStub = MockRepository.GenerateStub<IFieldIntroducer>();
      var giveMeSomeNameStub = MockRepository.GenerateStub<IGiveMeSomeName>();
      var schedulerStub = MockRepository.GenerateStub<IAspectSorter>();
      var methodCopierStub = MockRepository.GenerateStub<IMethodCopier>();
      var constructorPatcherStub = MockRepository.GenerateStub<IConstructorPatcher>();
      var factoryStub = MockRepository.GenerateStub<IFactory>();

      var mutableType = MutableTypeObjectMother.Create();
      var method = mutableType.GetMethods().First();

      var expressionGeneratorMock = MockRepository.GenerateStrictMock<IExpressionGenerator>();
      var descriptorMock = MockRepository.GenerateStrictMock<IAspectDescriptor>();
      var methodPatcherMock = MockRepository.GenerateStrictMock<IMethodPatcher>();

      // Arrange
      configurationStub
          .Stub (x => x.AspectDescriptorProviders)
          .Return (new IAspectDescriptorProvider[0]);
      giveMeSomeNameStub
          .Stub (x => x.IntroduceExpressionGenerators (null, null, new FieldInfoContainer()))
          .IgnoreArguments()
          .Return (Enumerable.Empty<IExpressionGenerator>());
      factoryStub
          .Stub (x => x.GetMethodPatcher (null, new FieldInfoContainer(), null, null))
          .IgnoreArguments()
          .Return (methodPatcherMock);

      // Act
      //var assembler = new MethodAssembler (
      //    configurationStub, fieldIntroducerStub, giveMeSomeNameStub, schedulerStub, methodCopierStub, constructorPatcherStub, factoryStub);
      //assembler.ModifyMethod (mutableType, method, new IExpressionGenerator[0]);

      //// Assert
      //expressionGeneratorMock.VerifyAllExpectations();
      //descriptorMock.VerifyAllExpectations();
    }

    private IFieldIntroducer _fieldIntroducerMock;
    private IGiveMeSomeName _giveMeSomeNameMock;
    private IAspectSorter _aspectSorterStub;
    private IMethodCopier _methodCopierStub;
    private IConstructorPatcher _constructorPatcherMock;
    private IFactory _factoryStub;
    private MutableType _mutableType;
    private MethodInfo _methodInfo;
    private FieldInfoContainer _fakeFields;
    private ITypeProvider _fakeTypeProvider;
    private IMethodPatcher _methodPatcherMock;

    private IAspectDescriptor _fakeDescriptorA;
    private IAspectDescriptor _fakeDescriptorB;
    private IAspectDescriptor _fakeDescriptorC;

    private IExpressionGenerator _fakeExpressionGeneratorA;
    private IExpressionGenerator _fakeExpressionGeneratorB;
    private IExpressionGenerator _fakeExpressionGeneratorC;

    private MutableMethodInfo _fakeCopiedMethod;
    private MutableMethodInfo _mutableMethod;

    [SetUp]
    public void SetUp ()
    {
      _fieldIntroducerMock = MockRepository.GenerateStrictMock<IFieldIntroducer> ();
      _giveMeSomeNameMock = MockRepository.GenerateStrictMock<IGiveMeSomeName> ();
      _aspectSorterStub = MockRepository.GenerateStub<IAspectSorter> ();
      _methodCopierStub = MockRepository.GenerateStub<IMethodCopier> ();
      _constructorPatcherMock = MockRepository.GenerateStrictMock<IConstructorPatcher> ();
      _factoryStub = MockRepository.GenerateStub<IFactory> ();

      _mutableType = MutableTypeObjectMother.Create ();
      _methodInfo = _mutableType.GetMethods().First();
      _fakeFields = CreateFieldInfoContainer ();

      _fakeTypeProvider = MockRepository.GenerateStub<ITypeProvider>();
      _methodPatcherMock = MockRepository.GenerateStrictMock<IMethodPatcher>();

      _fakeDescriptorA = CreateDescriptor ();
      _fakeDescriptorB = CreateDescriptor ();
      _fakeDescriptorC = CreateDescriptor ();

      _fakeExpressionGeneratorA = CreateExpressionGenerator (_fakeDescriptorA);
      _fakeExpressionGeneratorB = CreateExpressionGenerator (_fakeDescriptorB);
      _fakeExpressionGeneratorC = CreateExpressionGenerator (_fakeDescriptorC);

      _fakeCopiedMethod = MutableMethodInfoObjectMother.Create ();
      _mutableMethod = _mutableType.GetOrAddMutableMethod (_methodInfo);
    }

    [Test]
    public void ModifyMethod_NoMethodLevelAspects_NoTypeLevelAspects ()
    {
      var methodAssembler = CreateMethodAssembler (new IMethodLevelAspectDescriptorProvider[0]);

      _fieldIntroducerMock.Expect (mock => mock.IntroduceMethodFields (_mutableType, _methodInfo)).Return (_fakeFields);
      _giveMeSomeNameMock
          .Expect (
              mock => mock.IntroduceExpressionGenerators (
                  Arg.Is (_mutableType),
                  Arg<IEnumerable<IAspectDescriptor>>.List.Equal (new IAspectDescriptor[0]),
                  Arg.Is (_fakeFields)))
          .Return (new IExpressionGenerator[0]);

      methodAssembler.ModifyMethod (_mutableType, _methodInfo, new IExpressionGenerator[0]);

      _fieldIntroducerMock.VerifyAllExpectations();
      _giveMeSomeNameMock.VerifyAllExpectations();
      _constructorPatcherMock.AssertWasNotCalled (
          mock => mock.AddReflectionAndDelegateInitialization (
              Arg<MutableMethodInfo>.Is.Anything,
              Arg<FieldInfoContainer>.Is.Anything,
              Arg<MutableMethodInfo>.Is.Anything));
    }

    [Test]
    public void ModifyMethod_WithMethodLevelAspects_NoTypeLevelAspects ()
    {
      var methodLevelDescriptorProviderStub1 = CreateMethodLevelDescriptorProvider (_fakeDescriptorA, _fakeDescriptorB);
      var methodLevelDescriptorProviderStub2 = CreateMethodLevelDescriptorProvider (_fakeDescriptorC);

      var methodAssembler = CreateMethodAssembler (new[] { methodLevelDescriptorProviderStub1, methodLevelDescriptorProviderStub2 });

      _fieldIntroducerMock.Expect (mock => mock.IntroduceMethodFields (_mutableType, _methodInfo)).Return (_fakeFields);
      _giveMeSomeNameMock
          .Expect (
              mock => mock.IntroduceExpressionGenerators (
                  Arg.Is (_mutableType),
                  Arg<IEnumerable<IAspectDescriptor>>.List.Equal (new[] { _fakeDescriptorA, _fakeDescriptorB, _fakeDescriptorC }),
                  Arg.Is (_fakeFields)))
          .Return (new[] { _fakeExpressionGeneratorA, _fakeExpressionGeneratorB, _fakeExpressionGeneratorC });

      var fakeSortedGenerators = new[] { _fakeExpressionGeneratorC, _fakeExpressionGeneratorA, _fakeExpressionGeneratorB };
      var tuplesToBeSorted = 
          new[]
          {
              Tuple.Create (_fakeDescriptorA, _fakeExpressionGeneratorA), 
              Tuple.Create (_fakeDescriptorB, _fakeExpressionGeneratorB),
              Tuple.Create (_fakeDescriptorC, _fakeExpressionGeneratorC)
          };
      _aspectSorterStub.Stub (stub => stub.Sort (ArgEnumerable (tuplesToBeSorted))).Return (fakeSortedGenerators);

      _methodCopierStub
          .Stub (stub => stub.GetCopy (Arg<MutableMethodInfo>.Matches (mi => mi == _mutableMethod)))
          .Return (_fakeCopiedMethod);

      _constructorPatcherMock.Expect (mock => mock.AddReflectionAndDelegateInitialization (_mutableMethod, _fakeFields, _fakeCopiedMethod));

      _factoryStub.Stub (stub => stub.GetTypeProvider (_methodInfo)).Return (_fakeTypeProvider);
      _factoryStub.Stub (stub => stub.GetMethodPatcher (_mutableMethod, _fakeFields, fakeSortedGenerators, _fakeTypeProvider)).Return (_methodPatcherMock);

      _methodPatcherMock.Expect (mock => mock.AddMethodInterception());

      methodAssembler.ModifyMethod (_mutableType, _methodInfo, new IExpressionGenerator[0]);

      _fieldIntroducerMock.VerifyAllExpectations ();
      _giveMeSomeNameMock.VerifyAllExpectations ();
      _constructorPatcherMock.VerifyAllExpectations();
      _methodPatcherMock.VerifyAllExpectations();
    }

    private IExpressionGenerator CreateExpressionGenerator (IAspectDescriptor aspectDescriptor)
    {
      var expressionGeneratorStub = MockRepository.GenerateStub<IExpressionGenerator>();
      expressionGeneratorStub.Stub (stub => stub.AspectDescriptor).Return (aspectDescriptor);
      return expressionGeneratorStub;
    }

    private IMethodLevelAspectDescriptorProvider CreateMethodLevelDescriptorProvider (params IAspectDescriptor[] fakeDescriptors)
    {
      var methodLevelDescriptorProviderStub = MockRepository.GenerateStub<IMethodLevelAspectDescriptorProvider>();
      methodLevelDescriptorProviderStub.Stub (stub => stub.GetDescriptors (_methodInfo)).Return (fakeDescriptors);
      return methodLevelDescriptorProviderStub;
    }

    private IAspectDescriptor CreateDescriptor ()
    {
      return MockRepository.GenerateStub<IAspectDescriptor>();
    }

    private IEnumerable<T> ArgEnumerable<T> (params T[] items)
    {
      return Arg<IEnumerable<T>>.List.Equal (items);
    }

    private MethodAssembler CreateMethodAssembler (IMethodLevelAspectDescriptorProvider[] aspectDescriptorProviders)
    {
      return new MethodAssembler (
          aspectDescriptorProviders,
          _fieldIntroducerMock,
          _giveMeSomeNameMock,
          _aspectSorterStub,
          _methodCopierStub,
          _constructorPatcherMock,
          _factoryStub);
    }

    private static FieldInfoContainer CreateFieldInfoContainer ()
    {
      return new FieldInfoContainer();
    }
  }
}