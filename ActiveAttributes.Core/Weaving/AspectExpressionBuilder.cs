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
using ActiveAttributes.Extensions;
using ActiveAttributes.Weaving.Construction;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Weaving
{
  /// <summary>Serves as a helper for creation of <see cref="Expression"/>s.</summary>
  [ConcreteImplementation (typeof (AspectExpressionBuilder))]
  public interface IAspectExpressionBuilder
  {
    /// <summary>Creates a <see cref="MemberInitExpression"/> for a given <see cref="IAspectConstruction"/> describing an aspect instantiation.</summary>
    MemberInitExpression CreateAspectInitExpressions (IAspectConstruction construction);
  }

  public class AspectExpressionBuilder : IAspectExpressionBuilder
  {
    public MemberInitExpression CreateAspectInitExpressions (IAspectConstruction construction)
    {
      ArgumentUtility.CheckNotNull ("construction", construction);

      var constructorInfo = construction.ConstructorInfo;
      var constructorArguments = constructorInfo.GetParameters ().Select (x => x.ParameterType).Zip (
          construction.ConstructorArguments, (type, value) => Expression.Constant (value, type)).Cast<Expression>();
      var createExpression = Expression.New (constructorInfo, constructorArguments.ToArray());

      var memberBindingExpressions = construction.NamedArguments.Select (GetMemberBindingExpression).Cast<MemberBinding>();

      return Expression.MemberInit (createExpression, memberBindingExpressions);
    }

    private MemberAssignment GetMemberBindingExpression (ICustomAttributeNamedArgument namedArgument)
    {
      var constantExpression = ConvertTypedArgumentToExpression (namedArgument);
      var bindingExpression = Expression.Bind (namedArgument.MemberInfo, constantExpression);
      return bindingExpression;
    }

    private Expression ConvertTypedArgumentToExpression (ICustomAttributeNamedArgument typedArgument)
    {
      return typedArgument.ConvertTo<Expression> (CreateElement, CreateArray);
    }

    private NewArrayExpression CreateArray (Type type, IEnumerable<Expression> objs)
    {
      return Expression.NewArrayInit (type, objs);
    }

    private ConstantExpression CreateElement (Type type, object obj)
    {
      return Expression.Constant (obj, type);
    }
  }
}