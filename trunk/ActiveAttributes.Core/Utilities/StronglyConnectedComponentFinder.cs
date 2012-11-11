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

namespace ActiveAttributes.Utilities
{
  /// <summary>
  /// Implementation of the Tarjan stronly connected components algorithm.
  /// </summary>
  /// <seealso cref="http://en.wikipedia.org/wiki/Tarjan's_strongly_connected_components_algorithm"/>
  /// <seealso cref="http://stackoverflow.com/questions/261573/best-algorithm-for-detecting-cycles-in-a-directed-graph"/>
  internal class StronglyConnectedComponentFinder<T>
  {
    private StronglyConnectedComponentList<T> _stronglyConnectedComponents;
    private Stack<Vertex<T>> _stack;
    private int _index;

    /// <summary>
    /// Calculates the sets of strongly connected vertices.
    /// </summary>
    /// <param name="graph">Graph to detect cycles within.</param>
    /// <returns>Set of strongly connected components (sets of vertices)</returns>
    public StronglyConnectedComponentList<T> DetectCycle (IEnumerable<Vertex<T>> graph)
    {
      _stronglyConnectedComponents = new StronglyConnectedComponentList<T> ();
      _index = 0;
      _stack = new Stack<Vertex<T>> ();
      foreach (var v in graph)
      {
        if (v.Index < 0)
          StrongConnect (v);
      }
      return _stronglyConnectedComponents;
    }

    private void StrongConnect (Vertex<T> v)
    {
      v.Index = _index;
      v.LowLink = _index;
      _index++;
      _stack.Push (v);

      foreach (var w1 in v.Dependencies)
      {
        if (w1.Index < 0)
        {
          StrongConnect (w1);
          v.LowLink = Math.Min (v.LowLink, w1.LowLink);
        }
        else if (_stack.Contains (w1))
          v.LowLink = Math.Min (v.LowLink, w1.Index);
      }

      if (v.LowLink != v.Index)
        return;

      var scc = new StronglyConnectedComponent<T> ();
      Vertex<T> w2;
      do
      {
        w2 = _stack.Pop ();
        scc.Add (w2);
      } while (v != w2);
      _stronglyConnectedComponents.Add (scc);
    }
  }
}