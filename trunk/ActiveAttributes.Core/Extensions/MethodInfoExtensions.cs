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
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Extensions
{
  public static class MethodInfoExtensions
  {
    private static readonly IRelatedMethodFinder _relatedMethodFinder;

    static MethodInfoExtensions ()
    {
      _relatedMethodFinder = new RelatedMethodFinder();
    }

    public static MethodInfo GetOverridenMethod (this MethodInfo methodInfo)
    {
      return _relatedMethodFinder.GetBaseMethod (methodInfo);
    }

    public static Type GetDelegateType (this MethodInfo methodInfo)
    {
      var parameterTypes = methodInfo.GetParameters().Select (x => x.ParameterType);
      var returnType = new[] { methodInfo.ReturnType };

      var delegateTypes = parameterTypes.Concat (returnType).ToArray();
      return Expression.GetDelegateType (delegateTypes);
    }

    public static PropertyInfo GetRelatedPropertyInfo (this MethodInfo methodInfo)
    {
      var methodName = methodInfo.Name;
      if (!methodName.StartsWith ("get_") && !methodName.StartsWith ("set_"))
        return null;

      var propertyName = methodName.Substring (4);
      var propertyInfo = methodInfo.DeclaringType.GetProperty (
          propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      return propertyInfo;
    }

    public static EventInfo GetRelatedEventInfo (this MethodInfo methodInfo)
    {
      var methodName = methodInfo.Name;
      if (!methodName.StartsWith ("add_") && !methodName.StartsWith ("remove_"))
        return null;

      var startIndex = methodName.IndexOf('_') + 1;
      var eventName = methodName.Substring(startIndex);
      var eventInfo = methodInfo.DeclaringType.GetEvent (
          eventName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      return eventInfo;
    }

    public static EventInfo GetRelatedEventInfo2 (this MethodInfo methodInfo)
    {
      return null;
    }
  }
}