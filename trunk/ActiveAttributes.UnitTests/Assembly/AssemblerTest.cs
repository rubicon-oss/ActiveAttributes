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
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.AdviceDeclarationProviders;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Enumerables;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AssemblerTest
  {
    [Test]
    public void ModifyType ()
    {
      var adviceDeclarationProviderMock = MockRepository.GenerateStrictMock<IAdviceDeclarationProvider>();
      var weaverMock = MockRepository.GenerateStrictMock<IWeaver>();
      var adviceComposerMock = MockRepository.GenerateStrictMock<IAdviceComposer>();
      var mutableType = ObjectMother2.GetMutableType();

      var fakeAssemblyAdviceDeclarations = new[] { ObjectMother2.GetAdviceBuilder() };
      var fakeTypeAdviceDeclarations = new[] { ObjectMother2.GetAdviceBuilder() };
      var fakeMethodAdviceDeclarations = new[] { ObjectMother2.GetAdviceBuilder() };

      adviceDeclarationProviderMock.Expect (x => x.GetDeclarations()).Return (fakeAssemblyAdviceDeclarations.AsOneTime());
      adviceDeclarationProviderMock.Expect (x => x.GetDeclarations (mutableType)).Return (fakeTypeAdviceDeclarations.AsOneTime());

      foreach (var method in mutableType.GetMethods())
      {
        var method1 = method;
        var fakeAdvices = new[] { ObjectMother2.GetAdvice() };

        adviceDeclarationProviderMock.Expect (x => x.GetDeclarations (method)).Return (fakeMethodAdviceDeclarations);
        var allAdviceBuilders = fakeAssemblyAdviceDeclarations.Concat (fakeTypeAdviceDeclarations).Concat (fakeMethodAdviceDeclarations);
        adviceComposerMock
            .Expect (
                x => x.Compose (
                    Arg<IEnumerable<IAdviceBuilder>>.List.Equal (allAdviceBuilders),
                    Arg<JoinPoint>.Matches (y => y.Member == method1)))
            .Return (fakeAdvices.AsOneTime());

        weaverMock.Expect (x => x.Weave (method1, fakeAdvices));
      }

      new Assembler (adviceDeclarationProviderMock, adviceComposerMock, weaverMock).ModifyType (mutableType);

      adviceDeclarationProviderMock.VerifyAllExpectations();
      adviceComposerMock.VerifyAllExpectations();
      weaverMock.VerifyAllExpectations();
    }
  }
}