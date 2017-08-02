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
using ActiveAttributes.Aspects2;
using ActiveAttributes.Declaration;
using ActiveAttributes.Declaration.Providers;
using ActiveAttributes.UnitTests.Declaration.Providers;
using ActiveAttributes.Utilities;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

[assembly: AssemblyAttributeDeclarationProviderTest.DomainAspectAttribute]

namespace ActiveAttributes.UnitTests.Declaration.Providers
{
  [TestFixture]
  public class AssemblyAttributeDeclarationProviderTest
  {
    private AssemblyAttributeDeclarationProvider _provider;

    private IAttributeDeclarationProvider _attributeDeclarationProviderMock;
    private IAspectTypesProvider _aspectTypesProviderMock;
    private ICustomAttributeDataHelper _customAttributeDataHelperMock;

    [SetUp]
    public void SetUp ()
    {
      _attributeDeclarationProviderMock = MockRepository.GenerateStrictMock<IAttributeDeclarationProvider>();
      _aspectTypesProviderMock = MockRepository.GenerateStrictMock<IAspectTypesProvider>();
      _customAttributeDataHelperMock = MockRepository.GenerateStrictMock<ICustomAttributeDataHelper>();

      _provider = new AssemblyAttributeDeclarationProvider (
          _aspectTypesProviderMock,
          _attributeDeclarationProviderMock,
          _customAttributeDataHelperMock);
    }

    //[Test]
    //public void GetDeclarations ()
    //{
    //  var fakeAdviceBuilder = ObjectMother.GetAdviceBuilder();
    //  var type = typeof (AssemblyWithAttribute.DomainAspectAttribute);

    //  _aspectTypesProviderMock.Expect (x => x.GetAspectAttributeTypes()).Return (new[] { type });
    //  _customAttributeDataHelperMock
    //      .Expect (x => x.IsAspectAttribute (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == type)))
    //      .Return (true);
    //  _customAttributeDataHelperMock
    //      .Expect (x => x.IsAspectAttribute (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType != type)))
    //      .Return (false).Repeat.Any();
    //  _attributeDeclarationProviderMock
    //      .Expect (x => x.GetAdviceBuilders (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == type)))
    //      .Return (new[] { fakeAdviceBuilder });

    //  var result = _provider.GetDeclarations().ToArray();

    //  _aspectTypesProviderMock.VerifyAllExpectations();
    //  _attributeDeclarationProviderMock.VerifyAllExpectations();
    //  Assert.That (result, Is.EquivalentTo (new[] { fakeAdviceBuilder }));
    //}

    //[Test]
    //public void GetDeclarations_DistinctAssemblies ()
    //{
    //  var type = typeof (AssemblyWithAttribute.DomainAspectAttribute);

    //  _aspectTypesProviderMock.Expect (x => x.GetAspectAttributeTypes ()).Return (new[] { type, type });
    //  _customAttributeDataHelperMock
    //      .Expect (x => x.IsAspectAttribute (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == type)))
    //      .Return (true);
    //  _customAttributeDataHelperMock
    //      .Expect (x => x.IsAspectAttribute (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType != type)))
    //      .Return (false).Repeat.Any ();
    //  _attributeDeclarationProviderMock
    //      .Expect (x => x.GetAdviceBuilders (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == type)))
    //      .Return (new IAdviceBuilder[0]);

    //  Assert.That (() => _provider.GetDeclarations().ToArray(), Throws.Nothing);
    //}

    [Test]
    public void Resolution ()
    {
      var instances = SafeServiceLocator.Current.GetAllInstances<IAssemblyLevelDeclarationProvider> ();

      Assert.That (instances, Has.Some.TypeOf<AssemblyAttributeDeclarationProvider> ());
    }

    [AttributeUsage (AttributeTargets.Assembly)]
    public class DomainAspectAttribute : AspectAttributeBase { }
  }
}