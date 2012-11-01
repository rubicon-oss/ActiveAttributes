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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
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
  public class MethodExpressionHelperTest
  {
    private MutableType _declaringType;
    private ThisExpression _thisExpression;
    private BodyContextBase _bodyContext;
    private MethodExpressionHelper _expressionHelper;
    private ParameterExpression _parameterExpression1;
    private ParameterExpression _parameterExpression2;
    private IInvocationExpressionHelper _invocationExpressionHelperMock;
    private IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> _aspectDescriptorDictionaryMock;
      
    [SetUp]
    public void SetUp ()
    {
      _declaringType = ObjectMother.GetMutableType ();
      _thisExpression = new ThisExpression (_declaringType);
      _parameterExpression1 = ObjectMother.GetParameterExpression (typeof (string), "param1");
      _parameterExpression2 = ObjectMother.GetParameterExpression (typeof (int), "param2");
      _bodyContext = ObjectMother.GetBodyContextBase (_declaringType, new[] { _parameterExpression1, _parameterExpression2 });
      _invocationExpressionHelperMock = MockRepository.GenerateMock<IInvocationExpressionHelper>();
      _aspectDescriptorDictionaryMock = MockRepository.GenerateMock<IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>>>();
      var method = ObjectMother.GetMutableMethodInfo();

      _expressionHelper = new MethodExpressionHelper (method, _bodyContext, _aspectDescriptorDictionaryMock, _invocationExpressionHelperMock);
    }

    [Test]
    public void GetInvocationContextExpressions ()
    {
      var invocationContextType = typeof (FuncInvocationContext<object, string, int, int>);
      var memberInfoFieldMock = MockRepository.GenerateStrictMock<IFieldWrapper>();
      var fakeMemberInfoExpression = ObjectMother.GetMemberExpression (typeof (MethodInfo));

      // TODO cannot make expectations on ThisExpressions
      memberInfoFieldMock
        // .Expect (x => x.GetAccessExpression (_thisExpression))
          .Expect (x => x.GetAccessExpression (Arg<Expression>.Matches(y => y.Type == _declaringType)))
          .Return (fakeMemberInfoExpression);

      var result = _expressionHelper.CreateInvocationContextExpressions (invocationContextType, memberInfoFieldMock);
      var actualParameterExpression = result.Item1;
      var actualAssignExpression = result.Item2;

      var expectedParameterExpression = Expression.Variable (typeof (FuncInvocationContext<object, string, int, int>), "ctx");
      var expectedAssignExpression =
          Expression.Assign (
              actualParameterExpression,
              Expression.New (
                  typeof (FuncInvocationContext<object, string, int, int>).GetConstructors().Single(),
                  new Expression[] { fakeMemberInfoExpression, _thisExpression , _parameterExpression1, _parameterExpression2 }));

      ExpressionTreeComparer.CheckAreEqualTrees (expectedParameterExpression, actualParameterExpression);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedAssignExpression, actualAssignExpression);
    }

    [Test]
    public void CreateInvocationExpressions ()
    {
      var innerInvocationType = typeof (FuncInvocation<object, string, int, int>);
      var invocationContext = ObjectMother.GetParameterExpression();
      var delegateField = ObjectMother.GetFieldWrapper();
      var aspectDescriptorDictionaryMock = MockRepository.GenerateStrictMock<IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>>> ();
      var aspectDescriptor1 = ObjectMother.GetAspectDescriptor();
      var aspectDescriptor2 = ObjectMother.GetAspectDescriptor();
      var aspectDescriptors = new[] { aspectDescriptor1, aspectDescriptor2 };
      var fakeExpression1 = ObjectMother.GetNewExpression(typeof (FuncInvocation<object, string, int, int>));
      var fakeExpression2 = ObjectMother.GetNewExpression (typeof (OuterInvocation));

      var fakeField = ObjectMother.GetFieldWrapper(typeof(AspectAttribute[]));
      aspectDescriptorDictionaryMock
          .Expect (x => x[aspectDescriptor1])
          .Return (Tuple.Create (fakeField, 0));
      aspectDescriptorDictionaryMock
          .Expect (x => x[aspectDescriptor2])
          .Return (Tuple.Create (fakeField, 1));

      _invocationExpressionHelperMock
          .Expect (
              x => x.CreateInnermostInvocation (
                  Arg<ThisExpression>.Is.Anything,
                  Arg<Type>.Is.Equal (typeof (FuncInvocation<object, string, int, int>)),
                  Arg<ParameterExpression>.Is.Equal (invocationContext),
                  Arg<IFieldWrapper>.Is.Equal (delegateField)))
          .Return (fakeExpression1);
      object[] arguments = null;
      _invocationExpressionHelperMock
          .Expect (x => x.CreateOuterInvocation (null, null, null, null))
          .IgnoreArguments()
          .WhenCalled (x => arguments = x.Arguments)
          .Return (fakeExpression2);

      var result = _expressionHelper.CreateInvocationExpressions (
          innerInvocationType, invocationContext, delegateField, aspectDescriptorDictionaryMock, aspectDescriptors).ToArray();
      
      var aspect1ActualParameterExpression = result[0].Item1;
      var aspect1ActualAssignExpression = result[0].Item2;
      var aspect2ActualParameterExpression = result[1].Item1;
      var aspect2ActualAssignExpression = result[1].Item2;

      _invocationExpressionHelperMock.VerifyAllExpectations();

      var aspect1ExpectedParameterExpression = Expression.Variable (typeof (FuncInvocation<object, string, int, int>), "ivc0");
      var aspect1ExpectedAssignExpression = Expression.Assign (aspect1ActualParameterExpression, fakeExpression1);
      var aspect2ExpectedParameterExpression = Expression.Variable (typeof (OuterInvocation), "ivc1");
      var aspect2ExpectedAssignExpression = Expression.Assign (aspect2ActualParameterExpression, fakeExpression2);

      ExpressionTreeComparer.CheckAreEqualTrees (aspect1ExpectedParameterExpression, aspect1ActualParameterExpression);
      ExpressionTreeComparer.CheckAreEqualTrees (aspect1ExpectedAssignExpression, aspect1ActualAssignExpression);
      ExpressionTreeComparer.CheckAreEqualTrees (aspect2ExpectedParameterExpression, aspect2ActualParameterExpression);
      ExpressionTreeComparer.CheckAreEqualTrees (aspect2ExpectedAssignExpression, aspect2ActualAssignExpression);
      
      var previousAspect = Expression.ArrayAccess (fakeField.GetAccessExpression (null), Expression.Constant (0));
      var method = default(MethodInfo);
      ExpressionTreeComparer.CheckAreEqualTrees (previousAspect, (Expression) arguments[0]);
      Assert.That (arguments[1], Is.SameAs (aspect1ActualParameterExpression));
      Assert.That (arguments[2], Is.EqualTo (method));
      Assert.That (arguments[3], Is.SameAs (invocationContext));
    }
  }
}