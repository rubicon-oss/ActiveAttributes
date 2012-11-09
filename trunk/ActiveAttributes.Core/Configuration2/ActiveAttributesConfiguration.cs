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
using ActiveAttributes.Core.Discovery.DeclarationProviders;
using ActiveAttributes.Core.Ordering;
using Remotion.ServiceLocation;

namespace ActiveAttributes.Core.Configuration2
{
  /// <summary>
  /// Serves as a configuration object for all concerns of <see cref="AspectAttribute"/>s.
  /// </summary>
  [ConcreteImplementation (typeof (ActiveAttributesConfiguration))]
  public interface IActiveAttributesConfiguration
  {
    IList<IDeclarationProvider> DeclarationProviders { get; }

    IList<AdviceOrderingBase> Orderings { get; }

    bool IsLocked { get; }

    void Lock ();
  }

  public class ActiveAttributesConfiguration : IActiveAttributesConfiguration
  {
    private readonly IList<IDeclarationProvider> _declarationProviders = new List<IDeclarationProvider>();
    private readonly IList<AdviceOrderingBase> _orderings = new List<AdviceOrderingBase> ();

    private bool _isLocked;

    public IList<IDeclarationProvider> DeclarationProviders
    {
      get { return !IsLocked ? _declarationProviders : new ReadOnlyCollection<IDeclarationProvider> (_declarationProviders); }
    }

    public IList<AdviceOrderingBase> Orderings
    {
      get { return !IsLocked ? _orderings : new ReadOnlyCollection<AdviceOrderingBase> (_orderings); }
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