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
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Utilities;
using Remotion.Collections;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly
{
  public class Scheduler : IScheduler
  {
    private readonly IConfiguration _configuration;

    public Scheduler (IConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IEnumerable<T> GetOrdered<T> (IEnumerable<Tuple<IDescriptor, T>> aspects)
    {
      var aspectsAsCollection = aspects.ConvertToCollection();
      var descriptors = aspectsAsCollection.Select (x => x.Item1);
      var sortedDescriptors = GetOrdered (descriptors);
      var sortedValues = sortedDescriptors.Select (x => aspectsAsCollection.Single (y => y.Item1 == x)).Select (x => x.Item2);
      return sortedValues;
    }

    public IEnumerable<IDescriptor> GetOrdered (IEnumerable<IDescriptor> aspects)
    {
      var aspectsAsCollection = aspects.ConvertToCollection();

      var dependenciesByPriority = GetDependenciesByPriority2 (aspectsAsCollection).ToList();
      var dependenciesByRule = GetDependenciesByRule (aspectsAsCollection);
      var dependenciesByRuleCleaned = CleanConflicts (dependenciesByRule, dependenciesByPriority);
      var allDependencies = dependenciesByPriority.Concat (dependenciesByRuleCleaned);

      return aspectsAsCollection.TopologicalSort (allDependencies, throwIfOrderIsUndefined: true);
    }

    // TODO extract IAspectDependencyProvider ?
    private IEnumerable<Tuple<IDescriptor, IDescriptor>> GetDependenciesByPriority2 (ICollection<IDescriptor> aspects)
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


    private IEnumerable<Tuple<IDescriptor, IDescriptor>> GetDependenciesByRule (ICollection<IDescriptor> aspects)
    {
      var combinations = (from aspect1 in aspects
                          from aspect2 in aspects
                          from rule in _configuration.Rules
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

    private IEnumerable<Tuple<IDescriptor, IDescriptor>> CleanConflicts (
        IEnumerable<Tuple<IDescriptor, IDescriptor>> rules, params IEnumerable<Tuple<IDescriptor, IDescriptor>>[] rulesAlreadyDefined)
    {
      var conflictRules = rulesAlreadyDefined.SelectMany(x => x.Select(y => Tuple.Create (y.Item2, y.Item1)));
      return rules.Where (x => !conflictRules.Contains (x));
    }
  }
}