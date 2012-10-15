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
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Introduces fields for storing reflection information (e.g., <see cref="MethodInfo"/> and <see cref="Delegate"/>)
  /// and <see cref="AspectAttribute"/>s.
  /// </summary>
  public interface IFieldIntroducer
  {
    /// <summary>
    /// Introduces fields for storing instance and static <see cref="AspectAttribute"/>s on type-level (shared across targets).
    /// </summary>
    /// <param name="mutableType">The mutable type.</param>
    /// <returns>References to the introduced fields.</returns>
    FieldInfoContainer IntroduceTypeFields (MutableType mutableType);

    /// <summary>
    /// Introduces fields for storing instance and static <see cref="AspectAttribute"/>s on method-level.
    /// </summary>
    /// <param name="mutableType">The mutable type.</param>
    /// <param name="methodInfo">The method.</param>
    /// <returns>References to the introduced fields.</returns>
    FieldInfoContainer IntroduceMethodFields (MutableType mutableType, MethodInfo methodInfo);
  }
}