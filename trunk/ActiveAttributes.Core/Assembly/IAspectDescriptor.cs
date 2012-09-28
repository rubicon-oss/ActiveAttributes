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
using ActiveAttributes.Core.Assembly.Configuration;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Provides information describing an <see cref="AspectAttribute"/>.
  /// </summary>
  public interface IAspectDescriptor
  {
    /// <summary>
    /// The <see cref="Type"/> of the <see cref="AspectAttribute"/>.
    /// </summary>
    Type AspectType { get; }

    /// <summary>
    /// A numeric value describing the priority over other <see cref="AspectAttribute"/>s. A higher priority means
    /// that the aspect is invoked earlier. The priority also overrides the <see cref="IAspectConfiguration"/>.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// The <see cref="AspectScope"/> (or lifetime) of the <see cref="AspectAttribute"/>.
    /// </summary>
    AspectScope Scope { get; }

    /// <summary>
    /// The <see cref="ConstructorInfo"/> used to create the <see cref="AspectAttribute"/>.
    /// </summary>
    ConstructorInfo ConstructorInfo { get; }

    /// <summary>
    /// The list of <see cref="CustomAttributeTypedArgument"/>s passed to the constructor.
    /// </summary>
    IList<CustomAttributeTypedArgument> ConstructorArguments { get; }

    /// <summary>
    /// The list of <see cref="CustomAttributeNamedArgument"/>s.
    /// </summary>
    IList<CustomAttributeNamedArgument> NamedArguments { get; }

    /// <summary>
    /// Returns <value>true</value> if the method matches the requirements of the <see cref="AspectAttribute"/>,
    /// otherwise false.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns><value>True</value> or <value>false</value>.</returns>
    bool Matches (MethodInfo method);
  }
}