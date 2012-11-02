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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class MethodExpressionHelperTest
  {
    private MutableType _declaringType;
    private BodyContextBase _bodyContext;
    private MethodExpressionHelper _expressionHelper;
    private ParameterExpression _parameterExpression1;
    private ParameterExpression _parameterExpression2;

    private IInvocationExpressionHelper _invocationExpressionHelperMock;
    private IDictionary<Advice, IFieldWrapper> _adviceDictionaryMock;

    [SetUp]
    public void SetUp ()
    {
      _declaringType = ObjectMother2.GetMutableType();
      _parameterExpression1 = ObjectMother2.GetParameterExpression (typeof (string), "param1");
      _parameterExpression2 = ObjectMother2.GetParameterExpression (typeof (int), "param2");
      _bodyContext = ObjectMother2.GetBodyContextBase (_declaringType, new[] { _parameterExpression1, _parameterExpression2 });

      _invocationExpressionHelperMock = MockRepository.GenerateMock<IInvocationExpressionHelper>();
      _adviceDictionaryMock = MockRepository.GenerateMock<IDictionary<Advice, IFieldWrapper>>();

      _expressionHelper = new MethodExpressionHelper (_bodyContext, _adviceDictionaryMock, _invocationExpressionHelperMock);
    }

    [Test]
    public void GetInvocationContextExpressions ()
    {
      var invocationContextType = typeof (FuncInvocationContext<object, string, int, int>);
      var fieldMock = MockRepository.GenerateStrictMock<IFieldWrapper>();
      var fakeExpression = ObjectMother2.GetMemberExpression (typeof (MethodInfo));

      fieldMock
          .Expect (x => x.GetAccessExpression (Arg<Expression>.Matches (y => y.Type.Equals (_declaringType))))
          .Return (fakeExpression);

      var result = _expressionHelper.CreateInvocationContextExpressions (invocationContextType, fieldMock);

      fieldMock.VerifyAllExpectations();
      var parameterExpression = result.Item1;
      Assert.That (parameterExpression.Type, Is.EqualTo (typeof (FuncInvocationContext<object, string, int, int>)));
      Assert.That (parameterExpression.Name, Is.EqualTo ("ctx"));
      var binaryExpression = result.Item2;
      Assert.That (binaryExpression.Left, Is.SameAs (parameterExpression));
      Assert.That (binaryExpression.Right, Is.TypeOf<NewExpression>());
      var newExpression = (NewExpression) binaryExpression.Right;
      Assert.That (newExpression.Constructor, Is.EqualTo (typeof (FuncInvocationContext<object, string, int, int>).GetConstructors().Single()));
      Assert.That (newExpression.Arguments, Has.Count.EqualTo (4));
      Assert.That (newExpression.Arguments[0], Is.SameAs (fakeExpression));
      Assert.That (newExpression.Arguments[1], Is.TypeOf<ThisExpression>().With.Property ("Type").EqualTo (_declaringType));
      Assert.That (newExpression.Arguments[2], Is.SameAs (_parameterExpression1));
      Assert.That (newExpression.Arguments[3], Is.SameAs (_parameterExpression2));
    }

    [Test]
    public void CreateInvocationExpressions ()
    {
      var invocationType = typeof (FuncInvocation<object, string, int, int>);
      var invocationContext = ObjectMother2.GetParameterExpression();
      var advice1 = ObjectMother2.GetAdvice();
      var advice2 = ObjectMother2.GetAdvice();
      var advices = new[] { advice1, advice2 };

      var fakeDelegateField = ObjectMother2.GetFieldWrapper();
      var fakeField = ObjectMother.GetFieldWrapper (typeof (IAspect));

      var fakeExpression1 = ObjectMother.GetNewExpression (typeof (FuncInvocation<object, string, int, int>));
      var fakeExpression2 = ObjectMother.GetNewExpression (typeof (OuterInvocation));

      _adviceDictionaryMock.Expect (x => x[advice1]).Return (fakeField);

      _invocationExpressionHelperMock
          .Expect (
              x => x.CreateInnermostInvocation (
                  Arg<ThisExpression>.Is.Anything,
                  Arg<Type>.Is.Equal (typeof (FuncInvocation<object, string, int, int>)),
                  Arg<ParameterExpression>.Is.Equal (invocationContext),
                  Arg<IFieldWrapper>.Is.Equal (fakeDelegateField)))
          .Return (fakeExpression1);
      object[] arguments = null;
      _invocationExpressionHelperMock
          .Expect (x => x.CreateOuterInvocation (null, null, null, null))
          .IgnoreArguments()
          .WhenCalled (x => arguments = x.Arguments)
          .Return (fakeExpression2);

      var result = _expressionHelper.CreateInvocationExpressions (invocationType, invocationContext, fakeDelegateField, advices).ToArray();

      _adviceDictionaryMock.VerifyAllExpectations();
      _invocationExpressionHelperMock.VerifyAllExpectations();

      var parameterExpression1 = result[0].Item1;
      var binaryExpression1 = result[0].Item2;
      var parameterExpression2 = result[1].Item1;
      var binaryExpression2 = result[1].Item2;

      Assert.That (parameterExpression1.Type, Is.EqualTo (typeof (FuncInvocation<object, string, int, int>)));
      Assert.That (parameterExpression1.Name, Is.EqualTo ("ivc0"));
      Assert.That (parameterExpression2.Type, Is.EqualTo (typeof (OuterInvocation)));
      Assert.That (parameterExpression2.Name, Is.EqualTo ("ivc1"));

      Assert.That (binaryExpression1.Left, Is.SameAs (parameterExpression1));
      Assert.That (binaryExpression1.Right, Is.SameAs (fakeExpression1));
      Assert.That (binaryExpression2.Left, Is.SameAs (parameterExpression2));
      Assert.That (binaryExpression2.Right, Is.SameAs (fakeExpression2));

      Assert.That (arguments[1], Is.SameAs (parameterExpression1));
      Assert.That (arguments[2], Is.EqualTo (advice1));
      Assert.That (arguments[3], Is.SameAs (invocationContext));
    }

    [Test]
    public void CreateOutermostAspectCallExpression ()
    {
      var advice = ObjectMother2.GetAdvice (new[] { typeof (IInvocation) });
      var invocation = ObjectMother2.GetParameterExpression (typeof (IInvocation));
      var fieldMock = MockRepository.GenerateStrictMock<IFieldWrapper>();
      var fakeExpression = ObjectMother2.GetMemberExpression (advice.Method.DeclaringType, declaringType: _declaringType);

      fieldMock.Expect (x => x.GetAccessExpression (Arg<ThisExpression>.Matches (y => y.Type.Equals (_declaringType)))).Return (fakeExpression);
      _adviceDictionaryMock.Expect (x => x[advice]).Return (fieldMock);

      var result = _expressionHelper.CreateOutermostAspectCallExpression (advice, invocation);

      fieldMock.VerifyAllExpectations();
      _adviceDictionaryMock.VerifyAllExpectations();
      Assert.That (result.Object, Is.SameAs (fakeExpression));
      Assert.That (result.Method, Is.SameAs (advice.Method));
      Assert.That (result.Arguments.Single(), Is.SameAs (invocation));
    }
  }
}