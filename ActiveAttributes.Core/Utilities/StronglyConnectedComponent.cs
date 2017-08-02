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
using System.Collections;
using System.Collections.Generic;

namespace ActiveAttributes.Utilities
{
  internal class StronglyConnectedComponent<T> : IEnumerable<Vertex<T>>
  {
    private readonly LinkedList<Vertex<T>> _list;

    public StronglyConnectedComponent ()
    {
      _list = new LinkedList<Vertex<T>> ();
    }

    public StronglyConnectedComponent (IEnumerable<Vertex<T>> collection)
    {
      _list = new LinkedList<Vertex<T>> (collection);
    }

    public void Add (Vertex<T> vertex)
    {
      _list.AddLast (vertex);
    }

    public IEnumerator<Vertex<T>> GetEnumerator ()
    {
      return _list.GetEnumerator ();
    }

    public int Count
    {
      get { return _list.Count; }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return _list.GetEnumerator ();
    }

    public bool IsCycle
    {
      get { return _list.Count > 1; }
    }
  }
}