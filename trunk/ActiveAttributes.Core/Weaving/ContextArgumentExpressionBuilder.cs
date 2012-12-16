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
using System.Reflection;
using ActiveAttributes.Extensions;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (ContextArgumentExpressionBuilder))]
  public interface IContextArgumentExpressionBuilder
  {
    /// <summary>Creates an <see cref="Expression"/> containing the <see cref="MethodInfo"/> for a given method.</summary>
    ConstantExpression CreateMethodInfoInitExpression (MethodInfo method);

    /// <summary>Creates an <see cref="Expression"/> containting the <see cref="PropertyInfo"/> for a given property.</summary>
    MethodCallExpression CreatePropertyInfoInitExpression (PropertyInfo property);
  }

  public class ContextArgumentExpressionBuilder : IContextArgumentExpressionBuilder
  {
    public Expression CreateMemberInfoInitExpression (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      if (member is MethodInfo)
        return CreateMethodInfoInitExpression ((MethodInfo) member);
      if (member is PropertyInfo)
        return CreatePropertyInfoInitExpression ((PropertyInfo) member);

      throw new NotSupportedException();
    }

    public ConstantExpression CreateMethodInfoInitExpression (MethodInfo method)
    {
      return Expression.Constant (method, typeof (MethodInfo));
    }

    public MethodCallExpression CreatePropertyInfoInitExpression (PropertyInfo property)
    {
      return Expression.Call (
          Expression.Constant (property.DeclaringType),
          typeof (Type).GetMethod ("GetProperty", new[] { typeof (string), typeof (BindingFlags) }),
          Expression.Constant (property.Name),
          Expression.Constant (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
    }
  }
}