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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Extensions;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Extensions
{
  [TestFixture]
  public class ICustomAttributeDataExtensionsTest
  {
    public class ConvertTo
    {
      private string _elementField;
      private object[] _arrayField;

      private ICustomAttributeNamedArgument _customAttributeNamedArgumentMock;

      [Test]
      public void Element ()
      {
        var customAttributeNamedArgumentMock = MockRepository.GenerateStrictMock<ICustomAttributeNamedArgument> ();
        var input = "test";
        var output = "output";
        customAttributeNamedArgumentMock
            .Expect (x => x.MemberType)
            .Return (typeof(string));
        customAttributeNamedArgumentMock
            .Expect (x => x.Value)
            .Return (input);

        Func<Type, object, object> elementConstructor =
            (type, value) =>
            {
              Assert.That (type, Is.EqualTo (typeof (string)));
              Assert.That (value, Is.SameAs (input));
              return output;
            };
        Func<Type, IEnumerable<object>, object> arrayConstructor =
            (type, enumerable) =>
            {
              throw new Exception ("should not get here");
            };

        var result = ICustomAttributeDataExtensions.ConvertTo (customAttributeNamedArgumentMock, elementConstructor, arrayConstructor);

        Assert.That (result, Is.SameAs (output));
        customAttributeNamedArgumentMock.VerifyAllExpectations ();
      }

      [Test]
      public void Array ()
      {
        var customAttributeNamedArgumentMock = MockRepository.GenerateStrictMock<ICustomAttributeNamedArgument> ();
        var input = new object[] { "str", 7 };
        var output = "output";
        customAttributeNamedArgumentMock
            .Expect (x => x.MemberType)
            .Return (typeof(object[]));
        customAttributeNamedArgumentMock
            .Expect (x => x.Value)
            .Return (input);

        Func<Type, object, object> elementConstructor = (type, value) => value;
        Func<Type, IEnumerable<object>, object> arrayConstructor =
            (type, enumerable) =>
            {
              Assert.That (type, Is.EqualTo (typeof (object)));
              Assert.That (enumerable, Is.EquivalentTo (input));
              return output;
            };

        var result = ICustomAttributeDataExtensions.ConvertTo (customAttributeNamedArgumentMock, elementConstructor, arrayConstructor);

        Assert.That (result, Is.SameAs (output));
        customAttributeNamedArgumentMock.VerifyAllExpectations ();
      }
    }

    public class IsAspectAttribute
    {
      [Test]
      public void AspectAttribute_ReturnsTrue ()
      {
        var customAttributeDataMock = MockRepository.GenerateStrictMock<ICustomAttributeData> ();
        customAttributeDataMock
            .Expect (x => x.Constructor)
            .Return (typeof(DomainAttribute).GetConstructor(Type.EmptyTypes));

        var result = ICustomAttributeDataExtensions.IsAspectAttribute (customAttributeDataMock);

        Assert.That (result, Is.True);
      }

      [Test]
      public void CompilerGenerated_ReturnsFalse ()
      {
        var customAttributeDataMock = MockRepository.GenerateStrictMock<ICustomAttributeData> ();
        customAttributeDataMock
            .Expect (x => x.Constructor)
            .Return (typeof (CompilerGeneratedAttribute).GetConstructors ().Single ());

        var result = ICustomAttributeDataExtensions.IsAspectAttribute (customAttributeDataMock);

        Assert.That (result, Is.False);
      }
    }

    public class CreateAttribute
    {
      [Test]
      public void DefaultConstructor ()
      {
        var customAttributeDataMock = MockRepository.GenerateStrictMock<ICustomAttributeData>();
        customAttributeDataMock
            .Expect (x => x.Constructor)
            .Return (typeof (DomainAttribute).GetConstructor (Type.EmptyTypes));
        customAttributeDataMock
            .Expect (x => x.ConstructorArguments)
            .Return (new ReadOnlyCollection<object>(new object[0]));
        // TODO: how to test enumerable? ToCollection before argument checks?
        customAttributeDataMock
            .Expect (x => x.NamedArguments)
            .Return (new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (new ICustomAttributeNamedArgument[0]))
            .Repeat.Twice();

        var result = ICustomAttributeDataExtensions.CreateAttribute (customAttributeDataMock);

        Assert.That (result, Is.TypeOf<DomainAttribute>());
      }

      [Test]
      public void ConstructorWithArgument ()
      {
        var input = new[] { "ctor" };

        var customAttributeDataMock = MockRepository.GenerateStrictMock<ICustomAttributeData> ();
        customAttributeDataMock
            .Expect (x => x.Constructor)
            .Return (typeof (DomainAttribute).GetConstructor (new[] { typeof (string) }));
        customAttributeDataMock
            .Expect (x => x.ConstructorArguments)
            .Return (new ReadOnlyCollection<object> (input));
        // TODO: how to test enumerable? ToCollection before argument checks?
        customAttributeDataMock
            .Expect (x => x.NamedArguments)
            .Return (new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (new ICustomAttributeNamedArgument[0]))
            .Repeat.Twice ();

        var result = ICustomAttributeDataExtensions.CreateAttribute<DomainAttribute> (customAttributeDataMock);

        Assert.That (result, Is.Not.SameAs (input));
        Assert.That (result.ConstructorArgument, Is.EqualTo ("ctor"));
      }

      [Test]
      public void NamedArgument ()
      {
        var input = new[] { "named" };

        var customAttributeDataMock = MockRepository.GenerateStrictMock<ICustomAttributeData> ();
        var customAttributeNamedArgumentMock = MockRepository.GenerateStrictMock<ICustomAttributeNamedArgument>();
        customAttributeNamedArgumentMock
            .Expect (x => x.MemberInfo)
            .Return (typeof(DomainAttribute).GetFields().Single());
        customAttributeNamedArgumentMock
            .Expect (x => x.MemberType)
            .Return (typeof (string[]));
        customAttributeNamedArgumentMock
            .Expect (x => x.Value)
            .Return (input);
        customAttributeDataMock
            .Expect (x => x.Constructor)
            .Return (typeof (DomainAttribute).GetConstructor (Type.EmptyTypes));
        customAttributeDataMock
            .Expect (x => x.ConstructorArguments)
            .Return (new ReadOnlyCollection<object> (new object[0]));
        // TODO: how to test enumerable? ToCollection before argument checks?
        customAttributeDataMock
            .Expect (x => x.NamedArguments)
            .Return (new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (new[] { customAttributeNamedArgumentMock }))
            .Repeat.Twice ();

        var result = ICustomAttributeDataExtensions.CreateAttribute<DomainAttribute> (customAttributeDataMock);

        Assert.That (result.Field, Is.Not.SameAs (input));
        Assert.That (result.Field, Is.EqualTo (input));
      }
    }

    class DomainAttribute : AspectAttribute
    {
      public string[] Field;

      public DomainAttribute () {}

      public DomainAttribute (string constructorArgument)
      {
        ConstructorArgument = constructorArgument;
      }

      public string ConstructorArgument { get; set; }
    }
  }
}