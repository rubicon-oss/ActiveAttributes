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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class CallExpressionHelperTest
  {
    [Test]
    public void CreateAdviceCallExpression ()
    {
      var fakeMethodInvocation = ObjectMother2.GetVariableExpression (typeof (IInvocationContext));
      var fakeAdvice = ObjectMother2.GetMethodInfo (parameterTypes: new[] { typeof (IInvocation) });
      var fakeAspect = ObjectMother2.GetVariableExpression (fakeAdvice.DeclaringType);
      var fakeInvocation = ObjectMother2.GetVariableExpression (typeof (IInvocation));

      var expressionHelper = new CallExpressionHelper();

      var result = expressionHelper.CreateAdviceCallExpression (fakeMethodInvocation, fakeAspect, fakeAdvice, fakeInvocation);

      Assert.That (result.Object, Is.SameAs (fakeAspect));
      Assert.That (result.Method, Is.SameAs (fakeAdvice));
      Assert.That (result.Arguments, Has.Count.EqualTo (1));
    }
  }
}