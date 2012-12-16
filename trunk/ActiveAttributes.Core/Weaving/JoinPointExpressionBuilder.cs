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
using ActiveAttributes.Weaving.Storage;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (JoinPointExpressionBuilder))]
  public interface IJoinPointExpressionBuilder
  {
    Tuple<ParameterExpression, BinaryExpression> CreateContextExpression (JoinPoint joinPoint, IStorage memberInfo);

    Expression CreateCallExpression (JoinPoint joinPoint, ParameterExpression context);

    Expression CreateReturnExpression (JoinPoint joinPoint, ParameterExpression context);
  }

  public class JoinPointExpressionBuilder : IJoinPointExpressionBuilder
  {
    private readonly IInvocationTypeProvider _invocationTypeProvider;
    private readonly IValueMapper _valueMapper;

    public JoinPointExpressionBuilder (IInvocationTypeProvider invocationTypeProvider, IValueMapper valueMapper)
    {
      _invocationTypeProvider = invocationTypeProvider;
      _valueMapper = valueMapper;
    }

    public Tuple<ParameterExpression, BinaryExpression> CreateContextExpression (JoinPoint joinPoint, IStorage memberInfo)
    {
      var contextType = _invocationTypeProvider.GetInvocationType (joinPoint.Method);
      var parameter = Expression.Variable (contextType, "ctx");

      var constructor = contextType.GetConstructors().Single();
      var arguments = new[] { memberInfo.CreateStorageExpression (joinPoint.ThisExpression), joinPoint.ThisExpression }
          .Concat (joinPoint.Parameters.Cast<Expression>())
          .Concat (Expression.Default (constructor.GetParameters().Last().ParameterType));
      var create = Expression.New (constructor, arguments);
      var assign = Expression.Assign (parameter, create);

      return Tuple.Create (parameter, assign);
    }

    public Expression CreateCallExpression (JoinPoint joinPoint, ParameterExpression context)
    {
      var arguments = joinPoint.Method.GetParameters().Select ((x, i) => _valueMapper.GetIndexMapping (context, i));
      var call = BodyContextUtility.ReplaceParameters (joinPoint.Parameters, joinPoint.Expression, arguments);

      if (joinPoint.Method.ReturnType != typeof (void))
      {
        var returnValue = _valueMapper.GetReturnMapping (context);
        call = Expression.Assign (returnValue, call);
      }

      return call;
    }

    public Expression CreateReturnExpression (JoinPoint joinPoint, ParameterExpression context)
    {
      return joinPoint.Method.ReturnType == typeof (void) ? Expression.Empty() : _valueMapper.GetReturnMapping (context);
    }
  }
}