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

      var bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
      var typeSequence = methodInfo.DeclaringType.CreateSequence (x => x.BaseType);
      var properties = typeSequence.SelectMany (x => x.GetProperties (bindingFlags));

      return properties.First (x => methodInfo.IsRootEqualTo(x.GetGetMethod (true)) || methodInfo.IsRootEqualTo(x.GetSetMethod (true)));
    }

    private static bool IsRootEqualTo (this MethodInfo method1, MethodInfo method2)
    {
      return method1.GetBaseDefinition() == method2.GetBaseDefinition();
    }

    public static EventInfo GetRelatedEventInfo (this MethodInfo methodInfo)
    {
      var methodName = methodInfo.Name;
      if (!methodName.StartsWith ("add_") && !methodName.StartsWith ("remove_"))
        return null;

      var bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
      var typeSequence = methodInfo.DeclaringType.CreateSequence (x => x.BaseType);
      var events = typeSequence.SelectMany (x => x.GetEvents (bindingFlags));

      return events.First (x => methodInfo.IsRootEqualTo (x.GetAddMethod (true)) || methodInfo.IsRootEqualTo (x.GetRemoveMethod (true)));
    }

    public static bool BelongsToEvent (this MethodInfo methodInfo)
    {
      return methodInfo.GetRelatedEventInfo() != null;
    }

    public static bool BelongsToProperty (this MethodInfo methodInfo)
    {
      return methodInfo.GetRelatedEventInfo() != null;
    }
  }
}