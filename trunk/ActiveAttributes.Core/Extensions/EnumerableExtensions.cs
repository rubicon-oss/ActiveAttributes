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
using Remotion.Collections;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Extensions
{
  public static class EnumerableExtensions
  {
    public static IEnumerable<T> TopologicalSort<T> (
        this IEnumerable<T> items, IEnumerable<Tuple<T, T>> dependencies, bool throwForUndefinedOrder = false) where T: class
    {
      var itemsAsList = items.ToList();
      var dependenciesAsList = dependencies.ToList();

      var result = new List<T> (itemsAsList.Count);

      while (itemsAsList.Count > 0)
      {
        var independents = itemsAsList.Where (x => dependenciesAsList.All (y => y.Item2 != x)).ConvertToCollection();
        if (throwForUndefinedOrder && independents.Count > 1)
        {
          // TODO: exception class
          var stringBuilder = new StringBuilder();
          stringBuilder.Append ("Undefiend order of items:\r\n");
          foreach (var item in independents)
            stringBuilder.Append (item).Append ("\r\n");
          throw new InvalidOperationException (stringBuilder.ToString());
        }

        var independent = independents.FirstOrDefault();
        if (independent == default(T))
          // TODO: exception class
          throw new ArgumentException ("Circular dependencies defined");

        itemsAsList.Remove (independent);
        dependenciesAsList.RemoveAll (x => x.Item1 == independent);

        result.Add (independent);
      }

      return result;
    }
  }
}