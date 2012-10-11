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
using System.Linq;
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Extensions
{
  internal static class PropertyInfoExtensions
  {
    /// <summary>
    /// Returns the base <see cref="PropertyInfo"/>.
    /// </summary>
    /// <returns>The base <see cref="PropertyInfo"/>; can be null.</returns>
    public static PropertyInfo GetBaseProperty (this PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      Assertion.IsTrue (propertyInfo.DeclaringType != null);

      var typeSequence = propertyInfo.DeclaringType.BaseType.CreateSequence (x => x.BaseType);
      var getMethod = propertyInfo.GetGetMethod (true);
      var setMethod = propertyInfo.GetSetMethod (true);
      var getMethodBase = getMethod != null ? getMethod.GetBaseDefinition() : null;
      var setMethodBase = setMethod != null ? setMethod.GetBaseDefinition() : null;

      var bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
      return typeSequence
          .SelectMany (x => x.GetProperties (bindingFlags))
          .FirstOrDefault (
              x => x.GetGetMethod (true).IsRootEqualTo (getMethodBase) ||
                   x.GetSetMethod (true).IsRootEqualTo (setMethodBase));
    }

    /// <summary>
    /// Determines whether the specified <see cref="PropertyInfo"/> is an indexer.
    /// </summary>
    /// <param name="propertyInfo">The <see cref="PropertyInfo"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="PropertyInfo"/> is an indexer; otherwise, <c>false</c>.</returns>
    public static bool IsIndexer (this PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return propertyInfo.GetIndexParameters().Length == 1;
    }
  }
}