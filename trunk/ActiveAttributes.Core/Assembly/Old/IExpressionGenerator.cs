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
using ActiveAttributes.Core.Aspects;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.Expressions;

namespace ActiveAttributes.Core.Assembly.Old
{
  /// <summary>
  /// Creates <see cref="Expression"/> for accessing and creating <see cref="AspectAttribute"/>s.
  /// </summary>
  public interface IExpressionGenerator
  {
    /// <summary>
    /// The underlying <see cref="IAspectDescriptor"/>.
    /// </summary>
    IAspectDescriptor AspectDescriptor { get; }

    /// <summary>
    /// Creates an <see cref="Expression"/> that can be used to access the <see cref="AspectAttribute"/>.
    /// </summary>
    /// <param name="thisExpression">The <see cref="ThisExpression"/></param>
    /// <returns>An <see cref="Expression"/> accessing the <see cref="AspectAttribute"/>.</returns>
    Expression GetStorageExpression (Expression thisExpression);

    /// <summary>
    /// Creates an <see cref="Expression"/> that generates an exact copy of the <see cref="AspectAttribute"/>
    /// described by the <see cref="IAspectDescriptor"/>.
    /// </summary>
    /// <returns>An <see cref="Expression"/> creating the <see cref="AspectAttribute"/>.</returns>
    Expression GetInitExpression ();
  }
}