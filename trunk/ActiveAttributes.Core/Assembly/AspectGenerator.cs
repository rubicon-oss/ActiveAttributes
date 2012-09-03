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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Extensions;
using Microsoft.Scripting.Ast;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectGenerator : IAspectGenerator
  {
    private static Expression ConvertTypedArgumentToExpression (CustomAttributeTypedArgument typedArgument)
    {
      return typedArgument.ConvertTo (CreateElement, CreateArray);
    }

    private static MemberBinding GetMemberBindingExpression (CustomAttributeNamedArgument namedArgument)
    {
      var constantExpression = ConvertTypedArgumentToExpression (namedArgument.TypedValue);
      var bindingExpression = Expression.Bind (namedArgument.MemberInfo, constantExpression);
      return bindingExpression;
    }

    private static Expression CreateElement (Type type, object obj)
    {
      if (type.IsEnum)
        obj = Enum.ToObject (type, obj);

      return Expression.Constant (obj, type);
    }

    private static Expression CreateArray (Type type, IEnumerable<Expression> objs)
    {
      return Expression.NewArrayInit (type, objs);
    }

    private readonly IArrayAccessor _arrayAccessor;
    private readonly int _index;

    public AspectGenerator (IArrayAccessor arrayAccessor, int index, IAspectDescriptor descriptor)
    {
      ArgumentUtility.CheckNotNull ("arrayAccessor", arrayAccessor);
      ArgumentUtility.CheckNotNull ("descriptor", descriptor);

      _arrayAccessor = arrayAccessor;
      _index = index;
      Descriptor = descriptor;
    }

    public IAspectDescriptor Descriptor { get; private set; }

    public Expression GetStorageExpression (Expression thisExpression)
    {
      var array = _arrayAccessor.GetAccessExpression (thisExpression);
      var index = Expression.Constant (_index);
      return Expression.ArrayAccess (array, index);
    }

    public Expression GetInitExpression ()
    {
      Func<CustomAttributeTypedArgument, Expression> argumentConverter = ConvertTypedArgumentToExpression;

      var constructorInfo = Descriptor.ConstructorInfo;
      var constructorArguments = Descriptor.ConstructorArguments.Select (argumentConverter);
      var createExpression = Expression.New (constructorInfo, constructorArguments);

      var memberBindingExpressions = Descriptor.NamedArguments.Select (GetMemberBindingExpression);

      var initExpression = Expression.MemberInit (createExpression, memberBindingExpressions);

      return initExpression;
    }
  }
}