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
using ActiveAttributes.Core.Infrastructure.Orderings;
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
    //TODO IList<IAspectDescriptorProvider> AspectDescriptorProviders { get; }

    IList<IAdviceDependencyProvider> AspectDependencyProviders { get; }

    /// <summary>
    /// A list of <see cref="IAdviceOrdering"/>s used to sort aspects properly.
    /// </summary>
    IList<IAdviceOrdering> AspectOrderingRules { get; }

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
    //private readonly IList<IAspectDescriptorProvider> _aspectDescriptorProviders;
    private readonly IList<IAdviceDependencyProvider> _aspectDependencyProviders;
    private readonly IList<IAdviceOrdering> _aspectOrderingRules;
    private readonly IDictionary<Type, string> _aspectRoles;

    private bool _isLocked;

    public ActiveAttributesConfiguration ()
    {
      //_aspectDescriptorProviders = new List<IAspectDescriptorProvider>();
      _aspectDependencyProviders = new List<IAdviceDependencyProvider>();
      _aspectOrderingRules = new List<IAdviceOrdering>();
      _aspectRoles = new Dictionary<Type, string>();
    }

    public IList<IAdviceDependencyProvider> AspectDependencyProviders
    {
      get { return !IsLocked ? _aspectDependencyProviders : new ReadOnlyCollection<IAdviceDependencyProvider> (_aspectDependencyProviders); }
    }

    public IList<IAdviceOrdering> AspectOrderingRules
    {
      get { return !IsLocked ? _aspectOrderingRules : new ReadOnlyCollection<IAdviceOrdering> (_aspectOrderingRules); }
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