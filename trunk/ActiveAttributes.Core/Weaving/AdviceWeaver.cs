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
using ActiveAttributes.Aspects;
using ActiveAttributes.Model;
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using ActiveAttributes.Extensions;

namespace ActiveAttributes.Weaving
{

  [ConcreteImplementation (typeof (AdviceWeaver))]
  public interface IAdviceWeaver
  {
    void Weave (JoinPoint joinPoint, IEnumerable<Advice> advices);
  }

  public class AdviceWeaver : IAdviceWeaver
  {
    private readonly IJoinPointExpressionBuilder _joinPointExpressionBuilder;
    private readonly IWeaveBlockBuilder _weaveBlockBuilder;
    private readonly IContextArgumentStorageProvider _contextArgumentStorageProvider;
    private readonly IAspectStorageCache _aspectStorageCache;

    public AdviceWeaver (
        IJoinPointExpressionBuilder joinPointExpressionBuilder,
        IWeaveBlockBuilder weaveBlockBuilder,
        IContextArgumentStorageProvider contextArgumentStorageProvider,
        IAspectStorageCache aspectStorageCache)
    {
      _joinPointExpressionBuilder = joinPointExpressionBuilder;
      _weaveBlockBuilder = weaveBlockBuilder;
      _contextArgumentStorageProvider = contextArgumentStorageProvider;
      _aspectStorageCache = aspectStorageCache;
    }

    public void Weave (JoinPoint joinPoint, IEnumerable<Advice> advices)
    {
      ArgumentUtility.CheckNotNull ("advices", advices);

      var mutableMethod = joinPoint.Method;

      mutableMethod.SetBody (
          ctx =>
          {
            var memberInfo = _contextArgumentStorageProvider.AddMemberInfo (mutableMethod);
            var weaveTimeAdvices = advices
                .Reverse()
                .Select (x => new WeaveTimeAdvice (x, _aspectStorageCache.GetOrAddStorage (x.Aspect, joinPoint).CreateStorageExpression (ctx.This)))
                .ConvertToCollection();

            var contextTuple = _joinPointExpressionBuilder.CreateContextExpression (joinPoint, memberInfo);
            var context = contextTuple.Item1;
            var contextAssign = contextTuple.Item2;

            var innerExpression = _joinPointExpressionBuilder.CreateCallExpression (joinPoint, context);

            while (weaveTimeAdvices.Any())
            {
              var nextBlocksAdvices = weaveTimeAdvices.SkipWhile (x => x.Advice.Execution != AdviceExecution.Around).Skip (1);
              var currentBlockAdvices = weaveTimeAdvices.Except (nextBlocksAdvices).Reverse();
              weaveTimeAdvices = nextBlocksAdvices.ToList();

              innerExpression = _weaveBlockBuilder.CreateBlock (joinPoint, innerExpression, context, currentBlockAdvices);
            }

            var returnValue = _joinPointExpressionBuilder.CreateReturnExpression (joinPoint, context);

            var blockExpression = Expression.Block (
              new[] { context },
              contextAssign, 
              innerExpression, 
              returnValue);
            return blockExpression;
          });
    }
  }
}