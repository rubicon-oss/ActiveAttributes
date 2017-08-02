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
using ActiveAttributes.Aspects;
using ActiveAttributes.Extensions;
using System.Linq;
using ActiveAttributes.Model;
using ActiveAttributes.Model.Ordering;
using ActiveAttributes.Model.Pointcuts;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Discovery
{
  /// <summary>
  /// Transforms methods of an aspect type into <see cref="Advice"/>s.
  /// </summary>
  /// <remarks>
  /// This interface is used by the <see cref="AspectBuilder"/>.
  /// </remarks>
  [ConcreteImplementation (typeof (AdviceBuilder))]
  public interface IAdviceBuilder
  {
    IEnumerable<Advice> GetAdvices (Aspect aspect);
  }

  public class AdviceBuilder : IAdviceBuilder
  {
    private readonly IPointcutBuilder _pointcutBuilder;
    private readonly IOrderingBuilder _orderingBuilder;
    private readonly IContextMappingBuilder _contextMappingBuilder;

    public AdviceBuilder (IPointcutBuilder pointcutBuilder, IOrderingBuilder orderingBuilder, IContextMappingBuilder contextMappingBuilder)
    {
      ArgumentUtility.CheckNotNull ("pointcutBuilder", pointcutBuilder);

      _pointcutBuilder = pointcutBuilder;
      _orderingBuilder = orderingBuilder;
      _contextMappingBuilder = contextMappingBuilder;
    }

    public IEnumerable<Advice> GetAdvices (Aspect aspect)
    {
      ArgumentUtility.CheckNotNull ("aspect", aspect);

      foreach (var method in aspect.Type.GetMethods())
      {
        var adviceAttribute = method.GetCustomAttributes<AdviceAttribute> (true).SingleOrDefault();
        if (adviceAttribute == null)
          continue;

        var execution = adviceAttribute.Execution;

        var attributePointcut = _pointcutBuilder.Build (method);
        var mappings = _contextMappingBuilder.GetMappingsAndPointcut (method);
        var pointcut = new AllPointcut (new[] { attributePointcut, mappings.Item2 });

        var orderings = new List<IOrdering>();
        var crosscutting = new Crosscutting (pointcut, orderings, method.Name);
        orderings.AddRange (_orderingBuilder.BuildOrderings (crosscutting, method));

        yield return new Advice (aspect, method, execution, crosscutting);
      }
    }

  }
}