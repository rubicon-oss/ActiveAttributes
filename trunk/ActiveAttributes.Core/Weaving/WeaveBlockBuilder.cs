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
using ActiveAttributes.Infrastructure;
using ActiveAttributes.Weaving.Invocation;
using Microsoft.Scripting.Ast;
using Remotion;
using Remotion.ServiceLocation;
using Remotion.FunctionalProgramming;
using Remotion.Collections;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (WeaveBlockBuilder))]
  public interface IWeaveBlockBuilder
  {
    Expression CreateBlock (Expression innerExpression, ParameterExpression context, IEnumerable<WeaveTimeAdvice> advices);
  }

  public class WeaveBlockBuilder : IWeaveBlockBuilder
  {
    private readonly ConstructorInfo _staticInvocationConstructor = typeof (StaticInvocation).GetConstructors().Single();
    private readonly IAdviceCallExpressionBuilder _adviceCallExpressionBuilder;

    public WeaveBlockBuilder (IAdviceCallExpressionBuilder adviceCallExpressionBuilder)
    {
      _adviceCallExpressionBuilder = adviceCallExpressionBuilder;
    }

    public Expression CreateBlock (Expression innerExpression, ParameterExpression context, IEnumerable<WeaveTimeAdvice> advices)
    {
      var advicesAsList = advices.ConvertToCollection();

      var beforeCalls = CreateBlock (advicesAsList, AdviceExecution.Before, context);
      var afterThrowingCalls = CreateBlock (advicesAsList, AdviceExecution.AfterThrowing, context);
      var afterReturningCalls = CreateBlock (advicesAsList, AdviceExecution.AfterReturning, context);
      var afterCalls = CreateBlock (advicesAsList, AdviceExecution.After, context);

      var block = innerExpression;
      if (afterReturningCalls != null)
        block = Expression.Block (block, afterReturningCalls);
      if (afterThrowingCalls != null && afterCalls != null)
        block = Expression.TryCatchFinally (block, afterCalls, Expression.Catch (typeof (Exception), afterThrowingCalls));
      else if (afterThrowingCalls != null)
        block = Expression.TryCatch (block, Expression.Catch (typeof (Exception), afterThrowingCalls));
      else if (afterCalls != null)
        block = Expression.TryFinally (block, afterCalls);
      if (beforeCalls != null)
        block = Expression.Block (beforeCalls, block);

      var around = advicesAsList.SingleOrDefault (x => x.Advice.Execution == AdviceExecution.Around);
      if (around != null)
      {
        var invocation = Expression.New (_staticInvocationConstructor, context, Expression.Lambda (typeof (Action), block));
        var call = _adviceCallExpressionBuilder.CreateExpression (around, context, invocation);
        block = Expression.Block (invocation, call);
      }

      return block;
    }

    private Expression CreateBlock (IEnumerable<WeaveTimeAdvice> advices, AdviceExecution execution, ParameterExpression context)
    {
      var calls = advices
          .Where (x => x.Advice.Execution == execution)
          .Select (x => _adviceCallExpressionBuilder.CreateExpression (x, context))
          .Cast<Expression>().ToList();

      return calls.Any() ? Expression.Block (calls) : null;
    }

  }
}