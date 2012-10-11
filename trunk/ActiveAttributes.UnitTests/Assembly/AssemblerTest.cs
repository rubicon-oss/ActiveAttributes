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
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Providers;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  [Ignore]
  public class AssemblerTest
  {
    private ITypeLevelAspectProvider _typeLevelAspectProvider;
    private IMethodLevelAspectProvider _methodLevelAspectProvider;
    private IFieldIntroducer _fieldIntroducer;
    private IMethodPatcher _methodPatcher;
    private IAspectScheduler _scheduler;
    private IMethodCopier _copier;
    private IConstructorPatcher _constructorPatcher;
    private IFactory _factory;

    private Assembler2 _assembler;

    private MutableType _mutableType1;

    [SetUp]
    public void SetUp ()
    {
      _typeLevelAspectProvider = MockRepository.GenerateMock<ITypeLevelAspectProvider>();
      _methodLevelAspectProvider = MockRepository.GenerateMock<IMethodLevelAspectProvider>();
      _fieldIntroducer = MockRepository.GenerateMock<IFieldIntroducer>();
      _constructorPatcher = MockRepository.GenerateMock<IConstructorPatcher>();
      _methodPatcher = MockRepository.GenerateMock<IMethodPatcher>();
      _scheduler = MockRepository.GenerateMock<IAspectScheduler>();
      _copier = MockRepository.GenerateMock<IMethodCopier>();
      _factory = MockRepository.GenerateMock<IFactory>();

      _assembler = new Assembler2 (
          new IAspectProvider[] { _typeLevelAspectProvider, _methodLevelAspectProvider },
          _fieldIntroducer,
          _constructorPatcher,
          _copier,
          _factory,
          _scheduler);

      _mutableType1 = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
    }

    [Test]
    public void GetsTypeLevelAspects ()
    {
      var mockRepository = new MockRepository ();

      var typeLevelAspectProvider = mockRepository.StrictMock<ITypeLevelAspectProvider>();
      var methodLevelAspectProvider = mockRepository.StrictMock<IMethodLevelAspectProvider>();

      var typeLevelAspectsFake = mockRepository.DynamicMock<IAspectDescriptor>();
      var methodLevelAspectFake = mockRepository.DynamicMock<IAspectDescriptor>();

      using (mockRepository.Ordered())
      {
        typeLevelAspectProvider
            .Expect (x => x.GetAspects (_mutableType1.UnderlyingSystemType))
            .Return (new[] { typeLevelAspectsFake });

        var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
        methodLevelAspectProvider
            .Expect (x => x.GetAspects (null))
            .IgnoreArguments()
            .Return (new[] { methodLevelAspectFake })
            .Repeat.AtLeastOnce();
        typeLevelAspectsFake
            .Expect (x => x.Matches (method))
            .Return (true);
      }


      _assembler = new Assembler2 (
          new IAspectProvider[] { typeLevelAspectProvider, methodLevelAspectProvider },
          _fieldIntroducer,
          _constructorPatcher,
          _copier,
          _factory,
          _scheduler);
      mockRepository.ReplayAll();

      _assembler.ModifyType (_mutableType1);

      mockRepository.VerifyAll();
    }



    class DomainType
    {
      public virtual void Method () { }
    }
  }
}