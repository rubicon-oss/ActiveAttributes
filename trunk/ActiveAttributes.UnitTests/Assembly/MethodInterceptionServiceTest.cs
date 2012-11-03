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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.FieldWrapper;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class MethodInterceptionServiceTest
  {
    [Test]
    public void AddInterception ()
    {
      var invocationTypeProviderMock = MockRepository.GenerateStrictMock<IInvocationTypeProvider2>();
      var methodExpressionHelperFactoryMock = MockRepository.GenerateStrictMock<IMethodExpressionHelperFactory>();
      var methodExpressionHelperMock = MockRepository.GenerateStrictMock<IMethodExpressionHelper>();

      var methodInterceptionService = new MethodInterceptionService (invocationTypeProviderMock, methodExpressionHelperFactoryMock);

      var method = ObjectMother2.GetMutableMethodInfo();
      var delegateField = ObjectMother2.GetFieldWrapper();
      var memberInfoField = ObjectMother2.GetFieldWrapper();
      var advice = ObjectMother2.GetAdvice();
      var advices = new[] { ObjectMother2.GetAdvice(), advice };
      var advicesDictionary = new Dictionary<Advice, IFieldWrapper>();

      var invocationContextType = typeof (FuncInvocationContext<object, string>);
      var invocationType = typeof (FuncInvocation<object, int>);

      var fakeContext = ObjectMother2.GetVariableExpression(typeof(IInvocationContext));
      var fakeContextAssign = ObjectMother2.GetBinaryExpression();

      var fakeInvocation1 = ObjectMother2.GetVariableExpression();
      var fakeInvocationAssign1 = ObjectMother2.GetBinaryExpression();
      var fakeInvocation2 = ObjectMother2.GetVariableExpression();
      var fakeInvocationAssign2 = ObjectMother2.GetBinaryExpression();

      var fakeCall = ObjectMother2.GetMethodCallExpression();

      methodExpressionHelperFactoryMock
          .Expect (x => x.CreateMethodExpressionHelper (Arg.Is (method), Arg<BodyContextBase>.Is.Anything, Arg.Is (advicesDictionary)))
          .Return (methodExpressionHelperMock);
      invocationTypeProviderMock
          .Expect (
              x => x.GetInvocationTypes (Arg.Is (method), out Arg<Type>.Out (invocationType).Dummy, out Arg<Type>.Out (invocationContextType).Dummy));
      methodExpressionHelperMock
          .Expect (x => x.CreateInvocationContextExpressions (invocationContextType, memberInfoField))
          .Return (Tuple.Create (fakeContext, fakeContextAssign));
      methodExpressionHelperMock
          .Expect (x => x.CreateInvocationExpressions (invocationType, fakeContext, delegateField, advices))
          .Return (new[] { Tuple.Create (fakeInvocation1, fakeInvocationAssign1), Tuple.Create (fakeInvocation2, fakeInvocationAssign2) });
      methodExpressionHelperMock
          .Expect (x => x.CreateOutermostAspectCallExpression(advice, fakeInvocation2))
          .Return (fakeCall);

      methodInterceptionService.AddInterception (method, delegateField, memberInfoField, advices, advicesDictionary);

      invocationTypeProviderMock.VerifyAllExpectations();
      methodExpressionHelperFactoryMock.VerifyAllExpectations();
      methodExpressionHelperMock.VerifyAllExpectations();

      Assert.That (method.Body, Is.InstanceOf<BlockExpression>());
      var blockExpression = (BlockExpression) method.Body;

      Assert.That (blockExpression.Variables[0], Is.SameAs (fakeContext));
      Assert.That (blockExpression.Variables[1], Is.SameAs (fakeInvocation1));
      Assert.That (blockExpression.Variables[2], Is.SameAs (fakeInvocation2));

      Assert.That (blockExpression.Expressions[0], Is.SameAs (fakeContextAssign));
      Assert.That (blockExpression.Expressions[1], Is.InstanceOf<BlockExpression> ());
      var blockExpression2 = (BlockExpression) blockExpression.Expressions[1];
      Assert.That (blockExpression2.Expressions, Is.EqualTo (new[] { fakeInvocationAssign1, fakeInvocationAssign2 }));
      Assert.That (blockExpression.Expressions[2], Is.SameAs (fakeCall));
      //Assert.That (blockExpression.Expressions[3], Is.TypeOf<MemberExpression>());
    }
  }
}