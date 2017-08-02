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
using System.Runtime.CompilerServices;
using ActiveAttributes.Declaration;
using ActiveAttributes.Declaration.Providers;
using ActiveAttributes.Utilities;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace ActiveAttributes.UnitTests.Declaration.Providers
{
  [TestFixture]
  public class MemberAttributeDeclarationProviderTest
  {
    private MemberAttributeDeclarationProvider _provider;

    private ICustomAttributeDataHelper _customAttributeDataHelperMock;
    private IAttributeDeclarationProvider _attributeDeclarationProviderMock;
    private IAdviceBuilder _fakeAdviceBuilder1;
    private IAdviceBuilder _fakeAdviceBuilder2;
    private IAdviceBuilder _fakeAdviceBuilder3;

    private Func<ICustomAttributeData, Type, ICustomAttributeData> _customAttributeDataTypeValidator;

    [SetUp]
    public void SetUp ()
    {
      _attributeDeclarationProviderMock = MockRepository.GenerateStrictMock<IAttributeDeclarationProvider>();
      _customAttributeDataHelperMock = MockRepository.GenerateStrictMock<ICustomAttributeDataHelper>();

      _provider = new MemberAttributeDeclarationProvider (_attributeDeclarationProviderMock, _customAttributeDataHelperMock);

      _fakeAdviceBuilder1 = ObjectMother.GetAdviceBuilder();
      _fakeAdviceBuilder2 = ObjectMother.GetAdviceBuilder();
      _fakeAdviceBuilder3 = ObjectMother.GetAdviceBuilder();
    }

    [Test]
    public void GetAdviceBuilders ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
      _customAttributeDataHelperMock
          .Expect (x => x.IsInheriting (Arg<ICustomAttributeData>.Is.Anything))
          .Return (true);
      _customAttributeDataHelperMock
          .Expect (x => x.AllowsMultiple (Arg<ICustomAttributeData>.Is.Anything))
          .Return (true);
      _customAttributeDataHelperMock
          .Expect (x => x.IsAspectAttribute (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (InheritingAttribute))))
          .Return (true);
      _customAttributeDataHelperMock
          .Expect (x => x.IsAspectAttribute (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (CompilerGeneratedAttribute))))
          .Return (false);
      _attributeDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (InheritingAttribute))))
          .Return (new[] { _fakeAdviceBuilder1, _fakeAdviceBuilder2 }.AsOneTime());

      var result = _provider.GetAdviceBuilders (method, new[] { method }).ToArray();

      _customAttributeDataHelperMock.VerifyAllExpectations();
      _attributeDeclarationProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { _fakeAdviceBuilder1, _fakeAdviceBuilder2 }));
    }

    [Test]
    public void GetAdviceBuilders_Inheriting ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.InheritMethod ());
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainTypeBase obj) => obj.InheritMethod ());

      _customAttributeDataHelperMock
          .Stub (x => x.AllowsMultiple (Arg<ICustomAttributeData>.Is.Anything))
          .Return (true).Repeat.Any();
      _customAttributeDataHelperMock
          .Stub (x => x.IsAspectAttribute (Arg<ICustomAttributeData>.Is.Anything))
          .Return (true).Repeat.Any();
      _customAttributeDataHelperMock
          .Expect (x => x.IsInheriting (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (InheritingAttribute))))
          .Return (true);
      _customAttributeDataHelperMock
          .Expect (x => x.IsInheriting (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (NotInheritingAttribute))))
          .Return (false);
      _attributeDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (InheritingAttribute))))
          .Return (new[] { _fakeAdviceBuilder1 });

      var result = _provider.GetAdviceBuilders (method, new[] { method, baseMethod }).ToArray();

      _customAttributeDataHelperMock.VerifyAllExpectations();
      _attributeDeclarationProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { _fakeAdviceBuilder1 }));
    }

    [Test]
    public void GetAdviceBuilders_AllowsMultiple ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.MultipleMethod ());
      var baseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainTypeBase obj) => obj.MultipleMethod ());

      _customAttributeDataHelperMock
          .Stub (x => x.IsInheriting (Arg<ICustomAttributeData>.Is.Anything))
          .Return (true).Repeat.Any ();
      _customAttributeDataHelperMock
          .Stub (x => x.IsAspectAttribute (Arg<ICustomAttributeData>.Is.Anything))
          .Return (true).Repeat.Any ();
      _customAttributeDataHelperMock
          .Expect (x => x.AllowsMultiple (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (AllowMultipleAttribute))))
          .Return (true).Repeat.Twice();
      _customAttributeDataHelperMock
          .Expect (x => x.AllowsMultiple (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (DisallowMultipleAttribute))))
          .Return (false).Repeat.Twice();
      _attributeDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (AllowMultipleAttribute))))
          .Return (new[] { _fakeAdviceBuilder1 });
      _attributeDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (AllowMultipleAttribute))))
          .Return (new[] { _fakeAdviceBuilder2 });
      _attributeDeclarationProviderMock
          .Expect (x => x.GetAdviceBuilders (Arg<ICustomAttributeData>.Matches (y => y.Constructor.DeclaringType == typeof (DisallowMultipleAttribute))))
          .Return (new[] { _fakeAdviceBuilder3 });

      var result = _provider.GetAdviceBuilders (method, new[] { method, baseMethod }).ToArray();

      _customAttributeDataHelperMock.VerifyAllExpectations ();
      _attributeDeclarationProviderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EquivalentTo (new[] { _fakeAdviceBuilder1, _fakeAdviceBuilder2, _fakeAdviceBuilder3 }));
    }

    class DomainTypeBase
    {
      [Inheriting]
      [CompilerGenerated]
      public void Method () {}

      [Inheriting]
      [NotInheriting]
      public virtual void InheritMethod () {}

      [AllowMultiple]
      [DisallowMultiple]
      public virtual void MultipleMethod () {}
    }

    class DomainType : DomainTypeBase
    {
      public override void InheritMethod () {}

      [AllowMultiple]
      [DisallowMultiple]
      public override void MultipleMethod () { }
    }

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    class InheritingAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    class NotInheritingAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    class AllowMultipleAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    class DisallowMultipleAttribute : Attribute { }
  }
}