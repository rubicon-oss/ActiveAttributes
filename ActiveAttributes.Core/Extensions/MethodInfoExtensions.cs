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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Extensions
{
  internal static class MethodInfoExtensions
  {
    /// <summary>
    /// Gets the type of the delegate.
    /// </summary>
    /// <param name="methodInfo">The method info.</param>
    /// <returns>Type of the delegate.</returns>
    public static Type GetDelegateType (this MethodInfo methodInfo)
    {
      var parameterTypes = methodInfo.GetParameters().Select (x => x.ParameterType);
      var returnType = new[] { methodInfo.ReturnType };

      var delegateTypes = parameterTypes.Concat (returnType).ToArray();
      return Expression.GetDelegateType (delegateTypes);
    }

    /// <summary>
    /// Determines whether a <see cref="MethodInfo"/> is a property accessor.
    /// </summary>
    /// <param name="methodInfo">The method info.</param>
    /// <returns><c>true</c> if the specified method info is a property accessor; otherwise, <c>false</c>.</returns>
    public static bool IsPropertyAccessor (this MethodInfo methodInfo)
    {
      return methodInfo.GetRelatedPropertyInfo() != null;
    }

    /// <summary>
    /// Determines whether a <see cref="MethodInfo"/> is a event accessor.
    /// </summary>
    /// <param name="methodInfo">The method info.</param>
    /// <returns><c>true</c> if the specified method info is a event accessor; otherwise, <c>false</c>.</returns>
    public static bool IsEventAccessor (this MethodInfo methodInfo)
    {
      return methodInfo.GetRelatedEventInfo() != null;
    }

    /// <summary>
    /// Gets the related <see cref="PropertyInfo" />.
    /// </summary>
    /// <param name="methodInfo">The method info.</param>
    /// <returns>The related <see cref="PropertyInfo" />; can be null.</returns>
    public static PropertyInfo GetRelatedPropertyInfo (this MethodInfo methodInfo)
    {
      var methodName = methodInfo.Name;
      if (!methodName.StartsWith ("get_") && !methodName.StartsWith ("set_"))
        return null;

      var bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
      var typeSequence = methodInfo.DeclaringType.CreateSequence (x => x.BaseType);
      var properties = typeSequence.SelectMany (x => x.GetProperties (bindingFlags));

      return properties.FirstOrDefault (x => methodInfo.IsRootEqualTo (x.GetGetMethod (true)) || methodInfo.IsRootEqualTo (x.GetSetMethod (true)));
    }

    /// <summary>
    /// Gets the related <see cref="EventInfo" />.
    /// </summary>
    /// <param name="methodInfo">The method info.</param>
    /// <returns>The related <see cref="EventInfo" />; can be null.</returns>
    public static EventInfo GetRelatedEventInfo (this MethodInfo methodInfo)
    {
      var methodName = methodInfo.Name;
      if (!methodName.StartsWith ("add_") && !methodName.StartsWith ("remove_"))
        return null;

      var bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
      var typeSequence = methodInfo.DeclaringType.UnderlyingSystemType.CreateSequence (x => x.BaseType);
      var events = typeSequence.SelectMany (x => x.GetEvents (bindingFlags));

      return events.FirstOrDefault (x => methodInfo.IsRootEqualTo (x.GetAddMethod (true)) || methodInfo.IsRootEqualTo (x.GetRemoveMethod (true)));
    }

    /// <summary>
    /// Determines whether the specified <see cref="MethodInfo"/> is an <see cref="Action"/>.
    /// </summary>
    /// <param name="methodInfo">The <see cref="MethodInfo"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="MethodInfo"/> is an <see cref="Action"/>; otherwise, <c>false</c>.</returns>
    public static bool IsAction (this MethodInfo methodInfo)
    {
      return methodInfo.ReturnType == typeof (void);
    }

    /// <summary>
    /// Determines whether the specified <see cref="MethodInfo"/> is an <see cref="Func{TResult}"/>.
    /// </summary>
    /// <param name="methodInfo">The <see cref="MethodInfo"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="MethodInfo"/> is a <see cref="Func{TResult}"/>; otherwise, <c>false</c>.</returns>
    public static bool IsFunc (this MethodInfo methodInfo)
    {
      return methodInfo.ReturnType != typeof (void);
    }

    public static bool IsRootEqualTo (this MethodInfo method1, MethodInfo method2)
    {
      return method1 != null && method2 != null && method1.GetBaseDefinition() == method2.GetBaseDefinition();
    }
  }
}