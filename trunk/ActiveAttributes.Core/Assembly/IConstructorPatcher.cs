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
using System.Collections.Generic;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Done;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Patches all constructors of a <see cref="MutableType"/> for initialization of aspect related fields
  /// (e.g., reflection information, method delegate, and instance/static aspect arrays).
  /// </summary>
  [ConcreteImplementation (typeof (ConstructorPatcher))]
  public interface IConstructorPatcher
  {
    /// <summary>
    /// Adds initialization for reflection information (i.e., <see cref="PropertyInfo"/>, <see cref="EventInfo"/>, or
    /// <see cref="MethodInfo"/>) and original method delegate fields.
    /// </summary>
    /// <param name="mutableMethod">The mutable method.</param>
    /// <param name="fieldInfoContainer">The field data container.</param>
    /// <param name="copiedMethod">The copied method.</param>
    void AddReflectionAndDelegateInitialization (
        MutableMethodInfo mutableMethod,
        FieldInfoContainer fieldInfoContainer,
        MutableMethodInfo copiedMethod);

    /// <summary>
    /// Adds initialization for instance and static <see cref="AspectAttribute"/> arrays.
    /// </summary>
    /// <param name="mutableType">The mutable type.</param>
    /// <param name="staticAccessor">The field containing the static <see cref="AspectAttribute"/> array.</param>
    /// <param name="instanceAccessor">The field containing the instance <see cref="AspectAttribute"/> array.</param>
    /// <param name="aspects">The static <see cref="AspectAttribute"/>s.</param>
    void AddAspectInitialization (
        MutableType mutableType,
        IFieldWrapper staticAccessor,
        IFieldWrapper instanceAccessor,
        IEnumerable<IExpressionGenerator> aspects);
  }
}