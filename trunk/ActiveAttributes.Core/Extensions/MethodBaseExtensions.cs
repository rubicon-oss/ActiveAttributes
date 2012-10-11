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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ActiveAttributes.Core.Extensions
{
  internal static class MethodBaseExtensions
  {
    /// <summary>
    /// Determines whether a <see cref="MethodInfo"/> is compiler-generated.
    /// </summary>
    /// <param name="methodInfo">The method info.</param>
    /// <returns><c>true</c> if the <see cref="MethodInfo"/> is compiler-generated; otherwise, <c>false</c>.</returns>
    public static bool IsCompilerGenerated (this MethodBase methodInfo)
    {
      var customAttributes = methodInfo.GetCustomAttributes (typeof (CompilerGeneratedAttribute), true);
      return customAttributes.Length == 1;
    }
  }
}