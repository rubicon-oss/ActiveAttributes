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

namespace ActiveAttributes.Core.Extensions
{
  internal static class ArrayExtensions
  {
    public static void StableSort<T> (this T[] values, Comparison<T> comparison)
    {
      var keys = new KeyValuePair<int, T>[values.Length];
      for (var i = 0; i < values.Length; i++)
        keys[i] = new KeyValuePair<int, T> (i, values[i]);
      Array.Sort (keys, values, new StabilizingComparer<T> (comparison));
    }

    private sealed class StabilizingComparer<T> : IComparer<KeyValuePair<int, T>>
    {
      private readonly Comparison<T> _comparison;

      public StabilizingComparer (Comparison<T> comparison)
      {
        _comparison = comparison;
      }

      public int Compare (
          KeyValuePair<int, T> x,
          KeyValuePair<int, T> y)
      {
        var result = _comparison (x.Value, y.Value);
        return result != 0 ? result : x.Key.CompareTo (y.Key);
      }
    }
  }
}