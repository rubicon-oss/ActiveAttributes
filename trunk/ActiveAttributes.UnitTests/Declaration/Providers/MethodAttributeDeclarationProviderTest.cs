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
using System.Reflection;
using ActiveAttributes.Declaration;
using ActiveAttributes.Declaration.Providers;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Declaration.Providers
{
  [TestFixture]
  public class MethodAttributeDeclarationProviderTest
  {
    [Test]
    public void GetDeclarations ()
    {
      var memberAttributeDeclarationProviderMock = MockRepository.GenerateStrictMock<IMemberAttributeDeclarationProvider> ();
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainTypeBase obj) => obj.Method());
      var fakeAdviceBuilder = new IAdviceBuilder[0];

      memberAttributeDeclarationProviderMock
        .Expect (x => x.GetAdviceBuilders (Arg.Is(method), Arg<IEnumerable<MemberInfo>>.List.Equal(new[] { method, baseMethod })))
        .Return (fakeAdviceBuilder);

      var provider = new MethodAttributeDeclarationProvider (memberAttributeDeclarationProviderMock);
      var result = provider.GetDeclarations (method);

      memberAttributeDeclarationProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeAdviceBuilder));
    }

    [Test]
    public void Resolution ()
    {
      var instances = SafeServiceLocator.Current.GetAllInstances<IMethodLevelDeclarationProvider>();

      Assert.That (instances, Has.Some.TypeOf<MethodAttributeDeclarationProvider>());
    }

    class DomainTypeBase
    {
      public virtual void Method () {}
    }

    class DomainType : DomainTypeBase
    {
      public override void Method () {}
    }
  }
}