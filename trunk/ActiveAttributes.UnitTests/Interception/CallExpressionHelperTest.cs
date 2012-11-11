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
using ActiveAttributes.Interception;
using ActiveAttributes.Interception.Invocations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Interception
{
  [TestFixture]
  public class CallExpressionHelperTest
  {
    private CallExpressionHelper _expressionHelper;

    [SetUp]
    public void SetUp ()
    {
      _expressionHelper = new CallExpressionHelper();
    }

    [Test]
    public void CreateAdviceCallExpression ()
    {
      var methodInvocation = ObjectMother.GetVariableExpression (typeof (IInvocationContext));
      var advice = ObjectMother.GetMethodInfo (parameterTypes: new[] { typeof (IInvocation) });
      var aspect = ObjectMother.GetVariableExpression (advice.DeclaringType);
      var invocation = ObjectMother.GetVariableExpression (typeof (IInvocation));

      var result = _expressionHelper.CreateAdviceCallExpression (methodInvocation, aspect, advice, invocation);

      Assert.That (result.Object, Is.SameAs (aspect));
      Assert.That (result.Method, Is.SameAs (advice));
      Assert.That (result.Arguments, Has.Count.EqualTo (1));
    }

    [Test]
    public void CreateAdviceCallExpression_TypedArguments ()
    {
      string a = "";
      var methodInvocation = ObjectMother.GetVariableExpression (typeof (ActionInvocation<object, string, int>));
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Advice2 (null, ref a));
      var aspect = ObjectMother.GetVariableExpression (advice.DeclaringType);
      var invocation = ObjectMother.GetVariableExpression (typeof (IInvocation));

      var result = _expressionHelper.CreateAdviceCallExpression (methodInvocation, aspect, advice, invocation);

      var arguments = result.Arguments;
      Assert.That (arguments, Has.Count.EqualTo (2));
      Assert.That (arguments[1], Is.InstanceOf<MemberExpression>());
      var memberExpression = (MemberExpression) arguments[1];
      var field = typeof (ActionInvocation<object, string, int>).GetField ("Arg1");
      Assertion.IsTrue (field.FieldType == typeof (string));
      Assert.That (memberExpression.Member, Is.EqualTo (field));
      Assert.That (memberExpression.Expression, Is.SameAs (methodInvocation));
    }

    private class DomainType
    {
      public void Advice1 (IInvocation invocation, string arg) {}
      public void Advice2 (IInvocation invocation, ref string arg) {}
    }
  }
}