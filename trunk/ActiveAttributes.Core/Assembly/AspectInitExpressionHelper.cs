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
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Infrastructure.Construction;
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Serves as a helper for creation of <see cref="MemberInitExpression"/>s for <see cref="IAspectDescriptor"/>s.
  /// </summary>
  [ConcreteImplementation (typeof (AspectInitExpressionHelper))]
  public interface IAspectInitExpressionHelper
  {
    MemberInitExpression CreateInitExpression (IAspectConstructionInfo aspectConstructionInfo);
  }

  public class AspectInitExpressionHelper : IAspectInitExpressionHelper
  {
    public MemberInitExpression CreateInitExpression (IAspectConstructionInfo aspectConstructionInfo)
    {
      var constructorInfo = aspectConstructionInfo.ConstructorInfo;
      var constructorArguments = constructorInfo.GetParameters().Select (x => x.ParameterType).Zip (
          aspectConstructionInfo.ConstructorArguments, (type, value) => Expression.Constant (value, type)).Cast<Expression>();
      var createExpression = Expression.New (constructorInfo, constructorArguments.ToArray());

      var memberBindingExpressions = aspectConstructionInfo.NamedArguments.Select (GetMemberBindingExpression);

      var initExpression = Expression.MemberInit (createExpression, memberBindingExpressions.Cast<MemberBinding>());

      return initExpression;
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
      // TODO Should not be necessary with TypePipe custom attribute data - if it still is, fix in TypePipe
      if (type.IsEnum)
        obj = Enum.ToObject (type, obj);

      return Expression.Constant (obj, type);
    }
  }
}