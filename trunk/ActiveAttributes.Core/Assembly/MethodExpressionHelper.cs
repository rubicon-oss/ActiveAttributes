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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (MethodExpressionHelper))]
  public interface IMethodExpressionHelper
  {
    Tuple<ParameterExpression, BinaryExpression> CreateInvocationContextExpressions (Type invocationContextType, IFieldWrapper memberInfoField);

    IEnumerable<Tuple<ParameterExpression, BinaryExpression>> CreateInvocationExpressions (
        Type innerInvocationType,
        Expression invocationContext,
        IFieldWrapper delegateField,
        IEnumerable<Advice> sortedAdvices);

    MethodCallExpression CreateOutermostAspectCallExpression (Advice outermostAdvice, ParameterExpression outermostInvocation);
  }

  public class MethodExpressionHelper : IMethodExpressionHelper
  {
    private readonly BodyContextBase _context;
    private readonly IDictionary<Advice, IFieldWrapper> _adviceDictionary;
    private readonly IInvocationExpressionHelper _invocationExpressionHelper;

    public MethodExpressionHelper (
        BodyContextBase context,
        IDictionary<Advice, IFieldWrapper> adviceDictionary,
        IInvocationExpressionHelper invocationExpressionHelper)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("adviceDictionary", adviceDictionary);
      ArgumentUtility.CheckNotNull ("invocationExpressionHelper", invocationExpressionHelper);

      _context = context;
      _adviceDictionary = adviceDictionary;
      _invocationExpressionHelper = invocationExpressionHelper;
    }

    public Tuple<ParameterExpression, BinaryExpression> CreateInvocationContextExpressions (Type invocationContextType, IFieldWrapper memberInfoField)
    {
      ArgumentUtility.CheckNotNull ("invocationContextType", invocationContextType);
      ArgumentUtility.CheckNotNull ("memberInfoField", memberInfoField);
      Assertion.IsTrue (typeof (IInvocationContext).IsAssignableFrom (invocationContextType));

      var parameterExpression = Expression.Variable (invocationContextType, "ctx");

      var constructor = invocationContextType.GetConstructors().Single();
      var memberInfoExpression = memberInfoField.GetAccessExpression (_context.This);
      var argumentExpressions = new[] { memberInfoExpression, _context.This }.Concat (_context.Parameters.Cast<Expression>());
      var newExpression = Expression.New (constructor, argumentExpressions);
      var assignExpression = Expression.Assign (parameterExpression, newExpression);

      return Tuple.Create (parameterExpression, assignExpression);
    }

    public IEnumerable<Tuple<ParameterExpression, BinaryExpression>> CreateInvocationExpressions (
        Type innerInvocationType,
        Expression invocationContext,
        IFieldWrapper delegateField,
        IEnumerable<Advice> advices)
    {
      ArgumentUtility.CheckNotNull ("innerInvocationType", innerInvocationType);
      ArgumentUtility.CheckNotNull ("invocationContext", invocationContext);
      ArgumentUtility.CheckNotNull ("delegateField", delegateField);
      ArgumentUtility.CheckNotNull ("advices", advices);

      var advicesAsList = advices.ToList ();
      Assertion.IsTrue (advicesAsList.All (x => x.Scope == advicesAsList[0].Scope));

      var count = advicesAsList.Count;
      var invocations = new ParameterExpression[count];
      var invocationAssignExpression = new BinaryExpression[count];

      for (var i = 0; i < count; i++)
      {
        Type invocationType;
        NewExpression newExpression;

        if (i == 0)
        {
          invocationType = innerInvocationType;
          newExpression = _invocationExpressionHelper.CreateInnermostInvocation (_context.This, innerInvocationType, invocationContext, delegateField);
        }
        else
        {
          invocationType = typeof (OuterInvocation);
          var previousAdvice = advicesAsList[i - 1];
          var previousAspect = CreateAspectExpression (previousAdvice);
          var previousInvocation = invocations[i - 1];
          newExpression = _invocationExpressionHelper.CreateOuterInvocation (
              previousAspect, previousInvocation, previousAdvice.Method, invocationContext);
        }

        invocations[i] = Expression.Parameter (invocationType, "ivc" + i);
        invocationAssignExpression[i] = Expression.Assign (invocations[i], newExpression);
      }

      return invocations.Zip (invocationAssignExpression, Tuple.Create);
    }

    public MethodCallExpression CreateOutermostAspectCallExpression (Advice outermostAdvice, ParameterExpression outermostInvocation)
    {
      ArgumentUtility.CheckNotNull ("outermostAdvice", outermostAdvice);
      ArgumentUtility.CheckNotNull ("outermostInvocation", outermostInvocation);

      var aspect = CreateAspectExpression (outermostAdvice);
      return Expression.Call (aspect, outermostAdvice.Method, new Expression[] { outermostInvocation });
    }

    private MemberExpression CreateAspectExpression (Advice advice)
    {
      return _adviceDictionary[advice].GetAccessExpression (_context.This);
    }
  }
}