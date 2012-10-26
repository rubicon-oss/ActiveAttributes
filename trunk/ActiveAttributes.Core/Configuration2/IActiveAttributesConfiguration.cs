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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using Remotion.ServiceLocation;

namespace ActiveAttributes.Core.Configuration2
{
  /// <summary>
  /// Serves as a configuration object for all concerns of <see cref="AspectAttribute"/>s.
  /// </summary>
  [ConcreteImplementation (typeof (ActiveAttributesConfiguration))]
  public interface IActiveAttributesConfiguration
  {
    /// <summary>
    /// A list of <see cref="IAspectDescriptorProvider"/>s used to get <see cref="IAspectDescriptor"/>s for a certain join-point.
    /// </summary>
    IList<IAspectDescriptorProvider> AspectDescriptorProviders { get; }

    /// <summary>
    /// A list of <see cref="IAspectOrderingRule"/>s used to sort aspects properly.
    /// </summary>
    IList<IAspectOrderingRule> AspectOrderingRules { get; }

    /// <summary>
    /// A dictionary of aspect types and their corresponding rules.
    /// </summary>
    IDictionary<Type, string> AspectRoles { get; }

    /// <summary>
    /// Gets whether the configuration is locked. This is of remarkable importance when <see cref="IActiveAttributesConfigurator"/>s behave
    /// inconsistently, giving the opportunity to lock the configuration within the .appConfig file.
    /// </summary>
    bool IsLocked { get; }

    /// <summary>
    /// Locks the configuration for further modification.
    /// </summary>
    void Lock ();
  }
}