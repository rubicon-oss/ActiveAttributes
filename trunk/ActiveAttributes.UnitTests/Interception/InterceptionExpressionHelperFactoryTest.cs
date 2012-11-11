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
using ActiveAttributes.Core.Assembly.Storages;
using ActiveAttributes.Core.Interception;
using ActiveAttributes.Core.Interception.Invocations;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.Expressions;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Interception
{
  [TestFixture]
  public class InterceptionExpressionHelperFactoryTest
  {
    [Test]
    public void Create ()
    {
      var declaringType = ObjectMother2.GetMutableType();
      var method = ObjectMother2.GetMethodInfo();
      var parameterExpressions = ObjectMother2.GetMultiple (() => ObjectMother2.GetParameterExpression()).ToList();
      var bodyContextBase = ObjectMother2.GetBodyContextBase (declaringType, parameterExpressions);
      var fakeInvocationType = typeof (FuncInvocation<object, int>);
      var fakeAdvices = new Tuple<MethodInfo, IStorage>[0];
      var fakeMemberInfoStorage = ObjectMother2.GetStorage();
      var fakeDelegateStorage = ObjectMother2.GetStorage();

      var interceptionTypeProviderMock = MockRepository.GenerateStrictMock<IInterceptionTypeProvider>();
      interceptionTypeProviderMock.Expect (x => x.GetInvocationType (method)).Return (fakeInvocationType);

      var factory = new InterceptionExpressionHelperFactory (interceptionTypeProviderMock);
      var result = factory.Create (method, bodyContextBase, fakeAdvices, fakeMemberInfoStorage, fakeDelegateStorage);

      Check (result, "_callExpressionHelper", Is.TypeOf<CallExpressionHelper> ());
      Check (result, "_thisExpression", Is.TypeOf<ThisExpression>().With.Property ("Type").EqualTo (declaringType));
      Check (result, "_parameterExpressions", Is.EqualTo (parameterExpressions));
      Check (result, "_invocationType", Is.EqualTo (fakeInvocationType));
      Check (result, "_advices", Is.EqualTo (fakeAdvices));
      Check (result, "_memberInfoStorage", Is.EqualTo (fakeMemberInfoStorage));
      Check (result, "_delegateStorage", Is.EqualTo (fakeDelegateStorage));
    }

    private static void Check (IInterceptionExpressionHelper result, string fieldName, IResolveConstraint constraint)
    {
      Assert.That (PrivateInvoke.GetNonPublicField (result, fieldName), constraint);
    }
  }
}