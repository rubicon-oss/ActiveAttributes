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
using ActiveAttributes.Core.Assembly.Providers;
using Remotion.Collections;

namespace ActiveAttributes.Core.Assembly.Configuration
{
  public class Configuration : IConfiguration
  {
    static Configuration ()
    {
      Singleton = new Configuration();
    }

    public static IConfiguration Singleton { get; private set; }

    private readonly IList<IDescriptorProvider> _aspectProviders;
    private readonly IList<IOrderRule> _rules;
    private readonly IDictionary<Type, string> _roles;

    private bool _isReadOnly;

    public Configuration ()
    {
      // TODO default providers?
      _aspectProviders = new List<IDescriptorProvider>
                         {
                             new TypeBasedDescriptorProvider(),
                             new MethodBasedDescriptorProvider(),
                             new InterfaceMethodBasedDescriptorProvider(),
                             new PropertyBasedDescriptorProvider(),
                             new EventBasedDescriptorProvider(),
                             new ParameterBasedDescriptorProvider()
                         };
      _rules = new List<IOrderRule>();
      _roles = new Dictionary<Type, string>();
    }

    public IList<IDescriptorProvider> DescriptorProviders
    {
      get { return !IsReadOnly ? _aspectProviders : new ReadOnlyCollection<IDescriptorProvider> (_aspectProviders); }
    }

    public IList<IOrderRule> Rules
    {
      get { return !IsReadOnly ? _rules : new ReadOnlyCollection<IOrderRule> (_rules); }
    }

    public IDictionary<Type, string> Roles
    {
      get { return !IsReadOnly ? _roles : new ReadOnlyDictionary<Type, string> (_roles); }
    }

    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = _isReadOnly || value; }
    }
  }
}