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
using ActiveAttributes.Core.Assembly.Old;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using System.Linq;

namespace ActiveAttributes.Core.Assembly
{
  public class MethodInterceptionService
  {
    private readonly IInvocationTypeProvider2 _invocationTypeProvider;
    private readonly IMethodExpressionHelperFactory _methodExpressionHelperFactory;

    public MethodInterceptionService (IInvocationTypeProvider2 invocationTypeProvider, IMethodExpressionHelperFactory methodExpressionHelperFactory)
    {
      ArgumentUtility.CheckNotNull ("invocationTypeProvider", invocationTypeProvider);
      ArgumentUtility.CheckNotNull ("methodExpressionHelperFactory", methodExpressionHelperFactory);

      _invocationTypeProvider = invocationTypeProvider;
      _methodExpressionHelperFactory = methodExpressionHelperFactory;
    }

    public void AddInterception (
        MutableMethodInfo method,
        IFieldWrapper delegateField,
        IFieldWrapper memberInfoField,
        IEnumerable<IAspectDescriptor> aspectDescriptors,
        IDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> aspectDescriptorDictionary)
    {
      method.SetBody (
          ctx =>
          {
            var helper = _methodExpressionHelperFactory.CreateMethodExpressionHelper (method, ctx, aspectDescriptorDictionary);
            Type invocationType;
            Type invocationContextType;
            _invocationTypeProvider.GetInvocationTypes (method, out invocationType, out invocationContextType);

            var invctx = helper.CreateInvocationContextExpressions (invocationContextType, memberInfoField);
            var invctxPar = invctx.Item1;
            var invctxAssign = invctx.Item2;
            var exps = helper.CreateInvocationExpressions (invocationType, invctxPar, delegateField, aspectDescriptorDictionary, aspectDescriptors);
            var invPars = exps.Select (x => x.Item1);
            var invAssigns = exps.Select (x => x.Item2);
            var callExp = helper.CreateOutermostAspectCallExpression (aspectDescriptors.Last(), invPars.Last(), aspectDescriptorDictionary);

            return Expression.Block (
                new[] { invctxPar }.Concat (invPars),
                invctxAssign,
                Expression.Block (invAssigns.Cast<Expression>()),
                callExp);
          });
      // get invocationcontext type
      // create invocationcontext
      // create tuples of invocation variables and init expressions
      // 
    }
  }
}