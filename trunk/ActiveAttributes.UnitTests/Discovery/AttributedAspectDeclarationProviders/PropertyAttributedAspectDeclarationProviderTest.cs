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
using ActiveAttributes.Core.Discovery.AttributedAspectDeclarationProviders;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Discovery.AttributedAspectDeclarationProviders
{
  [TestFixture]
  public class PropertyAttributedAspectDeclarationProviderTest
  {
    [Test]
    public void GetDeclarations ()
    {
      var aspectDeclarationHelperMock = MockRepository.GenerateStrictMock<IAspectDeclarationHelper> ();
      var relatedPropertyFinderMock = MockRepository.GenerateStrictMock<IRelatedPropertyFinder> ();
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.Property);
      var method = property.GetGetMethod ();
      var fakeProperty = ObjectMother2.GetPropertyInfo ();
      var fakeDeclaration1 = ObjectMother2.GetAspectDeclaration ();
      var fakeDeclaration2 = ObjectMother2.GetAspectDeclaration ();
      var fakeDeclaration3 = ObjectMother2.GetAspectDeclaration ();

      relatedPropertyFinderMock.Expect (x => x.GetBaseProperty (property)).Return (fakeProperty);
      relatedPropertyFinderMock.Expect (x => x.GetBaseProperty (fakeProperty)).Return (null);
      aspectDeclarationHelperMock.Expect (x => x.GetAspectDeclarations (property)).Return (new[] { fakeDeclaration1, fakeDeclaration2 });
      aspectDeclarationHelperMock.Expect (x => x.GetAspectDeclarations (fakeProperty)).Return (new[] { fakeDeclaration3 });

      var provider = new PropertyAttributedAspectDeclarationProvider (aspectDeclarationHelperMock, relatedPropertyFinderMock);
      var result = provider.GetDeclarations (method).ToArray ();

      relatedPropertyFinderMock.VerifyAllExpectations ();
      aspectDeclarationHelperMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo (new[] { fakeDeclaration1, fakeDeclaration2, fakeDeclaration3 }));
    }

    class DomainType
    {
      public string Property { get; set; }
    }
  }
}