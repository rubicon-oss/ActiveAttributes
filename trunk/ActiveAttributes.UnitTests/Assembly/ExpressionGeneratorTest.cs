//// Copyright (c) rubicon IT GmbH, www.rubicon.eu
////
//// See the NOTICE file distributed with this work for additional information
//// regarding copyright ownership.  rubicon licenses this file to you under 
//// the Apache License, Version 2.0 (the "License"); you may not use this 
//// file except in compliance with the License.  You may obtain a copy of the 
//// License at
////
////   http://www.apache.org/licenses/LICENSE-2.0
////
//// Unless required by applicable law or agreed to in writing, software 
//// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
//// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
//// License for the specific language governing permissions and limitations
//// under the License.

//using System;
//using System.Linq;
//using System.Reflection;
//using ActiveAttributes.Core.Aspects;
//using ActiveAttributes.Core.Assembly;
//using Microsoft.Scripting.Ast;
//using NUnit.Framework;
//using Remotion.TypePipe.Expressions;
//using Remotion.TypePipe.UnitTests.Expressions;
//using Remotion.Utilities;
//using Rhino.Mocks;
// TODO UNIT TEST
//namespace ActiveAttributes.UnitTests.Assembly
//{
//  [TestFixture]
//  public class ExpressionGeneratorTest
//  {
//    private static CustomAttributeData GetFromMethod<T> (MethodBase method)
//    {
//      var customData = CustomAttributeData.GetCustomAttributes (method).Single (x => x.Constructor.DeclaringType == typeof (T));
//      return customData;
//    }

//    private static IAspectDescriptor GetDescriptorMock (CustomAttributeData customData)
//    {
//      var descriptor = MockRepository.GenerateMock<IAspectDescriptor> ();
//      descriptor.Expect (x => x.ConstructorInfo).Return (customData.Constructor);
//      descriptor.Expect (x => x.ConstructorArguments).Return (customData.ConstructorArguments);
//      descriptor.Expect (x => x.NamedArguments).Return (customData.NamedArguments);
//      return descriptor;
//    }

//    public class GetInitExpression
//    {
//      private IArrayAccessor _arrayAccessor;
//      private Expression _elementExpression;
//      private Expression _arrayExpression;
      
//      [SetUp]
//      public void SetUp ()
//      {
//        _arrayAccessor = MockRepository.GenerateMock<IArrayAccessor> ();

//        _elementExpression = Expression.Constant ("a", typeof (string));
//        _arrayExpression = Expression.NewArrayInit (typeof (string), _elementExpression);
//      }

//      [Test]
//      [Aspect ("a")]
//      public void ContainsConstructorElementArg ()
//      {
//        var method = MethodInfo.GetCurrentMethod();
//        var customData = GetFromMethod<AspectAttribute> (method);
//        var descriptor = GetDescriptorMock (customData);
//        var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);

//        var expected = Expression.MemberInit (
//            Expression.New (
//                customData.Constructor,
//                _elementExpression));
//        var actual = generator.GetInitExpression ();

//        ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
//      }

//      [Test]
//      [Aspect (new[] { "a" })]
//      public void ContainsConstructorArrayArg ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var customData = GetFromMethod<AspectAttribute> (method);
//        var descriptor = GetDescriptorMock (customData);
//        var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);

//        var expected = Expression.MemberInit (Expression.New (customData.Constructor, _arrayExpression));
//        var actual = generator.GetInitExpression ();

//        ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
//      }

//      [Test]
//      [Aspect (PropertyElementArg = "a")]
//      public void ContainsPropertyElementArg ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var customData = GetFromMethod<AspectAttribute> (method);
//        var descriptor = GetDescriptorMock (customData);
//        var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);
//        var member = MemberInfoFromExpressionUtility.GetProperty (((AspectAttribute obj) => obj.PropertyElementArg));

//        var expected = Expression.MemberInit (
//            Expression.New (customData.Constructor),
//            Expression.Bind (member, _elementExpression));
//        var actual = generator.GetInitExpression ();

//        ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
//      }

//      [Test]
//      [Aspect (PropertyArrayArg = new[] { "a" })]
//      public void ContainsPropertyArrayArg ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var customData = GetFromMethod<AspectAttribute> (method);
//        var descriptor = GetDescriptorMock (customData);
//        var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);
//        var member = MemberInfoFromExpressionUtility.GetProperty (((AspectAttribute obj) => obj.PropertyArrayArg));

