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
using ActiveAttributes.Advices;
using ActiveAttributes.Assembly;
using ActiveAttributes.Declaration;
using ActiveAttributes.Declaration.DeclarationProviders;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AssemblerTest
  {
    private Assembler _assembler;

    private IDeclarationProvider _adviceDeclarationProviderMock;
    private IWeaver _weaverMock;
    private IAdviceComposer _adviceComposerMock;
    private MutableType _mutableType;

    [SetUp]
    public void SetUp ()
    {
      _adviceDeclarationProviderMock = MockRepository.GenerateStrictMock<IDeclarationProvider> ();
      _weaverMock = MockRepository.GenerateStrictMock<IWeaver> ();
      _adviceComposerMock = MockRepository.GenerateStrictMock<IAdviceComposer>();
      _mutableType = ObjectMother.GetMutableType ();

      _assembler = new Assembler (_adviceDeclarationProviderMock, _adviceComposerMock, _weaverMock);
    }

    [Test]
    public void ModifyType ()
    {
      var fakeAssemblyAdviceDeclarations = new[] { ObjectMother.GetAdviceBuilder() };
      var fakeTypeAdviceDeclarations = new[] { ObjectMother.GetAdviceBuilder() };
      var fakeMethodAdviceDeclarations = new[] { ObjectMother.GetAdviceBuilder() };
      var allAdviceBuilders =
          fakeAssemblyAdviceDeclarations
              .Concat (fakeTypeAdviceDeclarations)
              .Concat (fakeMethodAdviceDeclarations);

      _adviceDeclarationProviderMock.Expect (x => x.GetDeclarations()).Return (fakeAssemblyAdviceDeclarations.AsOneTime());
      _adviceDeclarationProviderMock.Expect (x => x.GetDeclarations (_mutableType)).Return (fakeTypeAdviceDeclarations.AsOneTime());

      foreach (var method in _mutableType.GetMethods())
      {
        var method1 = method;
        var fakeAdvices = method.Name == "ToString" ? new[] { ObjectMother.GetAdvice() } : new Advice[0];

        _adviceDeclarationProviderMock
            .Expect (x => x.GetDeclarations (method))
            .Return (fakeMethodAdviceDeclarations);
        _adviceComposerMock
            .Expect (
                x => x.Compose (
                    Arg<IEnumerable<IAdviceBuilder>>.List.Equal (allAdviceBuilders),
                    Arg<JoinPoint>.Matches (y => y.Member == method1)))
            .Return (fakeAdvices.AsOneTime());

        if (method.Name == "ToString")
        {
          _weaverMock
              .Expect (x => x.Weave (Arg<MethodInfo>.Is.Anything, Arg.Is (fakeAdvices)))
              .WhenCalled (x =>
              {
                var mutableMethod = (MutableMethodInfo) x.Arguments[0];
                Assert.That (mutableMethod.Name, Is.EqualTo (method.Name));
              });
        }
      }

      _assembler.ModifyType (_mutableType);

      _adviceDeclarationProviderMock.VerifyAllExpectations();
      _adviceComposerMock.VerifyAllExpectations();
      _weaverMock.VerifyAllExpectations();
    }

    [Test]
    public void ModifyType_ContinuesForEmptyAdvices ()
    {
      var emptyAdvices = new IAdviceBuilder[0];
      _adviceDeclarationProviderMock.Expect (x => x.GetDeclarations ()).Return (emptyAdvices);
      _adviceDeclarationProviderMock.Expect (x => x.GetDeclarations (_mutableType)).Return (emptyAdvices);
      _adviceDeclarationProviderMock.Expect (x => x.GetDeclarations (Arg<MethodInfo>.Is.Anything)).Return (emptyAdvices).Repeat.Any();
      _adviceComposerMock
        .Expect (x => x.Compose (Arg<IEnumerable<IAdviceBuilder>>.Is.Anything, Arg<JoinPoint>.Is.Anything))
        .Return (new Advice[0]).Repeat.Any ();

      _assembler.ModifyType (_mutableType);

      _weaverMock.AssertWasNotCalled (x => x.Weave (Arg<MethodInfo>.Is.Anything, Arg<IEnumerable<Advice>>.Is.Anything));
    }
  }
}