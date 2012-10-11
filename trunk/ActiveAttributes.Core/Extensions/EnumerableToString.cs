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
using System.Text;

internal static partial class EnumerableExtensions
{
  public static string ToString<T> (this IEnumerable<T> collection)
  {
    return ToString (collection, t => t.ToString (), ", ");
  }

  public static string ToString<T> (this IEnumerable<T> collection, string separator)
  {
    return ToString (collection, t => t.ToString (), separator);
  }

  public static string ToString<T> (this IEnumerable<T> collection, Func<T, string> stringElement, string separator)
  {
    var sb = new StringBuilder ();
    foreach (var item in collection)
    {
      sb.Append (stringElement (item));
      sb.Append (separator);
    }
    return sb.ToString (0, Math.Max (0, sb.Length - separator.Length));
  }
}