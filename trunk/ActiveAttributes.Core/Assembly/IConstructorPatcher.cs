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
  /// Patches the constructors of a <see cref="MutableType"/> so that they initialize fields for
  /// the <see cref="MethodInfo"/>, and the <see cref="Delegate"/> of the original method,
  /// as well as instance, and static <see cref="AspectAttribute"/>s.
  /// </summary>
  public interface IConstructorPatcher
  {
    void AddReflectionAndDelegateInitialization (
        MutableMethodInfo mutableMethod,
        FieldInfo propertyInfoFieldInfo,
        FieldInfo eventInfoFieldInfo,
        FieldInfo methodInfoFieldInfo,
        FieldInfo delegateFieldInfo,
        MutableMethodInfo copiedMethod);

    void AddAspectInitialization (
        MutableType mutableType,
        IArrayAccessor staticAspectsFieldInfo,
        IArrayAccessor instanceAspectsFieldInfo,
        IEnumerable<IAspectGenerator> staticAspects,
        IEnumerable<IAspectGenerator> instanceAspects);
  }
}