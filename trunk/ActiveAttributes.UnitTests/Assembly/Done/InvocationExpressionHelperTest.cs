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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Done;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly.Done
{
  [TestFixture]
  public class InvocationExpressionHelperTest
  {
    private InvocationExpressionHelper _expressionHelper;
    private ThisExpression _thisExpression;

    [SetUp]
    public void SetUp ()
    {
      var declaringType = ObjectMother.GetType_();
      _thisExpression = new ThisExpression (declaringType);
      _expressionHelper = new InvocationExpressionHelper();
    }

    [Test]
    public void CreateInvocation_InnermostInvocation ()
    {
      var innerInvocationType = typeof (FuncInvocation<object, int, string>);
      var invocationContext = ObjectMother.GetParameterExpression (typeof (FuncInvocationContext<object, int, string>));
      var delegateFieldStub = ObjectMother.GetFieldWrapper (typeof(Func<int, string>), thisExpression: _thisExpression);

      var actual = _expressionHelper.CreateInnermostInvocation (_thisExpression, innerInvocationType, invocationContext, delegateFieldStub);
      var expected =
          Expression.New (
              typeof (FuncInvocation<object, int, string>).GetConstructors().Single(),
              new Expression[] { invocationContext, delegateFieldStub.GetAccessExpression (_thisExpression) });

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void CreateInvocation_OuterInvocation ()
    {
      var invocationContext = ObjectMother.GetParameterExpression (typeof (IInvocationContext));
      var previousInvocation = ObjectMother.GetParameterExpression (typeof (IInvocation));
      var previousAdvice = ObjectMother.GetMethodInfo (typeof (Action<IInvocation>));
      var previousAspect = ObjectMother.GetParameterExpression (typeof (AspectAttribute)); // TODO change to IAspect

      var createDelegate = typeof (Delegate).GetMethod ("CreateDelegate", new[] { typeof (Type), typeof (object), typeof (MethodInfo) });

      var actual = _expressionHelper.CreateOuterInvocation (previousAspect, previousInvocation, previousAdvice, invocationContext);
      var expected =
          Expression.New (
              typeof (OuterInvocation).GetConstructors().Single(),
              new Expression[]
              {
                  invocationContext,
                  Expression.Convert (
                      Expression.Call (
                          null,
                          createDelegate,
                          Expression.Constant (typeof (Action<IInvocation>)),
                          previousAspect,
                          Expression.Constant (previousAdvice)),
                      typeof (Action<IInvocation>)),
                  previousInvocation
              });
      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }
  }
}