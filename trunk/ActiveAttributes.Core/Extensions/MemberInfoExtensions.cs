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

namespace ActiveAttributes.Core.Extensions
{
  public static class MemberInfoExtensions
  {
    public static bool EqualsBaseDefinition (this Type @this, Type other)
    {
      return EqualsBaseDefinition<Type> (@this, other);
    }

    public static bool EqualsBaseDefinition (this MethodInfo @this, MethodInfo other)
    {
      return EqualsBaseDefinition<MethodInfo> (@this, other);
    }

    private static bool EqualsBaseDefinition<T> (this MemberInfo @this, MemberInfo other)
    {
      if (@this == null || other == null)
        return false;

      if (typeof (T) == typeof (Type))
      {
        var thisBase = ((Type) @this).CreateSequence (x => x.BaseType).Last (x => x != typeof (object));
        var otherBase = ((Type) other).CreateSequence (x => x.BaseType).Last (x => x != typeof (object));

        return thisBase == otherBase;
      }
      else if (typeof (T) == typeof (MethodInfo))
      {
        var thisMethod = (MethodInfo) @this;
        var otherMethod = (MethodInfo) other;

        return thisMethod.GetBaseDefinition () == otherMethod.GetBaseDefinition ();
      }
      return false;
    }
  }
}