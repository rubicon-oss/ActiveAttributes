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
using System.Collections.ObjectModel;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using Remotion.Collections;
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

    IList<IAspectDependencyProvider> AspectDependencyProviders { get; }

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

  public class ActiveAttributesConfiguration : IActiveAttributesConfiguration
  {
    private readonly IList<IAspectDescriptorProvider> _aspectDescriptorProviders;
    private readonly IList<IAspectDependencyProvider> _aspectDependencyProviders;
    private readonly IList<IAspectOrderingRule> _aspectOrderingRules;
    private readonly IDictionary<Type, string> _aspectRoles;

    private bool _isLocked;

    public ActiveAttributesConfiguration ()
    {
      _aspectDescriptorProviders = new List<IAspectDescriptorProvider>();
      _aspectDependencyProviders = new List<IAspectDependencyProvider>();
      _aspectOrderingRules = new List<IAspectOrderingRule>();
      _aspectRoles = new Dictionary<Type, string>();
    }

    public IList<IAspectDescriptorProvider> AspectDescriptorProviders
    {
      get { return !IsLocked ? _aspectDescriptorProviders : new ReadOnlyCollection<IAspectDescriptorProvider> (_aspectDescriptorProviders); }
    }

    public IList<IAspectDependencyProvider> AspectDependencyProviders
    {
      get { return !IsLocked ? _aspectDependencyProviders : new ReadOnlyCollection<IAspectDependencyProvider> (_aspectDependencyProviders); }
    }

    public IList<IAspectOrderingRule> AspectOrderingRules
    {
      get { return !IsLocked ? _aspectOrderingRules : new ReadOnlyCollection<IAspectOrderingRule> (_aspectOrderingRules); }
    }

    public IDictionary<Type, string> AspectRoles
    {
      get { return !IsLocked ? _aspectRoles : new ReadOnlyDictionary<Type, string> (_aspectRoles); }
    }

    public bool IsLocked
    {
      get { return _isLocked; }
    }

    public void Lock ()
    {
      if (_isLocked)
        throw new InvalidOperationException ("Configuration is already locked.");

      _isLocked = true;
    }
  }
}