//        var expected = Expression.MemberInit (
//            Expression.New (customData.Constructor),
//            Expression.Bind (member, _arrayExpression));
//        var actual = generator.GetInitExpression ();

//        ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
//      }

//      [Test]
//      [Aspect (FieldElementArg = "a")]
//      public void ContainsFieldElementArg ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var customData = GetFromMethod<AspectAttribute> (method);
//        var descriptor = GetDescriptorMock (customData);
//        var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);
//        var member = MemberInfoFromExpressionUtility.GetField (((AspectAttribute obj) => obj.FieldElementArg));

//        var expected = Expression.MemberInit (
//            Expression.New (customData.Constructor),
//            Expression.Bind (member, _elementExpression));
//        var actual = generator.GetInitExpression ();

//        ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
//      }

//      [Test]
//      [Aspect (FieldArrayArg = new[] { "a" })]
//      public void ContainsFieldArrayArg ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var customData = GetFromMethod<AspectAttribute> (method);
//        var descriptor = GetDescriptorMock (customData);
//        var generator = new AspectGenerator (_arrayAccessor, 0, descriptor);
//        var member = MemberInfoFromExpressionUtility.GetField (((AspectAttribute obj) => obj.FieldArrayArg));

//        var expected = Expression.MemberInit (
//            Expression.New (customData.Constructor),
//            Expression.Bind (member, _arrayExpression));
//        var actual = generator.GetInitExpression ();

//        ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
//      }

//    }

//    public class GetStorageExpression
//    {
//      private static AspectAttribute[] StaticDummy;
//      private AspectAttribute[] InstanceDummy;
      
//      [Test]
//      [Aspect]
//      public void StaticFieldExpression ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var customData = GetFromMethod<AspectAttribute> (method);
//        var descriptor = GetDescriptorMock (customData);
//        var fieldInfo = MemberInfoFromExpressionUtility.GetField (() => StaticDummy);
//        var accessor = GetAccessor (fieldInfo, thisExpression: false);
//        var generator = new AspectGenerator (accessor, 0, descriptor);

//        var expected = Expression.ArrayAccess (
//            Expression.Field (null, fieldInfo),
//            Expression.Constant (0));
//        var actual = generator.GetStorageExpression (null);

//        ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
//      }

//      [Test]
//      [Aspect]
//      public void InstanceFieldExpression ()
//      {
//        var method = MethodInfo.GetCurrentMethod();
//        var customData = GetFromMethod<AspectAttribute> (method);
//        var descriptor = GetDescriptorMock (customData);
//        var fieldInfo = MemberInfoFromExpressionUtility.GetField (() => InstanceDummy);
//        var accessor = GetAccessor (fieldInfo, thisExpression: true);
//        var generator = new AspectGenerator (accessor, 0, descriptor);
        
//        var expected = Expression.ArrayAccess (
//            Expression.Field (new ThisExpression(GetType()), fieldInfo),
//            Expression.Constant (0));
//        var actual = generator.GetStorageExpression (new ThisExpression(GetType()));

//        ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
//      }

//      private IArrayAccessor GetAccessor (FieldInfo fieldInfo, bool thisExpression)
//      {
//        var mock = MockRepository.GenerateMock<IArrayAccessor> ();
//        var expression = Expression.Field (thisExpression ? new ThisExpression(GetType()) : null, fieldInfo);
//        mock.Expect (x => x.GetAccessExpression (null)).IgnoreArguments().Return (expression);
//        return mock;
//      }
//    }

//    private class AspectAttribute : Attribute
//    {
//      public string FieldElementArg;
//      public string[] FieldArrayArg;

//      public AspectAttribute () { }

//      public AspectAttribute (string constructorElementArg)
//      {
//        ConstructorElementArg = constructorElementArg;
//      }

//      public AspectAttribute (string[] constructorArrayArg)
//      {
//        ConstructorArrayArg = constructorArrayArg;
//      }

//      public string ConstructorElementArg { get; set; }
//      public string[] ConstructorArrayArg { get; set; }

//      public string PropertyElementArg { get; set; }
//      public string[] PropertyArrayArg { get; set; }
//    }
//  }
//}