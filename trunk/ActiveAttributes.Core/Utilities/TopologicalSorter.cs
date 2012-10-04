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
using Remotion.Collections;

namespace ActiveAttributes.Core.Utilities
{
  public static class TopologicalSorter
  {
    public static IEnumerable<T> TopologicalSort<T> (
        this IEnumerable<T> items, IEnumerable<Tuple<T, T>> dependencies, bool throwIfOrderIsUndefined = false)
        where T: class
    {
      var dependenciesAsCollection = dependencies.ToList();
      var dictionary = items.ToDictionary (x => x, x => new Vertex<T> (x));
      foreach (var vertex in dictionary.Values)
        vertex.Dependencies = dependenciesAsCollection.Where (x => x.Item1 == vertex.Value).Select (x => dictionary[x.Item2]).ToList();

      return dictionary.Values.TopologicalSort (throwIfOrderIsUndefined).Select (x => x.Value);
    }

    private static IEnumerable<Vertex<T>> TopologicalSort<T> (
        this IEnumerable<Vertex<T>> graph, bool throwIfOrderIsUndefined = false)
    {
      var graphAsList = graph.ToList();
      var result = new List<Vertex<T>> (graphAsList.Count);

      while (graphAsList.Any())
      {
        var independents = graphAsList
            .Where (x => !result.Contains (x) && !graphAsList.Any (y => y.Dependencies.Contains (x)))
            .ToList();
        if (throwIfOrderIsUndefined && independents.Count > 1)
          throw new UndefinedOrderException<T> (independents.Select (x => x.Value));

        var independent = independents.FirstOrDefault();
        if (independent == null)
        {
          var scc = new StronglyConnectedComponentFinder<T>();
          var enumerable = scc.DetectCycle (graphAsList).Select (x => x.Select (y => y.Value));
          throw new CircularDependencyException<T> (enumerable);
        }

        graphAsList.Remove (independent);
        result.Add (independent);
      }

      return result;
    }
  }
}