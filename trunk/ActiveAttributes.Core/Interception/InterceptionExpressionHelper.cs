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
using ActiveAttributes.Assembly.Storages;
using ActiveAttributes.Interception.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Interception
{
  // var ctx = new FuncInvocation<TA1, TA2, TR> (_memberInfo, this, arg1, arg2, func)
  // var ivc0 = ctx
  // var ivc1 = new OuterInvocation (ctx, () => _aspect1.AdviceMethod (ivc0))
  // _aspect2.AdviceMethod (ivc1)
  // return ctx.TypedReturnValue

  public interface IInterceptionExpressionHelper
  {
    Tuple<ParameterExpression, BinaryExpression> CreateMethodInvocationExpressions ();

    IEnumerable<Tuple<ParameterExpression, BinaryExpression>> CreateAdviceInvocationExpressions (Expression methodInvocation);

    MethodCallExpression CreateOutermostAdviceCallExpression (Expression methodInvocation, Expression outermostInvocation);

    Expression CreateReturnValueExpression (Expression methodInvocation);
  }

  public class InterceptionExpressionHelper : IInterceptionExpressionHelper
  {
    private readonly ICallExpressionHelper _callExpressionHelper;
    private readonly Expression _thisExpression;
    private readonly IEnumerable<ParameterExpression> _parameterExpressions;
    private readonly Type _invocationType;
    private readonly IList<Tuple<MethodInfo, IStorage>> _advices;
    private readonly IStorage _memberInfoStorage;
    private readonly IStorage _delegateStorage;

    public InterceptionExpressionHelper (
        ICallExpressionHelper callExpressionHelper,
        Expression thisExpression,
        IEnumerable<ParameterExpression> parameterExpressions,
        Type invocationType,
        IEnumerable<Tuple<MethodInfo, IStorage>> advices,
        IStorage memberInfoStorage,
        IStorage delegateStorage)
    {
      ArgumentUtility.CheckNotNull ("callExpressionHelper", callExpressionHelper);
      ArgumentUtility.CheckNotNull ("thisExpression", thisExpression);
      ArgumentUtility.CheckNotNull ("parameterExpressions", parameterExpressions);
      ArgumentUtility.CheckNotNull ("invocationType", invocationType);
      ArgumentUtility.CheckNotNull ("advices", advices);
      ArgumentUtility.CheckNotNull ("memberInfoStorage", memberInfoStorage);
      ArgumentUtility.CheckNotNull ("delegateStorage", delegateStorage);
      //Assertion.IsTrue (typeof (IInvocationContext).IsAssignableFrom (invocationContextType));

      _callExpressionHelper = callExpressionHelper;
      _thisExpression = thisExpression;
      _parameterExpressions = parameterExpressions;
      _invocationType = invocationType;
      _advices = advices.ToList();
      _memberInfoStorage = memberInfoStorage;
      _delegateStorage = delegateStorage;
    }

    public Tuple<ParameterExpression, BinaryExpression> CreateMethodInvocationExpressions ()
    {
      var parameterExpression = Expression.Variable (_invocationType, "ctx");

      var constructor = _invocationType.GetConstructors().Single();
      var memberInfoExpression = _memberInfoStorage.CreateStorageExpression (_thisExpression);
      var delegateFieldExpression = _delegateStorage.CreateStorageExpression (_thisExpression);
      var argumentExpressions =
          new[] { memberInfoExpression, _thisExpression }
              .Concat (_parameterExpressions.Cast<Expression>())
              .Concat (new[] { delegateFieldExpression });
      var newExpression = Expression.New (constructor, argumentExpressions);
      var assignExpression = Expression.Assign (parameterExpression, newExpression);

      return Tuple.Create (parameterExpression, assignExpression);
    }

    public IEnumerable<Tuple<ParameterExpression, BinaryExpression>> CreateAdviceInvocationExpressions (Expression methodInvocation)
    {
      ArgumentUtility.CheckNotNull ("methodInvocation", methodInvocation);

      var count = _advices.Count;
      var invocations = new ParameterExpression[count];
      var invocationAssignExpression = new BinaryExpression[count];

      for (var i = 0; i < count; i++)
      {
        var invocation = methodInvocation;
        if (i != 0)
        {
          var previous = i - 1;
          var previousInvocation = invocations[previous];
          var previousAdvice = _advices[previous].Item1;
          var previousAspect = _advices[previous].Item2.CreateStorageExpression (_thisExpression);
          var previousCall = _callExpressionHelper.CreateAdviceCallExpression (
              methodInvocation, previousAspect, previousAdvice, previousInvocation);

          var constructor = typeof (OuterInvocation).GetConstructors().Single();
          var arguments = new[]
                          {
                              methodInvocation,
                              Expression.Lambda (typeof (Action), previousCall)
                          };
          invocation = Expression.New (constructor, arguments);
        }

        invocations[i] = Expression.Parameter (typeof(IInvocation), "ivc" + i);
        invocationAssignExpression[i] = Expression.Assign (invocations[i], invocation);
      }

      return invocations.Zip (invocationAssignExpression, Tuple.Create);
    }
    
    public MethodCallExpression CreateOutermostAdviceCallExpression (Expression methodInvocation, Expression outermostInvocation)
    {
      var lastTuple = _advices.Last();
      var outermostAdvice = lastTuple.Item1;
      var outermostAspect = lastTuple.Item2.CreateStorageExpression (_thisExpression);
      return _callExpressionHelper.CreateAdviceCallExpression (methodInvocation, outermostAspect, outermostAdvice, outermostInvocation);
    }

    public Expression CreateReturnValueExpression (Expression methodInvocation)
    {
      var field = methodInvocation.Type.GetField ("TypedReturnValue");
      return field != null
                 ? (Expression) Expression.Field (methodInvocation, field)
                 : Expression.Empty();
    }
  }
}