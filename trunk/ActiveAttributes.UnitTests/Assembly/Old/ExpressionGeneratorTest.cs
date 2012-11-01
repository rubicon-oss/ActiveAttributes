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

namespace ActiveAttributes.UnitTests.Assembly.Old
{
  [TestFixture]
  public class ExpressionGeneratorTest
  {
    public class GetInitExpression
    {
      private IFieldWrapper _fieldWrapperMock;
      private IAspectDescriptor _aspectDescriptorMock;
      private ICustomAttributeNamedArgument _namedPropertyArgumentMock;
      private ICustomAttributeNamedArgument _namedFieldArgumentMock;

      [SetUp]
      public void SetUp ()
      {
        _fieldWrapperMock = MockRepository.GenerateStrictMock<IFieldWrapper>();
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

        var generator = new ExpressionGenerator (_fieldWrapperMock, 0, _aspectDescriptorMock);

        var expected = Expression.MemberInit (Expression.New (constructorInfo));
        var actual = generator.GetInitExpression();
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

        var generator = new ExpressionGenerator (_fieldWrapperMock, 0, _aspectDescriptorMock);

        var expected = Expression.MemberInit (Expression.New (constructorInfo, Expression.Constant ("ctor", typeof (string))));
        var actual = generator.GetInitExpression();
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
        var propertyInfo = NormalizingMemberInfoFromExpressionUtility.GetProperty ((AspectAttribute obj) => obj.PropertyElementArg);
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

        var generator = new ExpressionGenerator (_fieldWrapperMock, 0, _aspectDescriptorMock);

        var expected = Expression.MemberInit (
            Expression.New (constructorInfo),
            Expression.Bind (propertyInfo, Expression.Constant ("prop", typeof (string))),
            Expression.Bind (fieldInfo, Expression.Constant ("field", typeof (string)))
            );
        var actual = generator.GetInitExpression();

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

        var generator = new ExpressionGenerator (_fieldWrapperMock, 0, _aspectDescriptorMock);

        var expected = Expression.MemberInit (
            Expression.New (constructorInfo),
            Expression.Bind (
                fieldInfo,
                Expression.NewArrayInit (typeof (object), Expression.Constant ("str", typeof (object)), Expression.Constant (7, typeof (object))))
            );
        var actual = generator.GetInitExpression();

        ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
      }

      [Test]
      public void EnumArgument () {}

      //[Test]

      //[Aspect (new[] { "a" })]

      //public void ContainsConstructorArrayArg ()
      //{
      //  var method = MethodInfo.GetCurrentMethod ();
      //  var customData = GetFromMethod<AspectAttribute> (method);
      //  var descriptor = GetDescriptorMock (customData);
      //  var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);

      //  var expected = Expression.MemberInit (Expression.New (customData.Constructor, _arrayExpression));
      //  var actual = generator.CreateInitExpression ();

      //  ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
      //}

      //[Test]
      //[Aspect (PropertyElementArg = "a")]
      //public void ContainsPropertyElementArg ()
      //{
      //  var method = MethodInfo.GetCurrentMethod ();
      //  var customData = GetFromMethod<AspectAttribute> (method);
      //  var descriptor = GetDescriptorMock (customData);
      //  var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);
      //  var member = MemberInfoFromExpressionUtility.GetProperty (((AspectAttribute obj) => obj.PropertyElementArg));

      //  var expected = Expression.MemberInit (
      //      Expression.New (customData.Constructor),
      //      Expression.Bind (member, _elementExpression));
      //  var actual = generator.CreateInitExpression ();

      //  ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
      //}

      //[Test]
      //[Aspect (PropertyArrayArg = new[] { "a" })]
      //public void ContainsPropertyArrayArg ()
      //{
      //  var method = MethodInfo.GetCurrentMethod ();
      //  var customData = GetFromMethod<AspectAttribute> (method);
      //  var descriptor = GetDescriptorMock (customData);
      //  var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);
      //  var member = MemberInfoFromExpressionUtility.GetProperty (((AspectAttribute obj) => obj.PropertyArrayArg));

      //  var expected = Expression.MemberInit (
      //      Expression.New (customData.Constructor),
      //      Expression.Bind (member, _arrayExpression));
      //  var actual = generator.CreateInitExpression ();

