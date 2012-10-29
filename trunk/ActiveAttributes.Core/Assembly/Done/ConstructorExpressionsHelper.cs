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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Extensions;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly.Done
{

  [ConcreteImplementation (typeof (ConstructorExpressionHelper))]
  public interface IConstructorExpressionHelper
  {
    BinaryExpression CreateMemberInfoAssignExpression (IFieldWrapper field, MemberInfo method);
    BinaryExpression CreateDelegateAssignExpression (IFieldWrapper field, MethodInfo method);

    BinaryExpression CreateAspectAssignExpression (IFieldWrapper field, IEnumerable<IAspectDescriptor> aspectDescriptors);
  }

  public class ConstructorExpressionHelper : IConstructorExpressionHelper
  {
    private readonly IAspectInitExpressionHelper _aspectInitExpressionHelper;
    private readonly BodyContextBase _context;

    public ConstructorExpressionHelper (IAspectInitExpressionHelper aspectInitExpressionHelper, BodyContextBase context)
    {
      ArgumentUtility.CheckNotNull ("aspectInitExpressionHelper", aspectInitExpressionHelper);
      ArgumentUtility.CheckNotNull ("context", context);

      _aspectInitExpressionHelper = aspectInitExpressionHelper;
      _context = context;
    }

    public BinaryExpression CreateMemberInfoAssignExpression (IFieldWrapper field, MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("field", field);
      ArgumentUtility.CheckNotNull ("member", member);

      if (member is PropertyInfo)
        return CreateMemberInfoAssignExpression (field, (PropertyInfo) member);
      
      return CreateMemberInfoAssignExpression (field, (MethodInfo) member);
    }

    public BinaryExpression CreateDelegateAssignExpression (IFieldWrapper field, MethodInfo method)
    {
      var value = Expression.NewDelegate (method.GetDelegateType (), _context.This, method);

      return GetAssignExpression(field, value);
    }

    public BinaryExpression CreateAspectAssignExpression (IFieldWrapper field, IEnumerable<IAspectDescriptor> aspectDescriptors)
    {
      ArgumentUtility.CheckNotNull ("field", field);
      ArgumentUtility.CheckNotNull ("aspectDescriptors", aspectDescriptors);
      Assertion.IsTrue (field.Field.IsStatic == aspectDescriptors.All (x => x.Scope == Scope.Static));

      var value = Expression.NewArrayInit (
          typeof (AspectAttribute),
          aspectDescriptors.Select (x => _aspectInitExpressionHelper.CreateInitExpression (x)).Cast<Expression> ());

      return GetAssignExpression (field, value);
    }


    private BinaryExpression CreateMemberInfoAssignExpression (IFieldWrapper field, MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("field", field);
      ArgumentUtility.CheckNotNull ("method", method);

      return GetAssignExpression (field, method);
    }

    private BinaryExpression CreateMemberInfoAssignExpression (IFieldWrapper field, PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("field", field);
      ArgumentUtility.CheckNotNull ("property", property);

      var value = Expression.Call (
          Expression.Constant (_context.DeclaringType),
          typeof (Type).GetMethod ("GetProperty", new[] { typeof (string), typeof (BindingFlags) }),
          Expression.Constant (property.Name),
          Expression.Constant (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

      return GetAssignExpression (field, value);
    }


    private BinaryExpression GetAssignExpression (IFieldWrapper field, object constant)
    {
      var constantExpression = Expression.Constant (constant, constant.GetType());

      return GetAssignExpression (field, constantExpression);
    }

    private BinaryExpression GetAssignExpression (IFieldWrapper field, Expression value)
    {
      return Expression.Assign (GetFieldAccessExpression(field), value);
    }

    private MemberExpression GetFieldAccessExpression (IFieldWrapper fieldWrapper)
    {
      return fieldWrapper.GetAccessExpression (_context.This);
    }




    public IndexExpression CreateAspectAccessExpression (IAspectDescriptorContainer aspectDescriptorContainer, IAspectDescriptor aspectDescriptor)
    {
      var tuple = aspectDescriptorContainer.AspectStorageInfo[aspectDescriptor];
      var field = tuple.Item1;
      var index = tuple.Item2;

      return Expression.ArrayAccess (GetFieldAccessExpression (field), Expression.Constant (index));
    }

  }
}