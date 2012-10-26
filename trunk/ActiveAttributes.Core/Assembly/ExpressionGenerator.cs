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
using ActiveAttributes.Core.Extensions;
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class ExpressionGenerator : IExpressionGenerator
  {
    private static MemberBinding GetMemberBindingExpression (ICustomAttributeNamedArgument namedArgument)
    {
      var constantExpression = ConvertTypedArgumentToExpression (namedArgument);
      var bindingExpression = Expression.Bind (namedArgument.MemberInfo, constantExpression);
      return bindingExpression;
    }

    private static Expression ConvertTypedArgumentToExpression (ICustomAttributeNamedArgument typedArgument)
    {
      return typedArgument.ConvertTo (CreateElement, CreateArray);
    }

    private static Expression CreateElement (Type type, object obj)
    {
      // TODO Should not be necessary with TypePipe custom attribute data - if it still is, fix in TypePipe
      if (type.IsEnum)
        obj = Enum.ToObject (type, obj);

      return Expression.Constant (obj, type);
    }

    private static Expression CreateArray (Type type, IEnumerable<Expression> objs)
    {
      return Expression.NewArrayInit (type, objs);
    }

    private readonly IFieldWrapper _fieldWrapper;
    private readonly int _index;

    public ExpressionGenerator (IFieldWrapper fieldWrapper, int index, IAspectDescriptor aspectDescriptor)
    {
      ArgumentUtility.CheckNotNull ("fieldWrapper", fieldWrapper);
      ArgumentUtility.CheckNotNull ("aspectDescriptor", aspectDescriptor);
      Assertion.IsTrue (index >= 0);

      _fieldWrapper = fieldWrapper;
      _index = index;
      AspectDescriptor = aspectDescriptor;
    }

    public IAspectDescriptor AspectDescriptor { get; private set; }

    public Expression GetStorageExpression (Expression thisExpression /*, IArrayAccessor arrayAccessor, int index */)
    {
      // thisExpression can be null

      var array = _fieldWrapper.GetAccessExpression (thisExpression);
      var index = Expression.Constant (_index);

      return Expression.ArrayAccess (array, index);
    }

    public Expression GetInitExpression ()
    {

      var constructorInfo = AspectDescriptor.ConstructorInfo;
      var constructorArguments = constructorInfo.GetParameters().Select (x => x.ParameterType).Zip (
          AspectDescriptor.ConstructorArguments, (type, value) => Expression.Constant (value, type)).Cast<Expression>();
      var createExpression = Expression.New (constructorInfo, constructorArguments.ToArray());

      var memberBindingExpressions = AspectDescriptor.NamedArguments.Select (GetMemberBindingExpression);

      var initExpression = Expression.MemberInit (createExpression, memberBindingExpressions);

      return initExpression;
    }
  }
}