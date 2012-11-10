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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Storages;
using ActiveAttributes.Core.Interception.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Interception
{
  public interface IInterceptionExpressionHelper
  {
    Tuple<ParameterExpression, BinaryExpression> CreateInvocationContextExpressions ();

    IEnumerable<Tuple<ParameterExpression, BinaryExpression>> CreateInvocationExpressions (Expression invocationContext);

    MethodCallExpression CreateOutermostAdviceCallExpression (Expression outermostInvocation);

    MemberExpression CreateReturnValueExpression (Expression invocationContext);
  }

  public class InterceptionExpressionHelper : IInterceptionExpressionHelper
  {
    private readonly IInvocationExpressionHelper _invocationExpressionHelper;
    private readonly MethodInfo _interceptedMethod;
    private readonly Expression _thisExpression;
    private readonly IEnumerable<ParameterExpression> _parameterExpressions;
    private readonly Type _invocationType;
    private readonly Type _invocationContextType;
    private readonly IList<Tuple<MethodInfo, IStorage>> _advices;
    private readonly IStorage _memberInfoField;
    private readonly IStorage _delegateField;

    public InterceptionExpressionHelper (
        IInvocationExpressionHelper invocationExpressionHelper,
        MethodInfo interceptedMethod,
        Expression thisExpression,
        IEnumerable<ParameterExpression> parameterExpressions,
        Type invocationType,
        Type invocationContextType,
        IEnumerable<Tuple<MethodInfo, IStorage>> advices,
        IStorage memberInfoField,
        IStorage delegateField)
    {
      ArgumentUtility.CheckNotNull ("invocationExpressionHelper", invocationExpressionHelper);
      ArgumentUtility.CheckNotNull ("interceptedMethod", interceptedMethod);
      ArgumentUtility.CheckNotNull ("thisExpression", thisExpression);
      ArgumentUtility.CheckNotNull ("parameterExpressions", parameterExpressions);
      ArgumentUtility.CheckNotNull ("invocationType", invocationType);
      ArgumentUtility.CheckNotNull ("invocationContextType", invocationContextType);
      ArgumentUtility.CheckNotNull ("advices", advices);
      ArgumentUtility.CheckNotNull ("memberInfoField", memberInfoField);
      ArgumentUtility.CheckNotNull ("delegateField", delegateField);
      //Assertion.IsTrue (typeof (IInvocationContext).IsAssignableFrom (invocationContextType));

      _invocationExpressionHelper = invocationExpressionHelper;
      _interceptedMethod = interceptedMethod;
      _thisExpression = thisExpression;
      _parameterExpressions = parameterExpressions;
      _invocationType = invocationType;
      _invocationContextType = invocationContextType;
      _advices = advices.ToList();
      _memberInfoField = memberInfoField;
      _delegateField = delegateField;
    }

    public Tuple<ParameterExpression, BinaryExpression> CreateInvocationContextExpressions ()
    {
      var parameterExpression = Expression.Variable (_invocationContextType, "ctx");

      var constructor = _invocationContextType.GetConstructors().Single();
      var memberInfoExpression = _memberInfoField.GetStorageExpression (_thisExpression);
      var argumentExpressions = new[] { memberInfoExpression, _thisExpression }.Concat (_parameterExpressions.Cast<Expression>());
      var newExpression = Expression.New (constructor, argumentExpressions);
      var assignExpression = Expression.Assign (parameterExpression, newExpression);

      return Tuple.Create (parameterExpression, assignExpression);
    }

    public IEnumerable<Tuple<ParameterExpression, BinaryExpression>> CreateInvocationExpressions (Expression invocationContext)
    {
      ArgumentUtility.CheckNotNull ("invocationContext", invocationContext);

      var count = _advices.Count;
      var invocations = new ParameterExpression[count];
      var invocationAssignExpression = new BinaryExpression[count];

      for (var i = 0; i < count; i++)
      {
        Type invocationType;
        NewExpression newExpression;

        if (i == 0)
        {
          invocationType = _invocationType;
          newExpression = _invocationExpressionHelper.CreateInnermostInvocation (_thisExpression, _invocationType, invocationContext, _delegateField);
        }
        else
        {
          invocationType = typeof (OuterInvocation);
          var previousInvocation = invocations[i - 1];
          var previousAdvice = _advices[i - 1].Item1;
          var previousAspect = _advices[i - 1].Item2.GetStorageExpression (_thisExpression);
          newExpression = _invocationExpressionHelper.CreateOuterInvocation (previousAspect, previousInvocation, previousAdvice, invocationContext);
        }

        invocations[i] = Expression.Parameter (invocationType, "ivc" + i);
        invocationAssignExpression[i] = Expression.Assign (invocations[i], newExpression);
      }

      return invocations.Zip (invocationAssignExpression, Tuple.Create);
    }

    public MethodCallExpression CreateOutermostAdviceCallExpression (Expression outermostInvocation)
    {
      ArgumentUtility.CheckNotNull ("outermostInvocation", outermostInvocation);

      var outermostAspect = _advices.Last().Item2.GetStorageExpression (_thisExpression);
      var outermostAdvice = _advices.Last().Item1;

      return Expression.Call (outermostAspect, outermostAdvice, new[] { outermostInvocation });
    }

    public MemberExpression CreateReturnValueExpression (Expression invocationContext)
    {
      // TODO return empty or property depending on interceptedMethod
      return Expression.Property (invocationContext, "ReturnValue");
    }
  }
}