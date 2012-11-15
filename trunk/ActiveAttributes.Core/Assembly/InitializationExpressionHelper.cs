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
using ActiveAttributes.Discovery.Construction;
using ActiveAttributes.Extensions;
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Assembly
{
  /// <summary>
  /// Serves as a helper for creation of <see cref="Expression"/>s.
  /// </summary>
  [ConcreteImplementation (typeof (InitializationExpressionHelper))]
  public interface IInitializationExpressionHelper
  {
    /// <summary>
    /// Creates a <see cref="MemberInitExpression"/> for a given <see cref="IConstruction"/> describing an aspect instantiation.
    /// </summary>
    MemberInitExpression CreateAspectInitExpression (IConstruction construction);

    /// <summary>
    /// Creates an <see cref="Expression"/> containing the <see cref="MethodInfo"/> for a given method.
    /// </summary>
    ConstantExpression CreateMethodInfoInitExpression (MethodInfo method);

    /// <summary>
    /// Creates an <see cref="Expression"/> containting the <see cref="PropertyInfo"/> for a given property.
    /// </summary>
    MethodCallExpression CreatePropertyInfoInitExpression (PropertyInfo property);

    /// <summary>
    /// Creates a <see cref="Expression"/> containting the <see cref="Delegate"/> for a given method.
    /// </summary>
    NewDelegateExpression CreateDelegateInitExpression (MethodInfo method, Expression thisExpression);
  }

  public class InitializationExpressionHelper : IInitializationExpressionHelper
  {
    private readonly IMethodCopyService _methodCopyService;

    public InitializationExpressionHelper (IMethodCopyService methodCopyService)
    {
      ArgumentUtility.CheckNotNull ("methodCopyService", methodCopyService);

      _methodCopyService = methodCopyService;
    }

    public MemberInitExpression CreateAspectInitExpression (IConstruction construction)
    {
      ArgumentUtility.CheckNotNull ("construction", construction);

      var constructorInfo = construction.ConstructorInfo;
      var constructorArguments = constructorInfo.GetParameters ().Select (x => x.ParameterType).Zip (
          construction.ConstructorArguments, (type, value) => Expression.Constant (value, type)).Cast<Expression> ();
      var createExpression = Expression.New (constructorInfo, constructorArguments.ToArray ());

      var memberBindingExpressions = construction.NamedArguments.Select (GetMemberBindingExpression);

      var initExpression = Expression.MemberInit (createExpression, memberBindingExpressions.Cast<MemberBinding> ());

      return initExpression;
    }

    public Expression CreateMemberInfoInitExpression (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      if (member is MethodInfo)
        return CreateMethodInfoInitExpression ((MethodInfo) member);
      if (member is PropertyInfo)
        return CreatePropertyInfoInitExpression ((PropertyInfo) member);

      throw new NotSupportedException();
    }

    public NewDelegateExpression CreateDelegateInitExpression (MethodInfo method, Expression thisExpression)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      ArgumentUtility.CheckNotNull ("thisExpression", thisExpression);
      Assertion.IsTrue (method is MutableMethodInfo);

      var mutableMethodInfo = (MutableMethodInfo) method;
      var copy = _methodCopyService.GetCopy (mutableMethodInfo);
      var delegateType = method.GetDelegateType ();

      return new NewDelegateExpression (delegateType, thisExpression, copy);
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