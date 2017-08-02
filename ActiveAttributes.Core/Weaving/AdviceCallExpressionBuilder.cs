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
using System.Reflection;
using ActiveAttributes.Aspects;
using ActiveAttributes.Weaving.Context;
using ActiveAttributes.Weaving.Invocation;
using Microsoft.Scripting.Ast;
using System.Linq;
using Remotion.ServiceLocation;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (AdviceCallExpressionBuilder))]
  public interface IAdviceCallExpressionBuilder
  {
    MethodCallExpression CreateExpression (WeaveTimeAdvice weaveTimeAdvice, Expression context, Expression invocation = null);
  }

  public class AdviceCallExpressionBuilder : IAdviceCallExpressionBuilder
  {
    private readonly IValueMapper _valueMapper;

    public AdviceCallExpressionBuilder (IValueMapper valueMapper)
    {
      _valueMapper = valueMapper;
    }

    public MethodCallExpression CreateExpression (WeaveTimeAdvice weaveTimeAdvice, Expression context, Expression invocation = null)
    {
      var aspect = weaveTimeAdvice.Aspect;
      var advice = weaveTimeAdvice.Advice;

      Func<ParameterInfo, Expression> mapping =
          param =>
          {
            var parameterType = param.ParameterType;

            if (parameterType == typeof (IInvocation))
            {
              if (advice.Execution != AdviceExecution.Around)
                throw new Exception();
             
              return invocation;
            }

            if (parameterType == typeof (IContext))
              return context;

            if (parameterType.IsByRef)
            {
              parameterType = parameterType.GetElementType();
              return _valueMapper.GetTypeMapping (context, parameterType);
            }

            throw new Exception();
          };

      var arguments = advice.Method.GetParameters().Select (mapping);
      return Expression.Call (aspect, advice.Method, arguments);
    }
  }
}