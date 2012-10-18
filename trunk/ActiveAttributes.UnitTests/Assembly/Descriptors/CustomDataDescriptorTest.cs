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
using System.Reflection;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Assembly.Descriptors;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly.Descriptors
{
  [TestFixture]
  public class CustomDataDescriptorTest
  {
    private static ICustomAttributeData GetCustomAttributeDataMockFor<T> (MemberInfo method)
    {
      var customAttributeData = CustomAttributeData.GetCustomAttributes (method).Single (x => x.Constructor.DeclaringType == typeof (T));
      var namedArguments = customAttributeData.NamedArguments.Select (
          x =>
          {
            var namedArgumentStub = MockRepository.GenerateStub<ICustomAttributeNamedArgument>();
            namedArgumentStub
                .Stub (z => z.MemberInfo)
                .Return (x.MemberInfo);
            namedArgumentStub
                .Stub (z => z.MemberType)
                .Return (x.TypedValue.ArgumentType);
            namedArgumentStub
                .Stub (z => z.Value)
                .Return (x.TypedValue.Value);
            return namedArgumentStub;
          });
      var namedArguments2 = new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (namedArguments.ToArray());

      var customAttributeDataMock = MockRepository.GenerateStrictMock<ICustomAttributeData>();
      customAttributeDataMock
          .Expect (x => x.Constructor)
          .Return (customAttributeData.Constructor)
          .Repeat.AtLeastOnce();
      customAttributeDataMock
          .Expect (x => x.ConstructorArguments)
          .Return (customAttributeData.ConstructorArguments.Select (x => x.Value).ToList().AsReadOnly())
          .Repeat.AtLeastOnce();
      customAttributeDataMock
          .Expect (x => x.NamedArguments)
          .Return (namedArguments2)
          .Repeat.AtLeastOnce();

      return customAttributeDataMock;
    }

    public class Initialize
    {
      [Test]
      [Aspect]
      public void Normal ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var customAttributeDataMock = GetCustomAttributeDataMockFor<AspectAttribute> (method);

        var descriptor = new CustomAttributeDataDescriptor (customAttributeDataMock);

        Assert.That (descriptor.ConstructorInfo, Is.SameAs (customAttributeDataMock.Constructor));
        Assert.That (descriptor.ConstructorArguments, Is.SameAs (customAttributeDataMock.ConstructorArguments));
        Assert.That (descriptor.NamedArguments, Is.SameAs (customAttributeDataMock.NamedArguments));
        Assert.That (descriptor.Scope, Is.EqualTo (Scope.Static));
        Assert.That (descriptor.Priority, Is.EqualTo (0));
        customAttributeDataMock.VerifyAllExpectations();
      }

      [Test]
      [ExpectedException (typeof (ArgumentException), ExpectedMessage = "CustomAttributeData must be from an AspectAttribute")]
      [NonAspect]
      public void ThrowsExceptionForNonAspectAttributes ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var customAttributeDataMock = GetCustomAttributeDataMockFor<NonAspectAttribute> (method);

        new CustomAttributeDataDescriptor (customAttributeDataMock);
      }

      [Test]
      [Aspect (Scope = Scope.Instance, Priority = 5)]
      public void SetsScopeAndPriority ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var customAttributeDataMock = GetCustomAttributeDataMockFor<AspectAttribute> (method);

        var descriptor = new CustomAttributeDataDescriptor (customAttributeDataMock);

        Assert.That (descriptor.Scope, Is.EqualTo (Scope.Instance));
        Assert.That (descriptor.Priority, Is.EqualTo (5));
      }
    }

    public class ToString_
    {
      [Test]
      [Aspect]
      public void ContainsTypeWithScopeAndPriority ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var customAttributeDataMock = GetCustomAttributeDataMockFor<AspectAttribute> (method);

        var descriptor = new CustomAttributeDataDescriptor (customAttributeDataMock);

        var result = descriptor.ToString ();
        Assert.That (result, Is.EqualTo ("AspectAttribute(Scope = Static, Priority = 0)"));
      }

      [Test]
      [Aspect ("muh")]
      public void ContainsConstructorArguments ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var customAttributeDataMock = GetCustomAttributeDataMockFor<AspectAttribute> (method);

        var descriptor = new CustomAttributeDataDescriptor (customAttributeDataMock);

        var result = descriptor.ToString ();
        Assert.That (result, Is.StringContaining ("Attribute({muh}"));
      }

      [Test]
      [Aspect (PropertyArgument = "muh", MemberNameFilter = "_")]
      public void ContainsNamedArguments ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var customAttributeDataMock = GetCustomAttributeDataMockFor<AspectAttribute> (method);

        var descriptor = new CustomAttributeDataDescriptor (customAttributeDataMock);

        var result = descriptor.ToString ();
        Assert.That (result, Is.StringContaining ("PropertyArgument = {muh}, MemberNameFilter = {_}"));
      }

      [Test]
      [Aspect (Scope = Scope.Instance, Priority = 5)]
      public void ContainsScopeAndPriorityOnlyOnce ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var customAttributeDataMock = GetCustomAttributeDataMockFor<AspectAttribute> (method);

        var descriptor = new CustomAttributeDataDescriptor (customAttributeDataMock);

        var result = descriptor.ToString ();
        Assert.That (result, Is.EqualTo ("AspectAttribute(Scope = Instance, Priority = 5)"));
      }
    }

    public class Matches_
    {
      [Test]
      [Aspect]
      public void DelegatesToAttribute ()
      {
        var method = MethodInfo.GetCurrentMethod();

        var customAttributeDataMock = GetCustomAttributeDataMockFor<AspectAttribute> (method);

        var descriptor = new CustomAttributeDataDescriptor (customAttributeDataMock);

        var matchingMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((object obj) => obj.ToString ());
        var notMatchingMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod ((object obj) => obj.GetType ());
        Assert.That (descriptor.Matches (matchingMethod), Is.True);
        Assert.That (descriptor.Matches (notMatchingMethod), Is.False);
      }
    }

    class AspectAttribute : Core.Aspects.AspectAttribute
    {
      public static MethodInfo MatchesMethod { get; private set; }

      public AspectAttribute () {}

      public AspectAttribute (string constructorArgument) {}

      public string PropertyArgument { get; set; }

      public override bool Matches (MethodInfo methodInfo)
      {
        return methodInfo.Name == "ToString";
      }
    }

    class NonAspectAttribute : Attribute { }
  }
}