      //  ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
      //}

      //[Test]
      //[Aspect (FieldElementArg = "a")]
      //public void ContainsFieldElementArg ()
      //{
      //  var method = MethodInfo.GetCurrentMethod ();
      //  var customData = GetFromMethod<AspectAttribute> (method);
      //  var descriptor = GetDescriptorMock (customData);
      //  var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);
      //  var member = MemberInfoFromExpressionUtility.GetField (((AspectAttribute obj) => obj.FieldElementArg));

      //  var expected = Expression.MemberInit (
      //      Expression.New (customData.Constructor),
      //      Expression.Bind (member, _elementExpression));
      //  var actual = generator.CreateInitExpression ();

      //  ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
      //}

      //[Test]
      //[Aspect (FieldArrayArg = new[] { "a" })]
      //public void ContainsFieldArrayArg ()
      //{
      //  var method = MethodInfo.GetCurrentMethod ();
      //  var customData = GetFromMethod<AspectAttribute> (method);
      //  var descriptor = GetDescriptorMock (customData);
      //  var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);
      //  var member = MemberInfoFromExpressionUtility.GetField (((AspectAttribute obj) => obj.FieldArrayArg));

      //  var expected = Expression.MemberInit (
      //      Expression.New (customData.Constructor),
      //      Expression.Bind (member, _arrayExpression));
      //  var actual = generator.CreateInitExpression ();

      //  ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
      //}
    }

    //public class GetStorageExpression
    //{
    //  private static AspectAttribute[] StaticDummy;
    //  private AspectAttribute[] InstanceDummy;

    //  [Test]
    //  [Aspect]
    //  public void StaticFieldExpression ()
    //  {
    //    var method = MethodInfo.GetCurrentMethod ();
    //    var customData = GetFromMethod<AspectAttribute> (method);
    //    var descriptor = GetDescriptorMock (customData);
    //    var fieldInfo = MemberInfoFromExpressionUtility.GetField (() => StaticDummy);
    //    var accessor = GetAccessor (fieldInfo, thisExpression: false);
    //    var generator = new AspectGenerator (accessor, 0, descriptor);

    //    var expected = Expression.ArrayAccess (
    //        Expression.Field (null, fieldInfo),
    //        Expression.Constant (0));
    //    var actual = generator.GetStorageExpression (null);

    //    ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    //  }

    //  [Test]
    //  [Aspect]
    //  public void InstanceFieldExpression ()
    //  {
    //    var method = MethodInfo.GetCurrentMethod();
    //    var customData = GetFromMethod<AspectAttribute> (method);
    //    var descriptor = GetDescriptorMock (customData);
    //    var fieldInfo = MemberInfoFromExpressionUtility.GetField (() => InstanceDummy);
    //    var accessor = GetAccessor (fieldInfo, thisExpression: true);
    //    var generator = new AspectGenerator (accessor, 0, descriptor);

    //    var expected = Expression.ArrayAccess (
    //        Expression.Field (new ThisExpression(GetType_()), fieldInfo),
    //        Expression.Constant (0));
    //    var actual = generator.GetStorageExpression (new ThisExpression(GetType_()));

    //    ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    //  }

    //  private IArrayAccessor GetAccessor (FieldInfo fieldInfo, bool thisExpression)
    //  {
    //    var mock = MockRepository.GenerateMock<IArrayAccessor> ();
    //    var expression = Expression.Field (thisExpression ? new ThisExpression(GetType_()) : null, fieldInfo);
    //    mock.Expect (x => x.GetAccessExpression (null)).IgnoreArguments().Return (expression);
    //    return mock;
    //  }
    //}

    private class AspectAttribute : Attribute
    {
      public string FieldElementArg;
      public object[] FieldArrayArg;

      public AspectAttribute () {}

      public AspectAttribute (string elementArgument)
      {
        ConstructorElementArg = elementArgument;
      }

      public AspectAttribute (string[] arrayArgument)
      {
        Dev.Null = arrayArgument;
      }

      public string ConstructorElementArg { get; set; }
      public string[] ConstructorArrayArg { get; set; }

      public string PropertyElementArg { get; set; }

      public BindingFlags EnumProperty { get; set; }
    }
  }
}