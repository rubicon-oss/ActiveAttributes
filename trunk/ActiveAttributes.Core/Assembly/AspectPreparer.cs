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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// The <see cref="AspectPreparer"/>
  /// </summary>
  public class AspectPreparer
  {
    public FieldInfo PrepareAspects (
        MutableType mutableType, MutableMethodInfo methodInfo, IEnumerable<AspectAttribute> aspectAttributes)
    {
      var aspects = aspectAttributes.ConvertToCollection();

      var staticAspectsField = mutableType.AddField (typeof (AspectAttribute[]), "_s_aspects_for_" + methodInfo.Name,
        FieldAttributes.Static | FieldAttributes.Private);
      var staticAspects = aspects.Where (x => x.Scope == AspectScope.Static).ToList ();
      var staticAspectsFieldExpression = Expression.Field (null, staticAspectsField);
      var staticAspectsElementInitExpressions = staticAspects.Select(x => Expression.New(x.GetType())).Cast<Expression>();
      var staticAspectsInitExpression = Expression.NewArrayInit (typeof (AspectAttribute), staticAspectsElementInitExpressions);
      var staticAspectsAssignExpression = Expression.Assign (staticAspectsFieldExpression, staticAspectsInitExpression);
      var staticAspectsIfNullExpression = Expression.Equal (staticAspectsFieldExpression, Expression.Constant (null, typeof (AspectAttribute[])));
      var staticAspectsAssignIfNullExpression = Expression.IfThen (staticAspectsIfNullExpression, staticAspectsAssignExpression);

      // NOTE that instanceAspectsField is initialized using the staticAspectsField in order to iterate only the instanceAspectsField and not both
      var instanceAspectsField = mutableType.AddField (typeof (AspectAttribute[]), "_m_aspects_for_" + methodInfo.Name);
      var instanceAspectsFieldExpression = new Func<BodyContextBase, Expression> (ctx =>
        Expression.Field (ctx.This, instanceAspectsField));
      var instanceAspectToStaticAspectsIndex = 0;
      var instanceAspectsElementInitExpressions = aspects
          .Select (
              x =>
              x.Scope == AspectScope.Static
                  ? (Expression) Expression.ArrayAccess (staticAspectsFieldExpression, Expression.Constant (instanceAspectToStaticAspectsIndex++))
                  : Expression.New (x.GetType()));
      var instanceAspectsInitExpression = Expression.NewArrayInit (typeof (AspectAttribute), instanceAspectsElementInitExpressions);
      var instanceAspectsAssignExpression = new Func<BodyContextBase, Expression> (ctx =>
        Expression.Assign (instanceAspectsFieldExpression(ctx), instanceAspectsInitExpression));



      foreach (var constructor in mutableType.AllMutableConstructors)
      {
        constructor.SetBody (
            ctx =>
            Expression.Block (
                staticAspectsAssignIfNullExpression,
                instanceAspectsAssignExpression (ctx),
                ctx.PreviousBody));
      }

      return instanceAspectsField;
    }

    //TODO: Fix for aspects with ctor args and settable properties/fields
    //private Expression GetAttributeInitializationExpression (CustomAttributeData attributeData)
    //{
    //  var ctorCall = Expression.New (
    //      attributeData.Constructor, 
    //      attributeData.ConstructorArguments.Select (arg => Expression.Constant (arg.Value, arg.ArgumentType)).Cast<Expression>());
    //  var memberBindings =
    //      attributeData.NamedArguments
    //          .Select (arg => Expression.Bind (arg.MemberInfo, Expression.Constant (arg.TypedValue.Value, arg.TypedValue.ArgumentType)))
    //          .Cast<MemberBinding>();
    //  return Expression.MemberInit (ctorCall, memberBindings);
    //}
  }
}