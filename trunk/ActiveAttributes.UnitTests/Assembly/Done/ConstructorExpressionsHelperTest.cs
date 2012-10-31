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
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Done;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
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
      var field = ObjectMother.GetFieldWrapper (typeof (Delegate), declaringType: _declaringType);
      var method = ObjectMother.GetMethodInfo (returnType: typeof (string), parameterTypes: new[] { typeof (int), typeof (string) });

      var actual = _constructorExpressionsHelper.CreateDelegateAssignExpression (field, method);
      var expected =
          Expression.Assign (
              Expression.Field (_thisExpression, field.Field),
              Expression.NewDelegate (
                  typeof (Func<int, string, string>),
                  new ThisExpression(_declaringType),
                  method));

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void CreateAspectsAssignExpression ()
    {
      var field = ObjectMother.GetFieldWrapper (typeof (AspectAttribute[]), declaringType: _declaringType);
      var aspectDescriptor1 = ObjectMother.GetInstanceAspectDescriptor (typeof (DomainAspect1Attribute));
      var aspectDescriptor2 = ObjectMother.GetInstanceAspectDescriptor (typeof (DomainAspect2Attribute));

      var aspectDescriptors = new[] { aspectDescriptor1, aspectDescriptor2 };

      var mockRepository = _aspectInitExpressionHelperMock.GetMockRepository();
      mockRepository.BackToRecordAll();
      using (mockRepository.Ordered())
      {
        _aspectInitExpressionHelperMock
            .Expect (x => x.CreateInitExpression (aspectDescriptor1))
            .Return (Expression.MemberInit (Expression.New (typeof (DomainAspect1Attribute))));
        _aspectInitExpressionHelperMock
            .Expect (x => x.CreateInitExpression (aspectDescriptor2))
            .Return (Expression.MemberInit (Expression.New (typeof (DomainAspect2Attribute))));
      }
      mockRepository.ReplayAll();

      var actual = _constructorExpressionsHelper.CreateAspectAssignExpression (field, aspectDescriptors);
      var expected =
          Expression.Assign (
              Expression.Field (_thisExpression, field.Field),
              Expression.NewArrayInit (
                  typeof (AspectAttribute),
                  Expression.MemberInit (Expression.New (typeof (DomainAspect1Attribute))),
                  Expression.MemberInit (Expression.New (typeof (DomainAspect2Attribute)))));
      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    private class DomainAspect1Attribute : AspectAttribute {}
    private class DomainAspect2Attribute : AspectAttribute {}
  }
}