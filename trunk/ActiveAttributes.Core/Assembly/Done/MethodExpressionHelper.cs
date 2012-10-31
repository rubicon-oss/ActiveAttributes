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
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly.Done
{
  [ConcreteImplementation (typeof (MethodExpressionHelper))]
  public interface IMethodExpressionHelper
  {
    Tuple<ParameterExpression, BinaryExpression> CreateInvocationContextExpressions (Type invocationContextType, IFieldWrapper memberInfoField);

    IEnumerable<Tuple<ParameterExpression, BinaryExpression>> CreateInvocationExpressions (
        Type innerInvocationType,
        Expression invocationContext,
        IFieldWrapper delegateField,
        IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> aspectDescriptorDictionary,
        IEnumerable<IAspectDescriptor> aspectDescriptors);

    MethodCallExpression CreateOutermostAspectCallExpression (
        IAspectDescriptor outermostAspectDescriptor,
        ParameterExpression outermostInvocation,
        IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> aspectDescriptorDictionary);
  }

  public class MethodExpressionHelper : IMethodExpressionHelper
  {
    private readonly MutableMethodInfo _method;
    private readonly BodyContextBase _context;
    private readonly IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> _aspectDescriptorDictionary;
    private readonly IInvocationExpressionHelper _invocationExpressionHelper;

    public MethodExpressionHelper (
      MutableMethodInfo method,
        BodyContextBase context,
        IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> aspectDescriptorDictionary,
        IInvocationExpressionHelper invocationExpressionHelper)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("aspectDescriptorDictionary", aspectDescriptorDictionary);
      ArgumentUtility.CheckNotNull ("invocationExpressionHelper", invocationExpressionHelper);

      _method = method;
      _context = context;
      _aspectDescriptorDictionary = aspectDescriptorDictionary;
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
        IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> aspectDescriptorDictionary,
        IEnumerable<IAspectDescriptor> aspectDescriptors)
    {
      ArgumentUtility.CheckNotNull ("innerInvocationType", innerInvocationType);
      ArgumentUtility.CheckNotNull ("invocationContext", invocationContext);
      ArgumentUtility.CheckNotNull ("delegateField", delegateField);
      ArgumentUtility.CheckNotNull ("aspectDescriptorDictionary", aspectDescriptorDictionary);
      ArgumentUtility.CheckNotNull ("aspectDescriptors", aspectDescriptors);

      var aspectDescriptorsAsList = aspectDescriptors.ToList ();
      Assertion.IsTrue (aspectDescriptorsAsList.All (x => x.Scope == aspectDescriptorsAsList[0].Scope));

      var count = aspectDescriptorsAsList.Count;
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
          var previousAspect = CreateAspectExpression (aspectDescriptorsAsList[i - 1], aspectDescriptorDictionary);
          var previousInvocation = invocations[i - 1];
          newExpression = _invocationExpressionHelper.CreateOuterInvocation (previousAspect, previousInvocation, null, invocationContext);
        }

        invocations[i] = Expression.Parameter (invocationType, "ivc" + i);
        invocationAssignExpression[i] = Expression.Assign (invocations[i], newExpression);
      }

      return invocations.Zip (invocationAssignExpression, Tuple.Create);
    }

    public MethodCallExpression CreateOutermostAspectCallExpression (IAspectDescriptor outermostAspectDescriptor, ParameterExpression outermostInvocation, IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> aspectDescriptorDictionary)
    {
      var aspect = CreateAspectExpression (outermostAspectDescriptor, aspectDescriptorDictionary);
      var method = GetInterceptMethod(outermostAspectDescriptor);
      var exp = Expression.Convert (aspect, method.DeclaringType);
      var exp2 = Expression.Call (exp, method);
      return exp2;
    }

    private MethodInfo GetInterceptMethod (IAspectDescriptor outermostAspectDescriptor)
    {
      MethodInfo method;
      if (typeof (MethodInterceptionAspectAttribute).IsAssignableFrom (outermostAspectDescriptor.Type))
        method = typeof (MethodInterceptionAspectAttribute).GetMethod ("OnIntercept");
      else
      {
        if (_method.Name.StartsWith ("set"))
          method = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptSet");
        else
          method = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptGet");
      }
      return method;
    }

    private IndexExpression CreateAspectExpression (
        IAspectDescriptor aspectDescriptor, IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> aspectDescriptorDictionary)
    {
      var tuple = aspectDescriptorDictionary[aspectDescriptor];
      var array = tuple.Item1;
      var index = tuple.Item2;
      return Expression.ArrayAccess (array.GetAccessExpression(_context.This), Expression.Constant (index));
    }
  }
}