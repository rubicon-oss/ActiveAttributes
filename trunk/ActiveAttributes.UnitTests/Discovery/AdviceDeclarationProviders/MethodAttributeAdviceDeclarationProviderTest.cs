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
using System.Linq;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.AdviceDeclarationProviders;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace ActiveAttributes.UnitTests.Discovery.AdviceDeclarationProviders
{
  [TestFixture]
  public class MethodAttributeAdviceDeclarationProviderTest
  {
    [Test]
    public void GetDeclarations ()
    {
      var aspectDeclarationHelperMock = MockRepository.GenerateStrictMock<IAttributeDeclarationProvider> ();
      var relatedMethodFinderMock = MockRepository.GenerateStrictMock<IRelatedMethodFinder> ();
      var method = ObjectMother2.GetMethodInfo ();
      var fakeMethod = ObjectMother2.GetMethodInfo ();
      var fakeAdviceBuilder1 = ObjectMother2.GetAdviceBuilder ();
      var fakeAdviceBuilder2 = ObjectMother2.GetAdviceBuilder ();
      var fakeAdviceBuilder3 = ObjectMother2.GetAdviceBuilder ();

      relatedMethodFinderMock.Expect (x => x.GetBaseMethod (method)).Return (fakeMethod);
      relatedMethodFinderMock.Expect (x => x.GetBaseMethod (fakeMethod)).Return (null);
      aspectDeclarationHelperMock.Expect (x => x.GetAdviceBuilders (method)).Return (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2 }.AsOneTime());
      aspectDeclarationHelperMock.Expect (x => x.GetAdviceBuilders (fakeMethod)).Return (new[] { fakeAdviceBuilder3 }.AsOneTime());

      var provider = new MethodAttributeAdviceDeclarationProvider (aspectDeclarationHelperMock, relatedMethodFinderMock);
      var result = provider.GetDeclarations (method).ToArray ();

      relatedMethodFinderMock.VerifyAllExpectations ();
      aspectDeclarationHelperMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { fakeAdviceBuilder1, fakeAdviceBuilder2, fakeAdviceBuilder3 }));
    }
  }
}