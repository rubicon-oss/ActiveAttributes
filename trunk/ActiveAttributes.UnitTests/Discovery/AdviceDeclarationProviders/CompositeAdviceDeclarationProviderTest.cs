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

namespace ActiveAttributes.UnitTests.Discovery.AdviceDeclarationProviders
{
  [TestFixture]
  public class CompositeAdviceDeclarationProviderTest
  {
    [Test]
    public void GetDeclarations ()
    {
      var mockRepository = new MockRepository();

      var assemblyLevelAdviceDeclarationProviderMock1 = mockRepository.StrictMock<IAssemblyLevelAdviceDeclarationProvider>();
      var assemblyLevelAdviceDeclarationProviderMock2 = mockRepository.StrictMock<IAssemblyLevelAdviceDeclarationProvider>();
      var typeLevelAdviceDeclarationProviderMock1 = mockRepository.StrictMock<ITypeLevelAdviceDeclarationProvider>();
      var typeLevelAdviceDeclarationProviderMock2 = mockRepository.StrictMock<ITypeLevelAdviceDeclarationProvider>();
      var methodLevelAdviceDeclarationProviderMock1 = mockRepository.StrictMock<IMethodLevelAdviceDeclarationProvider>();
      var methodLevelAdviceDeclarationProviderMock2 = mockRepository.StrictMock<IMethodLevelAdviceDeclarationProvider>();

      var fakeDeclarations1 = new[] { ObjectMother2.GetAdviceBuilder() };
      var fakeDeclarations2 = new[] { ObjectMother2.GetAdviceBuilder() };
      var fakeDeclarations3 = new[] { ObjectMother2.GetAdviceBuilder() };
      var fakeDeclarations4 = new[] { ObjectMother2.GetAdviceBuilder() };
      var fakeDeclarations5 = new[] { ObjectMother2.GetAdviceBuilder() };
      var fakeDeclarations6 = new[] { ObjectMother2.GetAdviceBuilder() };

      var type = ObjectMother2.GetDeclaringType();
      var method = ObjectMother2.GetMethodInfo();

      assemblyLevelAdviceDeclarationProviderMock1.Expect (x => x.GetDeclarations()).Return (fakeDeclarations1);
      assemblyLevelAdviceDeclarationProviderMock2.Expect (x => x.GetDeclarations()).Return (fakeDeclarations2);
      typeLevelAdviceDeclarationProviderMock1.Expect (x => x.GetDeclarations (type)).Return (fakeDeclarations3);
      typeLevelAdviceDeclarationProviderMock2.Expect (x => x.GetDeclarations (type)).Return (fakeDeclarations4);
      methodLevelAdviceDeclarationProviderMock1.Expect (x => x.GetDeclarations (method)).Return (fakeDeclarations5);
      methodLevelAdviceDeclarationProviderMock2.Expect (x => x.GetDeclarations (method)).Return (fakeDeclarations6);
      mockRepository.ReplayAll();

      var composite = new CompositeAdviceDeclarationProvider (
          new[] { assemblyLevelAdviceDeclarationProviderMock1, assemblyLevelAdviceDeclarationProviderMock2 }.AsOneTime(),
          new[] { typeLevelAdviceDeclarationProviderMock1, typeLevelAdviceDeclarationProviderMock2 }.AsOneTime(),
          new[] { methodLevelAdviceDeclarationProviderMock1, methodLevelAdviceDeclarationProviderMock2 }.AsOneTime());

      var assemblyResult = composite.GetDeclarations().ToList();
      var typeResult = composite.GetDeclarations (type).ToList();
      var methodResult = composite.GetDeclarations (method).ToList();

      mockRepository.VerifyAll();

      var actual = assemblyResult.Concat (typeResult).Concat (methodResult);
      var expected = Concat (fakeDeclarations1, fakeDeclarations2, fakeDeclarations3, fakeDeclarations4, fakeDeclarations5, fakeDeclarations6);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    private IEnumerable<IAdviceBuilder> Concat (params IEnumerable<IAdviceBuilder>[] declarations)
    {
      return declarations.SelectMany (x => x);
    }
  }
}