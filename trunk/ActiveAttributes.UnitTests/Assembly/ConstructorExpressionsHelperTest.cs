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
using System.Reflection;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.FieldWrapper;
using ActiveAttributes.Core.Infrastructure;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class ConstructorExpressionsHelperTest
  {
    private MutableType _declaringType;
    private ThisExpression _thisExpression;
    private BodyContextBase _bodyContext;
    private ConstructorExpressionsHelper _constructorExpressionsHelper;

    private IAspectInitExpressionHelper _aspectInitExpressionHelperMock;

    [SetUp]
    public void SetUp ()
    {
      _declaringType = ObjectMother.GetMutableType();
      _thisExpression = new ThisExpression (_declaringType);
      _bodyContext = ObjectMother.GetBodyContextBase (_declaringType);
      _aspectInitExpressionHelperMock = MockRepository.GenerateStrictMock<IAspectInitExpressionHelper>();
      _constructorExpressionsHelper = new ConstructorExpressionsHelper (_aspectInitExpressionHelperMock, _bodyContext);
    }

    [Test]
    public void CreateMemberInfoAssignExpression_MethodInfo ()
    {
      var field = ObjectMother.GetFieldWrapper (typeof (MethodInfo), declaringType: _declaringType);
      var method = ObjectMother.GetMethodInfo();

      var actual = _constructorExpressionsHelper.CreateMemberInfoAssignExpression (field, method);
      var expected =
          Expression.Assign (
              Expression.Field (_thisExpression, field.Field),
              Expression.Constant (method));

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void CreateMemberInfoAssignExpression_PropertyInfo ()
    {
      var field = ObjectMother.GetFieldWrapper (typeof (PropertyInfo), declaringType: _declaringType);
      var property = ObjectMother.GetPropertyInfo();

      var actual = _constructorExpressionsHelper.CreateMemberInfoAssignExpression (field, property);
      var expected =
          Expression.Assign (
              Expression.Field (_thisExpression, field.Field),
              Expression.Call (
                  Expression.Constant (_declaringType),
                  typeof (Type).GetMethod ("GetProperty", new[] { typeof (string), typeof (BindingFlags) }),
                  Expression.Constant (property.Name),
                  Expression.Constant (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)));

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void CreateDelegateAssignExpression_FieldWrapper ()
    {
      var method = ObjectMother2.GetMethodInfo (returnType: typeof (string), parameterTypes: new[] { typeof (int), typeof (string) });
      var fieldMock = MockRepository.GenerateStrictMock<IFieldWrapper> ();
      var fakeExpression1 = ObjectMother2.GetMemberExpression (typeof (Delegate));

      fieldMock.Expect (x => x.GetAccessExpression (Arg<Expression>.Is.Anything)).Return (fakeExpression1);

      var result = _constructorExpressionsHelper.CreateDelegateAssignExpression (fieldMock, method);

      fieldMock.VerifyAllExpectations();
      Assert.That (result.Left, Is.SameAs (fakeExpression1));
      Assert.That (result.Right, Is.TypeOf<NewDelegateExpression> ());
      var newDelegateExpression = (NewDelegateExpression) result.Right;
      Assert.That (newDelegateExpression.Type, Is.EqualTo (typeof (Func<int, string, string>)));
      Assert.That (newDelegateExpression.Target.Type, Is.EqualTo (_declaringType));
      Assert.That (newDelegateExpression.Method, Is.SameAs (method));
    }

    [Test]
    public void CreateAspectAssignExpression ()
    {
      var constructionInfo = ObjectMother2.GetAspectConstructionInfo();
      var fieldMock = MockRepository.GenerateStrictMock<IFieldWrapper>();
      var fakeExpression1 = ObjectMother2.GetMemberExpression (typeof (IAspect));
      var fakeExpression2 = ObjectMother2.GetMemberInitExpression (typeof (DomainAspect));

      fieldMock.Expect (x => x.GetAccessExpression (Arg<Expression>.Is.Anything)).Return (fakeExpression1);
      _aspectInitExpressionHelperMock.Expect (x => x.CreateInitExpression (constructionInfo)).Return (fakeExpression2);

      var result = _constructorExpressionsHelper.CreateAspectAssignExpression (fieldMock, constructionInfo);

      fieldMock.VerifyAllExpectations();
      _aspectInitExpressionHelperMock.VerifyAllExpectations();
      Assert.That (result.Left, Is.SameAs (fakeExpression1));
      Assert.That (result.Right, Is.SameAs (fakeExpression2));
    }

    class DomainAspect : IAspect {}
  }
}