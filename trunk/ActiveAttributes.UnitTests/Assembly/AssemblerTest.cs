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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Providers;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AssemblerTest
  {
    private IFieldIntroducer _introducer;
    private IMethodPatcher _methodPatcher;
    private IAspectScheduler _scheduler;
    private IMethodCopier _copier;
    private IConstructorPatcher _constructorPatcher;
    private IFactory _factory;

    private Assembler _assembler;

    private MutableType _mutableType1;
    private MutableType _mutableType2;

    [SetUp]
    public void SetUp ()
    {
      _introducer = MockRepository.GenerateMock<IFieldIntroducer>();
      _methodPatcher = MockRepository.GenerateMock<IMethodPatcher>();
      _scheduler = MockRepository.GenerateMock<IAspectScheduler>();
      _copier = MockRepository.GenerateMock<IMethodCopier>();
      _constructorPatcher = MockRepository.GenerateMock<IConstructorPatcher>();
      _factory = MockRepository.GenerateMock<IFactory>();

      _assembler = new Assembler (null, _introducer, _constructorPatcher, _copier, _factory, _scheduler);

      _mutableType1 = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType1));

    }

    [Ignore]
    [Test]
    public void GetsTypeLevelAspects ()
    {
      _assembler.ModifyType (_mutableType1);

    }



    private class DomainType1
    {
    }
  }
}