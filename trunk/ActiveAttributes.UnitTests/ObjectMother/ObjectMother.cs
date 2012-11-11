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
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother
  {
    private static readonly Random s_random = new Random();

    public static T GetRandom<T> (IEnumerable<T> enumerable)
    {
      var collection = enumerable.ToList();
      return collection[s_random.Next (0, collection.Count)];
    }

    public static IEnumerable<T> GetMultiple<T> (Func<T> factory, int count = -1)
    {
      count = count >= 0 ? count : s_random.Next (0, 5);
      return Enumerable.Range (0, count).Select (x => factory());
    }

    public static IEnumerable<T> GetMultiple<T> (Func<int, T> factory, int count = -1)
    {
      count = count >= 0 ? count : s_random.Next (0, 5);
      return Enumerable.Range (0, count).Select (factory);
    }
  }
}