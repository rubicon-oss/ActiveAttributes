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
using ActiveAttributes.Core.Assembly.Accessors;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  // TODO review all docs
  /// <summary>
  /// Acts as a factory for various types.
  /// </summary>
  public interface IFactory
  {
    /// <summary>
    /// Creates a <see cref="ITypeProvider"/> for a given methd.
    /// </summary>
    /// <param name="methodInfo">The method.</param>
    /// <returns>An <see cref="ITypeProvider"/></returns>
    ITypeProvider GetTypeProvider (MethodInfo methodInfo);
    
    /// <summary>
    /// Creates an <see cref="StaticArrayAccessor"/> for a given static field.
    /// </summary>
    /// <param name="fieldInfo">The static field.</param>
    /// <returns>An <see cref="IArrayAccessor"/></returns>
    IArrayAccessor GetAccessor (FieldInfo fieldInfo);

    /// <summary>
    /// Create a <see cref="MethodPatcher"/>.
    /// </summary>
    /// <param name="mutableMethod">The mutable method.</param>
    /// <param name="aspects">The collection of <see cref="IExpressionGenerator"/>s that should be applied to the method.</param>
    /// <param name="typeProvider">A type provider for the mutable method.</param>
    /// <returns></returns>
    IMethodPatcher GetMethodPatcher (
        MutableMethodInfo mutableMethod,
        FieldInfoContainer fieldInfoContainer,
        IEnumerable<IExpressionGenerator> aspects,
        ITypeProvider typeProvider);

    /// <summary>
    /// Creates a <see cref="ExpressionGenerator"/> for a given <see cref="IArrayAccessor"/> and <see cref="IDescriptor"/>.
    /// </summary>
    /// <param name="arrayAccessor">The array containing the aspects.</param>
    /// <param name="index">The array index of the aspect.</param>
    /// <param name="descriptor">The descriptor of the aspect.</param>
    /// <returns></returns>
    IExpressionGenerator GetGenerator (IArrayAccessor arrayAccessor, int index, IDescriptor descriptor);
  }
}