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
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.UnitTests.Expressions;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class ExpressionTreeComparer2
  {
    [DebuggerStepThrough]
    public static void CheckTreeContains (Expression tree, Expression searchedTree)
    {
      var subTrees = GetSubTrees (tree).ToArray();
      foreach (var subTree in subTrees)
      {
        try
        {
          ExpressionTreeComparer.CheckAreEqualTrees (subTree, searchedTree);
          return;
        }
        catch (Exception)
        {
        }
      }
      throw new Exception ("Tree does not contain subtree");
    }

    private static IEnumerable<Expression> GetSubTrees (Expression tree)
    {
      yield return tree;

      var propertyInfos = tree.GetType().GetProperties (BindingFlags.Instance | BindingFlags.Public);
      foreach (var propertyInfo in propertyInfos)
      {
        var value = propertyInfo.GetValue (tree, null);
        var asExpression = value as Expression;
        if (asExpression != null)
        {
          yield return asExpression;
        }

        var asEnumerableExpressions = value as IEnumerable<Expression>;
        if (asEnumerableExpressions != null)
        {
          foreach (var subTree in Yield (asEnumerableExpressions))
            yield return subTree;
        }

        var asEnumerableParameterExpressions = value as IEnumerable<ParameterExpression>;
        if (asEnumerableParameterExpressions != null)
        {
          foreach (var subTree in Yield (asEnumerableParameterExpressions.Cast<Expression>()))
            yield return subTree;
        }
      }
    }

    private static IEnumerable<Expression> Yield (IEnumerable<Expression> enumerable)
    {
      return enumerable.SelectMany (GetSubTrees);
    }
  }
}