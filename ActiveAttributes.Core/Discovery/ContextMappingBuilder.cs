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
using ActiveAttributes.Aspects;
using ActiveAttributes.Aspects.StrongContext;
using ActiveAttributes.Model.Pointcuts;
using ActiveAttributes.Weaving;
using Remotion.Collections;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Discovery
{
  /// <summary>
  /// Transforms strong-context attributes into tuples with field mappings and a pointcut.
  /// </summary>
  /// <remarks>
  /// This interface is used by <see cref="AdviceBuilder"/>.
  /// The mappings are used by <see cref="AdviceCallExpressionBuilder"/>.
  /// The pointcuts are used by <see cref="PointcutEvaluator"/>.
  /// </remarks>
  [ConcreteImplementation (typeof (ContextMappingBuilder))]
  public interface IContextMappingBuilder
  {
    Tuple<IEnumerable<Predicate<FieldInfo>>, IPointcut> GetMappingsAndPointcut (MethodInfo advice);
  }

  public class ContextMappingBuilder : IContextMappingBuilder
  {
    public Tuple<IEnumerable<Predicate<FieldInfo>>, IPointcut> GetMappingsAndPointcut (MethodInfo advice)
    {
      var mappingsAndPointcuts = advice.GetParameters().Select (GetTuple).ToList();
      var mappings = mappingsAndPointcuts.Select (x => x.Item1);
      var pointcut = (IPointcut) new AllPointcut(mappingsAndPointcuts.Select (x => x.Item2));

      return Tuple.Create (mappings, pointcut);
    }

    private Tuple<Predicate<FieldInfo>, IPointcut> GetTuple (ParameterInfo parameter)
    {
      Predicate<FieldInfo> mapping= info => false;
      IPointcut pointcut= new TruePointcut();

      var parameterType = parameter.ParameterType;
      if (parameterType == typeof (IInvocation))
      {
        return Tuple.Create (mapping, pointcut);
      }
      if (parameterType == typeof (IContext))
      {
        return Tuple.Create (mapping, pointcut);
      }

      var attributes = parameter.GetCustomAttributes (typeof (StrongContextAttributeBase), true).ToList();
      Assertion.IsTrue (attributes.Count == 1, "must use exactly one strong context attribute");

      var attribute = attributes.Single();
      Assertion.IsTrue (parameterType.IsByRef);
      parameterType = parameterType.GetElementType();

      if (attribute is InstanceAttribute)
      {
        mapping = info => info.Name == "TypedInstance";
        pointcut = new TypePointcut (parameterType);
        return Tuple.Create (mapping, pointcut);
      }

      if (attribute is ReturnValueAttribute)
      {
        mapping = info => info.Name == "TypedReturnValue";
        pointcut = new ReturnTypePointcut (parameterType);
        return Tuple.Create (mapping, pointcut);
      }

      var parameterAttribute = (ParameterAttribute) attribute;

      if (parameterAttribute.Index != -1)
      {
        mapping = info => info.Name == "Arg" + parameterAttribute.Index;
        pointcut = new ArgumentIndexPointcut (parameterType, parameterAttribute.Index);
        return Tuple.Create (mapping, pointcut);
      }

      if (!string.IsNullOrEmpty (parameterAttribute.Name))
      {
        throw new NotImplementedException();
        //pointcuts.Add (new ArgumentNamePointcut (parameter.ParameterType, parameterAttribute.Name));
        //mappings.Add (info => info.Name == parameterAttribute.)
      }

      mapping = info => parameterType.IsAssignableFrom (info.FieldType);
      pointcut = new ArgumentPointcut (parameterType);

      return Tuple.Create (mapping, pointcut);
    }
  }
}