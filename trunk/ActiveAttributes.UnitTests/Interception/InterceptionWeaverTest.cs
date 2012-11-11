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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.AdviceInfo;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Storages;
using ActiveAttributes.Core.Interception;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Interception
{
  [TestFixture]
  public class InterceptionWeaverTest
  {
    [Test]
    public void Weave ()
    {
      var expressionHelperFactoryMock = MockRepository.GenerateMock<IInterceptionExpressionHelperFactory> ();
      var expressionHelperMock = MockRepository.GenerateStrictMock<IInterceptionExpressionHelper> ();
      var aspectStorageServiceMock = MockRepository.GenerateStrictMock<IInitializationService> ();

      var method = ObjectMother2.GetMutableMethodInfo ();
      var mutableType = (MutableType) method.DeclaringType;

      var fakeMemberInfoField = ObjectMother2.GetFieldWrapper ();
      var fakeDelegateField = ObjectMother2.GetFieldWrapper ();

      var fakeAdviceMethod1 = ObjectMother2.GetMethodInfo ();
      var fakeAdviceMethod2 = ObjectMother2.GetMethodInfo ();
      var fakeAspectField1 = ObjectMother2.GetFieldWrapper ();
      var fakeAspectField2 = ObjectMother2.GetFieldWrapper ();

      var fakeContextParam = ObjectMother2.GetVariableExpression ();
      var fakeContextAssign = ObjectMother2.GetBinaryExpression ();
      var fakeInvocationParam1 = ObjectMother2.GetVariableExpression ();
      var fakeInvocationAssign1 = ObjectMother2.GetBinaryExpression ();
      var fakeInvocationParam2 = ObjectMother2.GetVariableExpression ();
      var fakeInvocationAssign2 = ObjectMother2.GetBinaryExpression ();
      var fakeAdviceCall = ObjectMother2.GetMethodCallExpression ();
      var fakeReturnValue = ObjectMother2.GetMemberExpression (method.ReturnType);
      var tuples = new[]
                   {
                       Tuple.Create (fakeInvocationParam1, fakeInvocationAssign1),
                       Tuple.Create (fakeInvocationParam2, fakeInvocationAssign2)
                   }.AsOneTime ();

      var advice1 = ObjectMother2.GetAdvice (method: fakeAdviceMethod1, execution: AdviceExecution.Around);
      var advice2 = ObjectMother2.GetAdvice (method: fakeAdviceMethod2, execution: AdviceExecution.Around);
      var advices = new[] { advice1, advice2 };

      aspectStorageServiceMock
          .Expect (x => x.AddMemberInfo (method))
          .Return (fakeMemberInfoField);
      aspectStorageServiceMock
          .Expect (x => x.AddDelegate (method))
          .Return (fakeDelegateField);

      aspectStorageServiceMock
          .Expect (x => x.GetOrAddAspect (advice1, mutableType))
          .Return (fakeAspectField1);
      aspectStorageServiceMock
          .Expect (x => x.GetOrAddAspect (advice2, mutableType))
          .Return (fakeAspectField2);

      IList<Tuple<MethodInfo, IStorage>> adviceTuples = null;
      expressionHelperFactoryMock
          .Expect (
              x => x.Create (
                  Arg.Is (method),
                  Arg<BodyContextBase>.Matches (y => y.DeclaringType == method.DeclaringType),
                  Arg<IEnumerable<Tuple<MethodInfo, IStorage>>>.Is.Anything,
                  Arg.Is (fakeMemberInfoField),
                  Arg.Is (fakeDelegateField)))
          .WhenCalled (x => adviceTuples = ((IEnumerable<Tuple<MethodInfo, IStorage>>) x.Arguments[2]).ToList())
          .Return (expressionHelperMock);

      expressionHelperMock
          .Expect (x => x.CreateInvocationContextExpressions())
          .Return (Tuple.Create (fakeContextParam, fakeContextAssign));
      expressionHelperMock
          .Expect (x => x.CreateInvocationExpressions (fakeContextParam))
          .Return (tuples.AsOneTime());
      expressionHelperMock
          .Expect (x => x.CreateOutermostAdviceCallExpression (fakeInvocationParam2))
          .Return (fakeAdviceCall);
      expressionHelperMock
          .Expect (x => x.CreateReturnValueExpression (fakeContextParam))
          .Return (fakeReturnValue);

      var interceptionWeaver = new InterceptionWeaver (expressionHelperFactoryMock, aspectStorageServiceMock);
      interceptionWeaver.Weave (method, advices.AsOneTime ());

      aspectStorageServiceMock.VerifyAllExpectations ();

      expressionHelperFactoryMock.VerifyAllExpectations ();
      Assert.That (adviceTuples[0].Item1, Is.SameAs (fakeAdviceMethod1));
      Assert.That (adviceTuples[0].Item2, Is.SameAs (fakeAspectField1));
      Assert.That (adviceTuples[1].Item1, Is.SameAs (fakeAdviceMethod2));
      Assert.That (adviceTuples[1].Item2, Is.SameAs (fakeAspectField2));

      expressionHelperMock.VerifyAllExpectations ();
      Assert.That (method.Body, Is.InstanceOf<BlockExpression> ());
      var blockExpression = (BlockExpression) method.Body;

      Assert.That (blockExpression.Variables[0], Is.SameAs (fakeContextParam));
      Assert.That (blockExpression.Variables[1], Is.SameAs (fakeInvocationParam1));
      Assert.That (blockExpression.Variables[2], Is.SameAs (fakeInvocationParam2));

      Assert.That (blockExpression.Expressions[0], Is.SameAs (fakeContextAssign));
      Assert.That (blockExpression.Expressions[1], Is.InstanceOf<BlockExpression> ());
      var blockExpression2 = (BlockExpression) blockExpression.Expressions[1];
      Assert.That (blockExpression2.Expressions, Is.EqualTo (new[] { fakeInvocationAssign1, fakeInvocationAssign2 }));
      Assert.That (blockExpression.Expressions[2], Is.SameAs (fakeAdviceCall));
      Assert.That (blockExpression.Expressions[3], Is.SameAs (fakeReturnValue));
    }
  }
}