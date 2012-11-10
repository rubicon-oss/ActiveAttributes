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
using ActiveAttributes.Core;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.Construction;
using ActiveAttributes.Core.Discovery.DeclarationProviders;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace ActiveAttributes.UnitTests.Discovery.DeclarationProviders
{
  [TestFixture]
  public class AspectClassDeclarationProviderTest
  {
    [Test]
    public void GetDeclarations ()
    {
      var typeDiscoveryServiceMock = MockRepository.GenerateStrictMock<ITypeDiscoveryService>();
      var classDeclarationProviderMock = MockRepository.GenerateStrictMock<IClassDeclarationProvider>();
      var provider = new AspectClassDeclarationProvider (typeDiscoveryServiceMock, classDeclarationProviderMock);

      var aspectTypes = new[] { typeof (DomainAspect1), typeof (DomainAspect2) };
      var adviceBuilderMock1 = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var adviceBuilderMock2 = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var adviceBuilderMock3 = MockRepository.GenerateStrictMock<IAdviceBuilder>();

      IConstruction construction1 = null;
      IConstruction construction2 = null;
      typeDiscoveryServiceMock.Expect (x => x.GetTypes (typeof (IAspect), false)).Return (aspectTypes);
      classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect1)))
          .Return (new[] { adviceBuilderMock1, adviceBuilderMock2 }.AsOneTime());
      classDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (typeof (DomainAspect2)))
          .Return (new[] { adviceBuilderMock3 });
      adviceBuilderMock1
          .Expect (x => x.SetConstruction (Arg<IConstruction>.Matches (y => y.ConstructorInfo.DeclaringType == typeof (DomainAspect1))))
          .WhenCalled (x => construction1 = (IConstruction) x.Arguments[0])
          .Return (adviceBuilderMock1);
      adviceBuilderMock2
          .Expect (x => x.SetConstruction (Arg<IConstruction>.Matches (y => y.ConstructorInfo.DeclaringType == typeof (DomainAspect1))))
          .WhenCalled (x => construction2 = (IConstruction) x.Arguments[0])
          .Return (adviceBuilderMock2);
      adviceBuilderMock3
          .Expect (x => x.SetConstruction (Arg<IConstruction>.Matches (y => y.ConstructorInfo.DeclaringType == typeof (DomainAspect2))))
          .Return (adviceBuilderMock3);

      var result = provider.GetDeclarations().ToList();

      typeDiscoveryServiceMock.VerifyAllExpectations();
      classDeclarationProviderMock.VerifyAllExpectations();
      adviceBuilderMock1.VerifyAllExpectations();
      adviceBuilderMock2.VerifyAllExpectations();
      adviceBuilderMock3.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { adviceBuilderMock1, adviceBuilderMock2, adviceBuilderMock3 }));
      Assert.That (construction1, Is.SameAs (construction2));
    }

    class DomainAspect1 : IAspect
    {
       
    }
    class DomainAspect2 : IAspect
    {
       
    }
  }
}