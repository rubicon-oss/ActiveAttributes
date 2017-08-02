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
using ActiveAttributes.Assembly;
using ActiveAttributes.Weaving.Construction;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class InitializationExpressionHelperTest
  {
    private InitializationExpressionHelper _initializationExpressionHelper;

    private IMethodCopyService _methodCopyServiceMock;
    private IAspectConstruction _constructionMock;

    [SetUp]
    public void SetUp ()
    {
      _constructionMock = MockRepository.GenerateStrictMock<IAspectConstruction> ();
      _methodCopyServiceMock = MockRepository.GenerateStrictMock<IMethodCopyService>();

      _initializationExpressionHelper = new InitializationExpressionHelper (_methodCopyServiceMock);
    }

    [Test]
    public void CreateAspectInitExpression_Constructor ()
    {
      var constructorInfo = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainAspect ());
      SetupConstructionMock (constructorInfo);

      var result = _initializationExpressionHelper.CreateAspectInitExpression (_constructionMock);

      var newExpression = result.NewExpression;
      Assert.That (newExpression.Constructor, Is.SameAs (constructorInfo));
    }

    [Test]
    public void CreateAspectInitExpression_ConstructorWithArgument ()
    {
      var constructorInfo = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainAspect (""));
      var constructorArguments = new object[] { "test" };
      SetupConstructionMock (constructorInfo, constructorArguments);

      var result = _initializationExpressionHelper.CreateAspectInitExpression (_constructionMock);

      var newExpression = result.NewExpression;
      Assert.That (newExpression.Arguments, Has.Count.EqualTo (1));
      Assert.That (newExpression.Arguments[0], Is.TypeOf<ConstantExpression> ());
      var constantExpression = (ConstantExpression) newExpression.Arguments[0];
      Assert.That (constantExpression.Value, Is.EqualTo ("test"));
    }

    [Test]
    public void CreateAspectInitExpression_NamedElementArgument ()
    {
      var namedFieldArgumentMock = MockRepository.GenerateStrictMock<ICustomAttributeNamedArgument> ();
      var namedPropertyArgumentMock = MockRepository.GenerateStrictMock<ICustomAttributeNamedArgument> ();

      var field = NormalizingMemberInfoFromExpressionUtility.GetField ((DomainAspect obj) => obj.FieldElementArg);
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((DomainAspect obj) => obj.PropertyElementArg);
      namedFieldArgumentMock.Expect (x => x.MemberInfo).Return (field);
      namedFieldArgumentMock.Expect (x => x.MemberType).Return (field.FieldType);
      namedFieldArgumentMock.Expect (x => x.Value).Return ("field");
      namedPropertyArgumentMock.Expect (x => x.MemberInfo).Return (property);
      namedPropertyArgumentMock.Expect (x => x.MemberType).Return (property.PropertyType);
      namedPropertyArgumentMock.Expect (x => x.Value).Return ("prop");

      SetupConstructionMock (namedArguments: new[] { namedFieldArgumentMock, namedPropertyArgumentMock });

      var result = _initializationExpressionHelper.CreateAspectInitExpression (_constructionMock);

      Assert.That (result.Bindings, Has.Count.EqualTo (2));
      var assignment1 = (MemberAssignment) result.Bindings[0];
      Assert.That (assignment1.Member, Is.EqualTo (field));
      Assert.That (assignment1.Expression, Is.TypeOf<ConstantExpression> ().With.Property ("Value").EqualTo ("field"));
      var assignment2 = (MemberAssignment) result.Bindings[1];
      Assert.That (assignment2.Member, Is.EqualTo (property));
      Assert.That (assignment2.Expression, Is.TypeOf<ConstantExpression> ().With.Property ("Value").EqualTo ("prop"));
    }

    [Test]
    public void CreateAspectInitExpression_NamedArrayArgument ()
    {
      var namedArgumentMock = MockRepository.GenerateStrictMock<ICustomAttributeNamedArgument> ();

      var arrayField = NormalizingMemberInfoFromExpressionUtility.GetField ((DomainAspect obj) => obj.FieldArrayArg);
      namedArgumentMock.Expect (x => x.MemberInfo).Return (arrayField);
      namedArgumentMock.Expect (x => x.MemberType).Return (arrayField.FieldType);
      namedArgumentMock.Expect (x => x.Value).Return (new object[] { "str", 7 });

      SetupConstructionMock (namedArguments: new[] { namedArgumentMock });

      var result = _initializationExpressionHelper.CreateAspectInitExpression (_constructionMock);

      var assignment = (MemberAssignment) result.Bindings[0];
      Assert.That (assignment.Expression, Is.InstanceOf<NewArrayExpression> ().With.Property ("Type").EqualTo (typeof (object[])));
      var newArrayExpression = (NewArrayExpression) assignment.Expression;
      Assert.That (newArrayExpression.Expressions, Has.Count.EqualTo (2));
      Assert.That (newArrayExpression.Expressions[0], Is.InstanceOf<ConstantExpression> ().With.Property ("Type").EqualTo (typeof (object)));
      var expression1 = (ConstantExpression) newArrayExpression.Expressions[0];
      Assert.That (expression1, Has.Property ("Value").EqualTo ("str"));
      Assert.That (expression1, Is.InstanceOf<ConstantExpression> ().With.Property ("Type").EqualTo (typeof (object)));
      var expression2 = (ConstantExpression) newArrayExpression.Expressions[1];
      Assert.That (expression2, Has.Property ("Value").EqualTo (7));
      Assert.That (expression2, Is.InstanceOf<ConstantExpression> ().With.Property ("Type").EqualTo (typeof (object)));
    }

    [Test]
    public void CreateMemberInfoInitExpression_MethodInfo ()
    {
      var method = ObjectMother.GetMethodInfo ();

      var result = _initializationExpressionHelper.CreateMemberInfoInitExpression (method);

      Assert.That (result, Is.InstanceOf<ConstantExpression> ());
      var constantExpression = (ConstantExpression) result;
      Assert.That (constantExpression.Value, Is.SameAs (method));
      Assert.That (constantExpression.Type, Is.EqualTo (typeof (MethodInfo)));
    }

    [Test]
    public void CreateMemberInfoInitExpression_PropertyInfo ()
    {
      var property = ObjectMother.GetPropertyInfo ();

      var result = _initializationExpressionHelper.CreateMemberInfoInitExpression (property);

      Assert.That (result, Is.InstanceOf<MethodCallExpression> ());
      var callExpression = (MethodCallExpression) result;
      Assert.That (callExpression.Object, Is.TypeOf<ConstantExpression>().With.Property("Value").EqualTo (property.DeclaringType));
      Assert.That (callExpression.Method, Is.EqualTo (typeof (Type).GetMethod ("GetProperty", new[] { typeof (string), typeof (BindingFlags) })));
      Assert.That (callExpression.Arguments, Has.Count.EqualTo (2));
      var arguments = callExpression.Arguments;
      Assert.That (arguments[0], Is.TypeOf<ConstantExpression> ().With.Property ("Value").EqualTo (property.Name));
      var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      Assert.That (arguments[1], Is.TypeOf<ConstantExpression> ().With.Property ("Value").EqualTo (bindingFlags));
    }

    [Test]
    public void CreateDelegateInitExpression ()
    {
      var baseMethod = ObjectMother.GetMethodInfo();
      var method = ObjectMother.GetMutableMethodInfo (baseMethod);
      var fakeMethod = ObjectMother.GetMutableMethodInfo (baseMethod);
      var thisExpression = ObjectMother.GetThisExpression (method.DeclaringType);

      _methodCopyServiceMock.Expect (x => x.GetCopy (method)).Return (fakeMethod);

      var result = _initializationExpressionHelper.CreateDelegateInitExpression (method, thisExpression);

      var types = baseMethod.GetParameters().Select (x => x.ParameterType).Concat (new[] { baseMethod.ReturnType }).ToArray();
      var delegateType = Expression.GetDelegateType (types);
      Assert.That (result.Type, Is.EqualTo (delegateType));
      Assert.That (result.Method, Is.EqualTo (fakeMethod));
      Assert.That (result.Target, Is.SameAs (thisExpression));
    }

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IInitializationExpressionHelper> ();

      Assert.That (instance, Is.TypeOf<InitializationExpressionHelper> ());
    }

    private void SetupConstructionMock (
        ConstructorInfo constructorInfo = null, object[] constructorArguments = null, ICustomAttributeNamedArgument[] namedArguments = null)
    {
      constructorInfo = constructorInfo ?? NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainAspect ());
      constructorArguments = constructorArguments ?? new object[0];
      namedArguments = namedArguments ?? new ICustomAttributeNamedArgument[0];

      _constructionMock.Expect (x => x.ConstructorInfo).Return (constructorInfo);
      _constructionMock.Expect (x => x.ConstructorArguments).Return (constructorArguments.ToList ().AsReadOnly ());
      _constructionMock.Expect (x => x.NamedArguments).Return (
          new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (namedArguments));
    }

    private class DomainAspect : IAspect
    {
      public readonly string FieldElementArg;
      public readonly object[] FieldArrayArg;

      public DomainAspect ()
      {
        FieldElementArg = null;
        FieldArrayArg = null;
      }

      public DomainAspect (string elementArgument)
      {
        Dev.Null = elementArgument;
        PropertyElementArg = null;
      }

      public DomainAspect (string[] arrayArgument)
      {
        Dev.Null = arrayArgument;
      }

      public string PropertyElementArg { get; private set; }

      public BindingFlags EnumProperty { get; set; }
    }
  }
}