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
using ActiveAttributes.Assembly.Storages;
using ActiveAttributes.Interception;
using ActiveAttributes.Interception.Invocations;
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

    private ICallExpressionHelper _callExpressionHelperMock;
    private ThisExpression _thisExpression;
    private MethodInfo _adviceMethod1;
    private MethodInfo _adviceMethod2;
    private IStorage _aspectFieldMock1;
    private IStorage _aspectFieldMock2;
    private IStorage _memberFieldMock;
    private IStorage _delegateFieldMock;

    private MutableType _declaringType;
    private ParameterExpression _parameterExpression1;
    private ParameterExpression _parameterExpression2;
    private Type _invocationType;

    [SetUp]
    public void SetUp ()
    {
      _declaringType = ObjectMother.GetMutableType ();
      _parameterExpression1 = ObjectMother.GetParameterExpression (typeof (string), "param1");
      _parameterExpression2 = ObjectMother.GetParameterExpression (typeof (int), "param2");
      _thisExpression = new ThisExpression (_declaringType);

      _callExpressionHelperMock = MockRepository.GenerateStrictMock<ICallExpressionHelper> ();
      _invocationType = typeof (FuncInvocation<object, string, int, int>);
      _memberFieldMock = MockRepository.GenerateStrictMock<IStorage> ();
      _delegateFieldMock = MockRepository.GenerateStrictMock<IStorage> ();
      _aspectFieldMock1 = MockRepository.GenerateStrictMock<IStorage> ();
      _aspectFieldMock2 = MockRepository.GenerateStrictMock<IStorage> ();
      _adviceMethod1 = ObjectMother.GetMethodInfo ();
      _adviceMethod2 = ObjectMother.GetMethodInfo ();
      var advices = new[] { Tuple.Create (_adviceMethod1, _aspectFieldMock1), Tuple.Create (_adviceMethod2, _aspectFieldMock2) };

      _expressionHelper = new InterceptionExpressionHelper (
          _callExpressionHelperMock,
          _thisExpression,
          new[] { _parameterExpression1, _parameterExpression2 },
          _invocationType,
          advices,
          _memberFieldMock,
          _delegateFieldMock);
    }

    [Test]
    public void CreateMethodInvocationExpressions ()
    {
      var fakeMemberExpression = ObjectMother.GetMemberExpression (typeof (MethodInfo));
      var fakeDelegateExpression = ObjectMother.GetMemberExpression (typeof (Func<string, int, int>));

      _memberFieldMock.Expect (x => x.GetStorageExpression (Arg.Is (_thisExpression))).Return (fakeMemberExpression);
      _delegateFieldMock.Expect (x => x.GetStorageExpression (Arg.Is (_thisExpression))).Return (fakeDelegateExpression);

      var result = _expressionHelper.CreateMethodInvocationExpressions ();

      _memberFieldMock.VerifyAllExpectations ();
      var parameterExpression = result.Item1;
      Assert.That (parameterExpression.Type, Is.EqualTo (typeof (FuncInvocation<object, string, int, int>)));
      Assert.That (parameterExpression.Name, Is.EqualTo ("ctx"));
      var binaryExpression = result.Item2;
      Assert.That (binaryExpression.Left, Is.SameAs (parameterExpression));
      Assert.That (binaryExpression.Right, Is.TypeOf<NewExpression> ());
      var newExpression = (NewExpression) binaryExpression.Right;
      Assert.That (newExpression.Constructor, Is.EqualTo (typeof (FuncInvocation<object, string, int, int>).GetConstructors ().Single ()));
      Assert.That (newExpression.Arguments, Has.Count.EqualTo (5));
      Assert.That (newExpression.Arguments[0], Is.SameAs (fakeMemberExpression));
      Assert.That (newExpression.Arguments[1], Is.TypeOf<ThisExpression> ().With.Property ("Type").EqualTo (_declaringType));
      Assert.That (newExpression.Arguments[2], Is.SameAs (_parameterExpression1));
      Assert.That (newExpression.Arguments[3], Is.SameAs (_parameterExpression2));
      Assert.That (newExpression.Arguments[4], Is.SameAs (fakeDelegateExpression));
    }

    [Test]
    public void CreateAdviceInvocationExpressions ()
    {
      var methodInvocation = ObjectMother.GetParameterExpression (typeof (IInvocation));
      var fakeMemberExpression = ObjectMother.GetMemberExpression (_adviceMethod1.DeclaringType);
      var fakeExpression = ObjectMother.GetMethodCallExpression ();

      _aspectFieldMock1.Expect (x => x.GetStorageExpression (_thisExpression)).Return (fakeMemberExpression);
      object[] arguments = null;
      _callExpressionHelperMock
          .Expect (x => x.CreateAdviceCallExpression (null, null, null, null))
          .IgnoreArguments ()
          .WhenCalled (x => arguments = x.Arguments)
          .Return (fakeExpression);

      var result = _expressionHelper.CreateAdviceInvocationExpressions (methodInvocation).ToArray ();

      _aspectFieldMock1.VerifyAllExpectations ();
      _callExpressionHelperMock.VerifyAllExpectations ();

      var parameterExpression1 = result[0].Item1;
      var binaryExpression1 = result[0].Item2;
      var parameterExpression2 = result[1].Item1;
      var binaryExpression2 = result[1].Item2;

      Assert.That (parameterExpression1.Type, Is.EqualTo (typeof (IInvocation)));
      Assert.That (parameterExpression1.Name, Is.EqualTo ("ivc0"));
      Assert.That (parameterExpression2.Type, Is.EqualTo (typeof (IInvocation)));
      Assert.That (parameterExpression2.Name, Is.EqualTo ("ivc1"));

      Assert.That (arguments[0], Is.SameAs (methodInvocation));
      Assert.That (arguments[1], Is.SameAs (fakeMemberExpression));
      Assert.That (arguments[2], Is.EqualTo (_adviceMethod1));
      Assert.That (arguments[3], Is.SameAs (parameterExpression1));

      Assert.That (binaryExpression1.Left, Is.SameAs (parameterExpression1));
      Assert.That (binaryExpression2.Left, Is.SameAs (parameterExpression2));

      Assert.That (binaryExpression1.Right, Is.SameAs (methodInvocation));
      Assert.That (binaryExpression2.Right, Is.TypeOf<NewExpression> ());

      var newExpression = (NewExpression) binaryExpression2.Right;
      var constructor = typeof (OuterInvocation).GetConstructors ().Single ();
      Assert.That (newExpression.Constructor, Is.EqualTo (constructor));
      var arguments2 = newExpression.Arguments;
      Assert.That (arguments2[0], Is.SameAs (methodInvocation));
      Assert.That (arguments2[1], Is.InstanceOf<LambdaExpression> ());
      var lambdaExpression = (LambdaExpression) arguments2[1];
      Assert.That (lambdaExpression.Type, Is.EqualTo (typeof (Action)));
      Assert.That (lambdaExpression.Body, Is.SameAs (fakeExpression));
    }

    [Test]
    public void CreateOutermostAspectCallExpression ()
    {
      var methodInvocation = ObjectMother.GetParameterExpression (typeof (IInvocation));
      var outermostInvocation = ObjectMother.GetParameterExpression (typeof (IInvocation));
      var fakeMemberExpression = ObjectMother.GetMemberExpression (_adviceMethod2.DeclaringType);
      var fakeCall = ObjectMother.GetMethodCallExpression ();

      _aspectFieldMock2
        .Expect (x => x.GetStorageExpression (_thisExpression))
        .Return (fakeMemberExpression);
      _callExpressionHelperMock
          .Expect (x => x.CreateAdviceCallExpression (methodInvocation, fakeMemberExpression, _adviceMethod2, outermostInvocation))
          .Return (fakeCall);

      var result = _expressionHelper.CreateOutermostAdviceCallExpression (methodInvocation, outermostInvocation);

      _aspectFieldMock2.VerifyAllExpectations ();
      _callExpressionHelperMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeCall));
    }

    [Test]
    public void CreateReturnValueExpression_Func ()
    {
      var baseType = typeof (FuncInvocationBase<,>);
      var invocationContextType = typeof (FuncInvocation<object, int>);
      Assert.That (baseType.MakeGenericType (invocationContextType.GetGenericArguments ()).IsAssignableFrom (invocationContextType), Is.True);
      var fakeContext = ObjectMother.GetVariableExpression (invocationContextType);

      var result = _expressionHelper.CreateReturnValueExpression (fakeContext);

      Assert.That (result, Is.InstanceOf<MemberExpression> ());
      var memberExpression = (MemberExpression) result;
      var property = invocationContextType.GetField ("TypedReturnValue");
      Assert.That (memberExpression.Member, Is.EqualTo (property));
      Assert.That (memberExpression.Expression, Is.SameAs (fakeContext));
    }

    [Test]
    public void CreateReturnValueExpression_Action ()
    {
      var invocationContextType = typeof (ActionInvocation<object>);
      var fakeContext = ObjectMother.GetVariableExpression (invocationContextType);

      var result = _expressionHelper.CreateReturnValueExpression (fakeContext);

      Assert.That (result, Is.TypeOf<DefaultExpression> ());
    }
  }
}