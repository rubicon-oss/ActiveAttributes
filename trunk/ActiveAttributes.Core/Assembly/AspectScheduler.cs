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
using System.Text;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Utilities;
using Remotion.Collections;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectScheduler : IAspectScheduler
  {
    private readonly IAspectConfiguration _configuration;

    public AspectScheduler (IAspectConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IEnumerable<IAspectGenerator> GetOrdered (IEnumerable<IAspectGenerator> aspects)
    {
      var aspectsAsCollection = aspects.ConvertToCollection();

      var dependenciesByPriority = new List<Tuple<IAspectGenerator, IAspectGenerator>> ();
      var aspectCombinations = (from aspect1 in aspectsAsCollection
                                from aspect2 in aspectsAsCollection
                                select new
                                       {
                                           Aspect1 = aspect1,
                                           Aspect2 = aspect2
                                       }).ToList();

      foreach (var item in aspectCombinations)
      {
        var compared = item.Aspect1.Descriptor.Priority.CompareTo (item.Aspect2.Descriptor.Priority);
        if (compared == 1)
          dependenciesByPriority.Add (Tuple.Create (item.Aspect1, item.Aspect2));
        else if (compared == -1)
          dependenciesByPriority.Add (Tuple.Create (item.Aspect2, item.Aspect1));
      }

      var dependenciesByRole = new List<Tuple<IAspectGenerator, IAspectGenerator>> ();
      var aspectsRuleCombination = from aspectCombination in aspectCombinations
                                   from rule in _configuration.Rules
                                   select new
                                          {
                                              aspectCombination.Aspect1,
                                              aspectCombination.Aspect2,
                                              Rule = rule
                                          };

      foreach (var item in aspectsRuleCombination)
      {
        var compared = item.Rule.Compare (item.Aspect1, item.Aspect2);
        if (compared == 0)
          continue;

        var tuple1 = Tuple.Create (item.Aspect1, item.Aspect2);
        var tuple2 = Tuple.Create (item.Aspect2, item.Aspect1);

        if (compared == -1 && !dependenciesByPriority.Contains (tuple2))
          dependenciesByRole.Add (tuple1);
        else if (compared == 1 && !dependenciesByPriority.Contains (tuple1))
          dependenciesByRole.Add (tuple2);
      }

      try
      {
        var dependencies = dependenciesByPriority.Concat (dependenciesByRole);
        return aspectsAsCollection.TopologicalSort (dependencies, throwIfOrderIsUndefined: true);
      }
      catch (ArgumentException exception)
      {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append ("Circular dependencies defined:\r\n");
        foreach (var dependency in dependenciesByRole)
        {
          stringBuilder
              .Append (dependency.Item1.Descriptor.AspectType)
              .Append (" -> ")
              .Append (dependency.Item2.Descriptor.AspectType);
        }
        var message = stringBuilder.ToString();
        throw new InvalidOperationException (message, exception);
      }
    }
  }
}