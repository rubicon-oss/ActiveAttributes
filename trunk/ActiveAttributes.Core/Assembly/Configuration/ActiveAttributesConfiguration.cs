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
using ActiveAttributes.Core.Checked;
using ActiveAttributes.Core.Configuration2;
using Remotion.Collections;

namespace ActiveAttributes.Core.Assembly.Configuration
{
  // TODO test
  public class ActiveAttributesConfiguration : IActiveAttributesConfiguration
  {
    public static IActiveAttributesConfiguration Singleton { get; private set; }

    private readonly IList<IAspectDescriptorProvider> _aspectDescriptorProviders;
    private readonly IList<IAspectOrderingRule> _rules;
    private readonly IDictionary<Type, string> _roles;

    private bool _isLocked;

    static ActiveAttributesConfiguration ()
    {
      Singleton = new ActiveAttributesConfiguration();
    }

    public ActiveAttributesConfiguration ()
    {
      _aspectDescriptorProviders = new List<IAspectDescriptorProvider>();
      _rules = new List<IAspectOrderingRule>();
      _roles = new Dictionary<Type, string>();
    }

    public IList<IAspectDescriptorProvider> AspectDescriptorProviders
    {
      get { return !IsLocked ? _aspectDescriptorProviders : new ReadOnlyCollection<IAspectDescriptorProvider> (_aspectDescriptorProviders); }
    }

    public IList<IAspectOrderingRule> AspectOrderingRules
    {
      get { return !IsLocked ? _rules : new ReadOnlyCollection<IAspectOrderingRule> (_rules); }
    }

    public IDictionary<Type, string> AspectRoles
    {
      get { return !IsLocked ? _roles : new ReadOnlyDictionary<Type, string> (_roles); }
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

  //public class ConfigProvider : IConfigProvider
  //{
  //  private IEnumerable _configurators;

  //  public ConfigProvider (IEnumerable<IConfigurator> configurators)
  //  {
  //    _configurators = configurators;
  //  }

  //  public Configuration GetConfiguration () 
  //  {
  //    var config = new Configuration ();
  //    foreach (var configurator in _configurators)
  //      configurator.Initialize (config);
  //    return config;
  //  }
  //}
}