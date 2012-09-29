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
    /// Returns the directly overriden property, or null if no overriden property exists.
    /// </summary>
    /// <param name="propertyInfo">A property info.</param>
    public static PropertyInfo GetOverridenProperty (this PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      Assertion.IsTrue (propertyInfo.DeclaringType != null);

      var typeSequence = propertyInfo.DeclaringType.BaseType.CreateSequence (x => x.BaseType);
      var getMethod = propertyInfo.GetGetMethod (true);
      var setMethod = propertyInfo.GetSetMethod (true);
      var getMethodBase = getMethod != null ? getMethod.GetBaseDefinition () : null;
      var setMethodBase = setMethod != null ? setMethod.GetBaseDefinition() : null;

      var bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
      return typeSequence
          .SelectMany (x => x.GetProperties (bindingFlags))
          .FirstOrDefault (
              x => SafeCompareBaseDefinition (x.GetGetMethod (true), getMethodBase) ||
                   SafeCompareBaseDefinition (x.GetSetMethod (true), setMethodBase));
    }

    private static bool SafeCompareBaseDefinition (MethodInfo accessorMethod, MethodInfo accessorBaseMethod)
    {
      return accessorMethod != null && accessorBaseMethod != null && accessorMethod.GetBaseDefinition() == accessorBaseMethod;
    }

    public static bool IsIndexer (this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetIndexParameters().Length == 1;
    }

    public static bool HasSetter (this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetSetMethod (true) != null;
    }

    public static bool HasGetter (this PropertyInfo propertyInfo)
    {
      return propertyInfo.GetGetMethod (true) != null;
    }

    public static bool HasGetterAndSetter (this PropertyInfo propertyInfo)
    {
      return propertyInfo.HasGetter() && propertyInfo.HasSetter();
    }

  }
}