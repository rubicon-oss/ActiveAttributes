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
using ActiveAttributes.Core.Assembly;

namespace ActiveAttributes.Core.Configuration2
{
  /// <summary>
  /// Serves as a provider for <see cref="IAspectDescriptor"/>s.
  /// </summary>
  /// <remarks>
  /// Used as a marker interface.
  /// </remarks>
  public interface IAspectDescriptorProvider { }

  /// <summary>
  /// Serves as a provider for <see cref="IAspectDescriptor"/>s on assembly level.
  /// </summary>
  /// <remarks><inheritdoc/></remarks>
  public interface IAssemblyLevelAspectDescriptorProvider : IAspectDescriptorProvider
  {
    IEnumerable<IAspectDescriptor> GetDescriptors (System.Reflection.Assembly assembly);
  }

  /// <summary>
  /// Serves as a provider for <see cref="IAspectDescriptor"/>s on type level.
  /// </summary>
  /// <remarks><inheritdoc/></remarks>
  public interface ITypeLevelAspectDescriptorProvider : IAspectDescriptorProvider
  {
    IEnumerable<IAspectDescriptor> GetDescriptors (Type type);
  }

  /// <summary>
  /// Serves as a provider for <see cref="IAspectDescriptor"/>s on method level.
  /// </summary>
  /// <remarks><inheritdoc/></remarks>
  public interface IMethodLevelAspectDescriptorProvider : IAspectDescriptorProvider
  {
    IEnumerable<IAspectDescriptor> GetDescriptors (MethodInfo method);
  }

  public class IExpressionLevelAspectDescriptorProvider : IAspectDescriptorProvider
  {
    // TODO (research)
  }
}