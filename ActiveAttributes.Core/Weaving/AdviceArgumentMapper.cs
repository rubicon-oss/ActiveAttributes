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
using System.Reflection;
using ActiveAttributes.Aspects.StrongContext;
using ActiveAttributes.Model;
using Microsoft.Scripting.Ast;
using System.Linq;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Weaving
{
  /// <summary>
  /// Creates a mapping between advice arguments and context information, i.e., instance, arguments, return value.
  /// </summary>
  public interface IAdviceArgumentMapper
  {
    IEnumerable<MemberExpression> GetMappings (Advice advice, Expression context, Expression invocation = null);
  }

  public class AdviceArgumentMapper : IAdviceArgumentMapper
  {
    private const string c_otherwiseBlamePointcutBuilder = "otherwise PointcutBuilder should have thrown an exception.";

    public IEnumerable<MemberExpression> GetMappings (Advice advice, Expression context, Expression invocation = null)
    {
      var contextType = context.Type;

      foreach (var parameter in advice.Method.GetParameters())
      {
        var parameterType = parameter.ParameterType;
        Assertion.IsTrue (parameterType.IsByRef, c_otherwiseBlamePointcutBuilder);
        parameterType = parameterType.GetElementType();

        var attributes = parameter.GetCustomAttributes (typeof (StrongContextAttributeBase), true).ToList();
        Assertion.IsTrue (attributes.Count == 1, c_otherwiseBlamePointcutBuilder);
        var attribute = attributes.Cast<StrongContextAttributeBase>().Single();

        FieldInfo field = null;

        if (attribute is InstanceAttribute)
          field = contextType.GetField ("TypedInstance");

        if (attribute is ReturnValueAttribute)
          field = contextType.GetField ("TypedReturnValue");

        var parameterAttribute = attribute as ParameterAttribute;
        if (parameterAttribute != null)
        {
          if (parameterAttribute.Index != -1)
          {
            field = contextType.GetFields ().SingleOrDefault (x => x.Name == "Arg" + parameterAttribute.Index);
          }
          else
          {
            field = contextType.GetFields().First (x => x.FieldType == parameterType);
          }
        }

        yield return Expression.Field (context, field);
      }
    }
  }
}