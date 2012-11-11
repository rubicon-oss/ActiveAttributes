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
using System.ComponentModel.Design;
using System.Linq;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.DeclarationProviders;
using ActiveAttributes.UnitTests.Discovery.DeclarationProviders;
using NUnit.Framework;
using Rhino.Mocks;

[assembly: AssemblyAttributeDeclarationProviderTest.DomainAspectAttribute]

namespace ActiveAttributes.UnitTests.Discovery.DeclarationProviders
{
  [TestFixture]
  public class AssemblyAttributeDeclarationProviderTest
  {
    [Test]
    public void GetDeclarations ()
    {
      var typeDiscoveryServiceMock = MockRepository.GenerateStrictMock<ITypeDiscoveryService>();
      var attributeDeclarationProviderMock = MockRepository.GenerateStrictMock<IAttributeDeclarationProvider>();
      var provider = new AssemblyAttributeDeclarationProvider (typeDiscoveryServiceMock, attributeDeclarationProviderMock);
      var fakeAdviceBuilder1 = ObjectMother2.GetAdviceBuilder();
      var fakeAdviceBuilder2 = ObjectMother2.GetAdviceBuilder();

      var type1 = typeof (DomainAspectAttribute);
      var type2 = typeof (AspectAttributeBase);
      typeDiscoveryServiceMock.Expect (x => x.GetTypes (type2, false)).Return (new[] { type1, type2 });
      attributeDeclarationProviderMock.Expect (x => x.GetAdviceBuilders (type1.Assembly)).Return (new[] { fakeAdviceBuilder1 });
      attributeDeclarationProviderMock.Expect (x => x.GetAdviceBuilders (type2.Assembly)).Return (new[] { fakeAdviceBuilder2 });

      var result = provider.GetDeclarations().ToArray();

      typeDiscoveryServiceMock.VerifyAllExpectations();
      attributeDeclarationProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2 }));
    }

    [AttributeUsage (AttributeTargets.Assembly)]
    public class DomainAspectAttribute : AspectAttributeBase {}
  }
}