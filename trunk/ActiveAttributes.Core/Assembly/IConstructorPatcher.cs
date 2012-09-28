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
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Patches all constructors of a <see cref="MutableType"/> for initialization of aspect related fields
  /// (e.g., reflection information, method delegate, and instance/static aspect arrays).
  /// </summary>
  public interface IConstructorPatcher
  {
    /// <summary>
    /// Adds initialization for reflection information (i.e., <see cref="PropertyInfo"/>, <see cref="EventInfo"/>, or
    /// <see cref="MethodInfo"/>) and original method delegate fields.
    /// </summary>
    /// <param name="mutableMethod">The mutable method.</param>
    /// <param name="propertyInfoFieldInfo">The field containing the <see cref="PropertyInfo"/>.</param>
    /// <param name="eventInfoFieldInfo">The field containing the <see cref="EventInfo"/>.</param>
    /// <param name="methodInfoFieldInfo">The field containing the <see cref="MethodInfo"/>.</param>
    /// <param name="delegateFieldInfo">The field containing the original method <see cref="Delegate"/>.</param>
    /// <param name="copiedMethod">The copied method.</param>
    void AddReflectionAndDelegateInitialization (
        MutableMethodInfo mutableMethod,
        FieldInfo propertyInfoFieldInfo,
        FieldInfo eventInfoFieldInfo,
        FieldInfo methodInfoFieldInfo,
        FieldInfo delegateFieldInfo,
        MutableMethodInfo copiedMethod);

    /// <summary>
    /// Adds initialization for instance and static <see cref="AspectAttribute"/> arrays.
    /// </summary>
    /// <param name="mutableType">The mutable type.</param>
    /// <param name="staticAspectsFieldInfo">The field containing the static <see cref="AspectAttribute"/> array.</param>
    /// <param name="instanceAspectsFieldInfo">The field containing the instance <see cref="AspectAttribute"/> array.</param>
    /// <param name="staticAspects">The static <see cref="AspectAttribute"/>s.</param>
    /// <param name="instanceAspects">The instance <see cref="AspectAttribute"/>s.</param>
    void AddAspectInitialization (
        MutableType mutableType,
        IArrayAccessor staticAspectsFieldInfo,
        IArrayAccessor instanceAspectsFieldInfo,
        IEnumerable<IAspectGenerator> staticAspects,
        IEnumerable<IAspectGenerator> instanceAspects);
  }
}