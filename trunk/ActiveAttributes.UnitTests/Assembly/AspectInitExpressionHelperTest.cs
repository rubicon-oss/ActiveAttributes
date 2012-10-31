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
using System.Collections.ObjectModel;
using System.Reflection;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectInitExpressionHelperTest
  {
    private AspectInitExpressionHelper _aspectInitExpressionHelper;

    private IAspectDescriptor _aspectDescriptorMock;
    private ICustomAttributeNamedArgument _namedPropertyArgumentMock;
    private ICustomAttributeNamedArgument _namedFieldArgumentMock;

    [SetUp]
    public void SetUp ()
    {
      _aspectInitExpressionHelper = new AspectInitExpressionHelper();

      _aspectDescriptorMock = MockRepository.GenerateStrictMock<IAspectDescriptor>();
      _namedPropertyArgumentMock = MockRepository.GenerateStrictMock<ICustomAttributeNamedArgument>();
      _namedFieldArgumentMock = MockRepository.GenerateStrictMock<ICustomAttributeNamedArgument>();
    }

    [Test]
    public void Constructor ()
    {
      var constructorInfo = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new AspectAttribute());
      var constructorArguments = new ReadOnlyCollection<object> (new object[0]);
      var namedArgumentsCollection = new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (new ICustomAttributeNamedArgument[0]);

      _aspectDescriptorMock
          .Expect (x => x.ConstructorInfo)
          .Return (constructorInfo);
      _aspectDescriptorMock
          .Expect (x => x.ConstructorArguments)
          .Return (constructorArguments);
      _aspectDescriptorMock
          .Expect (x => x.NamedArguments)
          .Return (namedArgumentsCollection);

      var actual = _aspectInitExpressionHelper.CreateInitExpression (_aspectDescriptorMock);
      var expected = Expression.MemberInit (Expression.New (constructorInfo));

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void ConstructorWithArgument ()
    {
      var constructorInfo = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new AspectAttribute (""));
      var constructorArguments = new ReadOnlyCollection<object> (new[] { "ctor" });
      var namedArgumentsCollection = new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (new ICustomAttributeNamedArgument[0]);

      _aspectDescriptorMock
          .Expect (x => x.ConstructorInfo)
          .Return (constructorInfo);
      _aspectDescriptorMock
          .Expect (x => x.ConstructorArguments)
          .Return (constructorArguments);
      _aspectDescriptorMock
          .Expect (x => x.NamedArguments)
          .Return (namedArgumentsCollection);

      var actual = _aspectInitExpressionHelper.CreateInitExpression (_aspectDescriptorMock);
      var expected = Expression.MemberInit (Expression.New (constructorInfo, Expression.Constant ("ctor", typeof (string))));

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void NamedElementArgument ()
    {
      var constructorInfo = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new AspectAttribute());
      var constructorArguments = new ReadOnlyCollection<object> (new object[0]);
      var namedArgumentsCollection =
          new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (
              new[] { _namedPropertyArgumentMock, _namedFieldArgumentMock });
      var propertyInfo =
          NormalizingMemberInfoFromExpressionUtility.GetProperty ((AspectAttribute obj) => obj.PropertyElementArg);
      var fieldInfo = NormalizingMemberInfoFromExpressionUtility.GetField ((AspectAttribute obj) => obj.FieldElementArg);

      _namedPropertyArgumentMock
          .Expect (x => x.MemberInfo)
          .Return (propertyInfo);
      _namedPropertyArgumentMock
          .Expect (x => x.MemberType)
          .Return (propertyInfo.PropertyType);
      _namedPropertyArgumentMock
          .Expect (x => x.Value)
          .Return ("prop");
      _namedFieldArgumentMock
          .Expect (x => x.MemberInfo)
          .Return (fieldInfo);
      _namedFieldArgumentMock
          .Expect (x => x.MemberType)
          .Return (fieldInfo.FieldType);
      _namedFieldArgumentMock
          .Expect (x => x.Value)
          .Return ("field");
      _aspectDescriptorMock
          .Expect (x => x.ConstructorInfo)
          .Return (constructorInfo);
      _aspectDescriptorMock
          .Expect (x => x.ConstructorArguments)
          .Return (constructorArguments);
      _aspectDescriptorMock
          .Expect (x => x.NamedArguments)
          .Return (namedArgumentsCollection);

      var actual = _aspectInitExpressionHelper.CreateInitExpression (_aspectDescriptorMock);
      var expected = Expression.MemberInit (
          Expression.New (constructorInfo),
          Expression.Bind (propertyInfo, Expression.Constant ("prop", typeof (string))),
          Expression.Bind (fieldInfo, Expression.Constant ("field", typeof (string)))
          );

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void NamedArrayArgument ()
    {
      var namedArgumentsCollection =
          new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (
              new[] { _namedFieldArgumentMock });
      var constructorInfo = typeof (AspectAttribute).GetConstructor (Type.EmptyTypes);
      var constructorArguments = new ReadOnlyCollection<object> (new object[0]);
      var fieldInfo = NormalizingMemberInfoFromExpressionUtility.GetField ((AspectAttribute obj) => obj.FieldArrayArg);

      _namedFieldArgumentMock
          .Expect (x => x.MemberInfo)
          .Return (fieldInfo);
      _namedFieldArgumentMock
          .Expect (x => x.MemberType)
          .Return (fieldInfo.FieldType);
      _namedFieldArgumentMock
          .Expect (x => x.Value)
          .Return (new object[] { "str", 7 });
      _aspectDescriptorMock
          .Expect (x => x.ConstructorInfo)
          .Return (constructorInfo);
      _aspectDescriptorMock
          .Expect (x => x.ConstructorArguments)
          .Return (constructorArguments);
      _aspectDescriptorMock
          .Expect (x => x.NamedArguments)
          .Return (namedArgumentsCollection);

      var actual = _aspectInitExpressionHelper.CreateInitExpression (_aspectDescriptorMock);
      var expected = Expression.MemberInit (
          Expression.New (constructorInfo),
          Expression.Bind (
              fieldInfo,
              Expression.NewArrayInit (typeof (object), Expression.Constant ("str", typeof (object)), Expression.Constant (7, typeof (object))))
          );

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    [Ignore ("TODO")]
    public void EnumArgument () {}

    class AspectAttribute : Core.Aspects.AspectAttribute
    {
      public readonly string FieldElementArg;
      public readonly object[] FieldArrayArg;

      public AspectAttribute ()
      {
        FieldElementArg = null;
        FieldArrayArg = null;
      }

      public AspectAttribute (string elementArgument)
      {
        Dev.Null = elementArgument;
        PropertyElementArg = null;
      }

      public AspectAttribute (string[] arrayArgument)
      {
        Dev.Null = arrayArgument;
      }

      public string PropertyElementArg { get; private set; }

      public BindingFlags EnumProperty { get; set; }
    }
  }
}