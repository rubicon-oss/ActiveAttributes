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
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Utilities;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly.Old
{
  /// <summary>
  /// Sorts a collection of items (i.e., mostly <see cref="IAspectDescriptor"/>s) according to the ordering rules defined in
  /// the <see cref="IActiveAttributesConfiguration"/>.
  /// </summary>
  [ConcreteImplementation (typeof (AspectSorter))]
  public interface IAspectSorter
  {
    IEnumerable<T> Sort<T> (IEnumerable<Tuple<IAspectDescriptor, T>> aspects);
    IEnumerable<IAspectDescriptor> Sort (IEnumerable<IAspectDescriptor> aspects);
  }

  public class AspectSorter : IAspectSorter
  {
    private readonly IActiveAttributesConfiguration _activeAttributesConfiguration;

    public AspectSorter (IActiveAttributesConfiguration activeAttributesConfiguration)
    {
      _activeAttributesConfiguration = activeAttributesConfiguration;
    }

    public IEnumerable<T> Sort<T> (IEnumerable<Tuple<IAspectDescriptor, T>> aspects)
    {
      var aspectsAsCollection = aspects.ConvertToCollection();
      var descriptors = aspectsAsCollection.Select (x => x.Item1);
      var sortedDescriptors = Sort (descriptors);
      var sortedValues = sortedDescriptors.Select (x => aspectsAsCollection.Single (y => y.Item1 == x)).Select (x => x.Item2);
      return sortedValues;
    }

    public IEnumerable<IAspectDescriptor> Sort (IEnumerable<IAspectDescriptor> aspects)
    {
      var aspectsAsCollection = aspects.ConvertToCollection();

      var dependenciesByPriority = GetDependenciesByPriority2 (aspectsAsCollection).ToList();
      var dependenciesByRule = GetDependenciesByRule (aspectsAsCollection);
      var dependenciesByRuleCleaned = CleanConflicts (dependenciesByRule, dependenciesByPriority);
      var allDependencies = dependenciesByPriority.Concat (dependenciesByRuleCleaned);

      return aspectsAsCollection.TopologicalSort (allDependencies, throwIfOrderIsUndefined: true);
    }

    private IEnumerable<Tuple<IAspectDescriptor, IAspectDescriptor>> GetDependenciesByPriority2 (ICollection<IAspectDescriptor> aspects)
    {
      var combinations = (from aspect1 in aspects
                          from aspect2 in aspects
                          select new
                                 {
                                     Aspect1 = aspect1,
                                     Aspect2 = aspect2
                                 }).ToList();

      foreach (var item in combinations)
      {
        var compared = item.Aspect1.Priority.CompareTo (item.Aspect2.Priority);
        if (compared == 1)
          yield return Tuple.Create (item.Aspect1, item.Aspect2);
        else if (compared == -1)
          yield return Tuple.Create (item.Aspect2, item.Aspect1);
      }
    }


    private IEnumerable<Tuple<IAspectDescriptor, IAspectDescriptor>> GetDependenciesByRule (ICollection<IAspectDescriptor> aspects)
    {
      var combinations = (from aspect1 in aspects
                          from aspect2 in aspects
                          from rule in _activeAttributesConfiguration.AspectOrderingRules
                          select new
                                 {
                                     Aspect1 = aspect1,
                                     Aspect2 = aspect2,
                                     Rule = rule
                                 }).ToList();

      foreach (var item in combinations)
      {
        var compared = item.Rule.Compare (item.Aspect1, item.Aspect2);
        if (compared == -1)
          yield return Tuple.Create (item.Aspect1, item.Aspect2);
        else if (compared == 1)
          yield return Tuple.Create (item.Aspect2, item.Aspect1);
      }
    }

    private IEnumerable<Tuple<IAspectDescriptor, IAspectDescriptor>> CleanConflicts (
        IEnumerable<Tuple<IAspectDescriptor, IAspectDescriptor>> rules, params IEnumerable<Tuple<IAspectDescriptor, IAspectDescriptor>>[] rulesAlreadyDefined)
    {
      var conflictRules = rulesAlreadyDefined.SelectMany(x => x.Select(y => Tuple.Create (y.Item2, y.Item1)));
      return rules.Where (x => !conflictRules.Contains (x));
    }
  }
}