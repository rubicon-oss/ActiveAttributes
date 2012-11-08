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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.FieldWrapper;
using ActiveAttributes.Core.Interception;
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Interception
{
  [TestFixture]
  public class InterceptionExpressionHelperTest
  {
    private InterceptionExpressionHelper _expressionHelper;

    private IInvocationExpressionHelper _invocationExpressionHelperMock;
    private ThisExpression _thisExpression;
    private MethodInfo _interceptedMethod;
    private MethodInfo _adviceMethod1;
    private MethodInfo _adviceMethod2;
    private IFieldWrapper _aspectFieldMock1;
    private IFieldWrapper _aspectFieldMock2;
    private IFieldWrapper _memberFieldMock;
    private IFieldWrapper _delegateFieldMock;

    private MutableType _declaringType;
    private ParameterExpression _parameterExpression1;
    private ParameterExpression _parameterExpression2;
    private Type _invocationType;
    private Type _invocationContextType;


    [SetUp]
    public void SetUp ()
    {
      _declaringType = ObjectMother2.GetMutableType();
      _parameterExpression1 = ObjectMother2.GetParameterExpression (typeof (string), "param1");
      _parameterExpression2 = ObjectMother2.GetParameterExpression (typeof (int), "param2");
      _thisExpression = new ThisExpression (_declaringType);

      _invocationExpressionHelperMock = MockRepository.GenerateMock<IInvocationExpressionHelper>();
      _invocationType = typeof (FuncInvocation<object, string, int, int>);
      _invocationContextType = typeof (FuncInvocationContext<object, string, int, int>);
      _interceptedMethod = ObjectMother2.GetMethodInfo (returnType: typeof (int));
      _memberFieldMock = MockRepository.GenerateStrictMock<IFieldWrapper>();
      _delegateFieldMock = MockRepository.GenerateStrictMock<IFieldWrapper>();
      _aspectFieldMock1 = MockRepository.GenerateStrictMock<IFieldWrapper>();
      _aspectFieldMock2 = MockRepository.GenerateStrictMock<IFieldWrapper>();
      _adviceMethod1 = ObjectMother2.GetMethodInfo();
      _adviceMethod2 = ObjectMother2.GetMethodInfo (parameterTypes: new[] { typeof (IInvocation) });
      var advices = new[] { Tuple.Create (_adviceMethod1, _aspectFieldMock1), Tuple.Create (_adviceMethod2, _aspectFieldMock2) };

      _expressionHelper = new InterceptionExpressionHelper (
          _invocationExpressionHelperMock,
          _interceptedMethod,
          _thisExpression,
          new[] { _parameterExpression1, _parameterExpression2 },
          _invocationType,
          _invocationContextType,
          advices,
          _memberFieldMock,
          _delegateFieldMock);
    }


    [Test]
    public void GetInvocationContextExpressions ()
    {
      var fakeExpression = ObjectMother2.GetMemberExpression (typeof (MethodInfo));

      _memberFieldMock.Expect (x => x.GetMemberExpression (Arg.Is (_thisExpression))).Return (fakeExpression);

      var result = _expressionHelper.CreateInvocationContextExpressions();

      _memberFieldMock.VerifyAllExpectations();
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
      var invocationContext = ObjectMother2.GetParameterExpression();

      var fakeMemberExpression = ObjectMother2.GetMemberExpression();
      var fakeExpression1 = ObjectMother.GetNewExpression (typeof (FuncInvocation<object, string, int, int>));
      var fakeExpression2 = ObjectMother.GetNewExpression (typeof (OuterInvocation));

      _invocationExpressionHelperMock
          .Expect (x => x.CreateInnermostInvocation (_thisExpression, _invocationType, invocationContext, _delegateFieldMock))
          .Return (fakeExpression1);
      _aspectFieldMock1
          .Expect (x => x.GetMemberExpression (_thisExpression))
          .Return (fakeMemberExpression);
      object[] arguments = null;
      _invocationExpressionHelperMock
          .Expect (x => x.CreateOuterInvocation (null, null, null, null))
          .IgnoreArguments()
          .WhenCalled (x => arguments = x.Arguments)
          .Return (fakeExpression2);

      var result = _expressionHelper.CreateInvocationExpressions (invocationContext).ToArray();

      _invocationExpressionHelperMock.VerifyAllExpectations();
      _aspectFieldMock1.VerifyAllExpectations();

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

      Assert.That (arguments[0], Is.SameAs (fakeMemberExpression));
      Assert.That (arguments[1], Is.SameAs (parameterExpression1));
      Assert.That (arguments[2], Is.EqualTo (_adviceMethod1));
      Assert.That (arguments[3], Is.SameAs (invocationContext));
    }

    [Test]
    public void CreateOutermostAspectCallExpression ()
    {
      var invocation = ObjectMother2.GetParameterExpression (typeof (IInvocation));
      var fakeExpression = ObjectMother2.GetMemberExpression (_adviceMethod2.DeclaringType);

      _aspectFieldMock2.Expect (x => x.GetMemberExpression (_thisExpression)).Return (fakeExpression);

      var result = _expressionHelper.CreateOutermostAdviceCallExpression (invocation);

      _aspectFieldMock2.VerifyAllExpectations();
      Assert.That (result.Object, Is.SameAs (fakeExpression));
      Assert.That (result.Method, Is.SameAs (_adviceMethod2));
      Assert.That (result.Arguments.Single(), Is.SameAs (invocation));
    }

    [Test]
    public void CreateReturnValueExpression ()
    {
      var invocationContextType = typeof (FuncInvocationContext<object, string, int, int>);
      var fakeContext = ObjectMother2.GetVariableExpression (invocationContextType);

      var result = _expressionHelper.CreateReturnValueExpression (fakeContext);

      var property = invocationContextType.GetProperty ("ReturnValue");
      Assert.That (result.Member, Is.EqualTo (property));
      Assert.That (result.Expression, Is.SameAs (fakeContext));
    }
  }
}