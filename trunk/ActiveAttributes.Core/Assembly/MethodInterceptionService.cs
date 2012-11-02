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
using ActiveAttributes.Core.Infrastructure;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;
using System.Linq;

namespace ActiveAttributes.Core.Assembly
{
  public interface IMethodInterceptionService
  {
    void AddInterception (
        MutableMethodInfo method,
        IFieldWrapper delegateField,
        IFieldWrapper memberInfoField,
        IEnumerable<Advice> advices,
        IDictionary<Advice, IFieldWrapper> adviceDictionary);
  }

  public class MethodInterceptionService : IMethodInterceptionService
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
      //method.SetBody (
      //    ctx =>
      //    {
      //      var helper = _methodExpressionHelperFactory.CreateMethodExpressionHelper (method, ctx, aspectDescriptorDictionary);
      //      Type invocationType;
      //      Type invocationContextType;
      //      _invocationTypeProvider.GetInvocationTypes (method, out invocationType, out invocationContextType);

      //      var invctx = helper.CreateInvocationContextExpressions (invocationContextType, memberInfoField);
      //      var invctxPar = invctx.Item1;
      //      var invctxAssign = invctx.Item2;
      //      var exps = helper.CreateInvocationExpressions (invocationType, invctxPar, delegateField, aspectDescriptorDictionary, aspectDescriptors);
      //      var invPars = exps.Select (x => x.Item1);
      //      var invAssigns = exps.Select (x => x.Item2);
      //      var callExp = helper.CreateOutermostAspectCallExpression (aspectDescriptors.Last (), invPars.Last (), aspectDescriptorDictionary);

      //      return Expression.Block (
      //          new[] { invctxPar }.Concat (invPars),
      //          invctxAssign,
      //          Expression.Block (invAssigns.Cast<Expression> ()),
      //          callExp);
      //    });
      // get invocationcontext type
      // create invocationcontext
      // create tuples of invocation variables and init expressions
      // 
    }

    public void AddInterception (
        MutableMethodInfo method,
        IFieldWrapper delegateField,
        IFieldWrapper memberInfoField,
        IEnumerable<Advice> advices,
        IDictionary<Advice, IFieldWrapper> adviceDictionary)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      ArgumentUtility.CheckNotNull ("delegateField", delegateField);
      ArgumentUtility.CheckNotNull ("memberInfoField", memberInfoField);
      ArgumentUtility.CheckNotNull ("advices", advices);
      ArgumentUtility.CheckNotNull ("adviceDictionary", adviceDictionary);

      method.SetBody (ctx => CreateNewBody(method, delegateField, memberInfoField, advices, adviceDictionary, ctx));
    }

    private Expression CreateNewBody (
        MutableMethodInfo method,
        IFieldWrapper delegateField,
        IFieldWrapper memberInfoField,
        IEnumerable<Advice> advices,
        IDictionary<Advice, IFieldWrapper> adviceDictionary,
        BodyContextBase ctx)
    {
      var advicesAsList = advices.ToList();

      Type invocationType;
      Type invocationContextType;
      _invocationTypeProvider.GetInvocationTypes (method, out invocationType, out invocationContextType);

      var expressionHelper = _methodExpressionHelperFactory.CreateMethodExpressionHelper (method, ctx, adviceDictionary);

      var contextTuple = expressionHelper.CreateInvocationContextExpressions (invocationContextType, memberInfoField);
      var context = contextTuple.Item1;
      var contextAssign = contextTuple.Item2;

      var invocationTuples = expressionHelper.CreateInvocationExpressions (invocationType, context, delegateField, advicesAsList).ToList();
      var invocations = invocationTuples.Select (x => x.Item1).ToList();
      var invocationAssigns = invocationTuples.Select (x => x.Item2).Cast<Expression>();

      var outermostCall = expressionHelper.CreateOutermostAspectCallExpression (advicesAsList.Last(), invocations.Last());

      // TODO context.ReturnValue
      return Expression.Block (
          new[] { context }.Concat (invocations),
          contextAssign,
          Expression.Block (invocationAssigns),
          outermostCall,
          Expression.Convert (Expression.Property (context, "ReturnValue"), method.ReturnType));
    }
  }
}