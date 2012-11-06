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
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.AdviceDeclarationProviders;
using ActiveAttributes.Core.Infrastructure.Orderings;
using Remotion.ServiceLocation;

namespace ActiveAttributes.Core.Configuration2
{
  /// <summary>
  /// Serves as a configuration object for all concerns of <see cref="AspectAttribute"/>s.
  /// </summary>
  [ConcreteImplementation (typeof (ActiveAttributesConfiguration))]
  public interface IActiveAttributesConfiguration
  {
    IList<IAdviceDeclarationProvider> AspectDeclarationProviders { get; }

    IList<AdviceOrderingBase> AdviceOrderings { get; }

    bool IsLocked { get; }

    void Lock ();
  }

  public class ActiveAttributesConfiguration : IActiveAttributesConfiguration
  {
    private readonly IList<IAdviceDeclarationProvider> _aspectDeclarationProviders;
    private readonly IList<AdviceOrderingBase> _adviceOrderings;

    private bool _isLocked;

    public ActiveAttributesConfiguration ()
    {
      _aspectDeclarationProviders = new List<IAdviceDeclarationProvider>();
      _adviceOrderings = new List<AdviceOrderingBase>();
    }

    public IList<IAdviceDeclarationProvider> AspectDeclarationProviders
    {
      get { return !IsLocked ? _aspectDeclarationProviders : new ReadOnlyCollection<IAdviceDeclarationProvider> (_aspectDeclarationProviders); }
    }

    public IList<AdviceOrderingBase> AdviceOrderings
    {
      get { return !IsLocked ? _adviceOrderings : new ReadOnlyCollection<AdviceOrderingBase> (_adviceOrderings); }
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