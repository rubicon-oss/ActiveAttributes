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
using System.ComponentModel.Design;
using System.Linq;
using ActiveAttributes.Core.Attributes.Aspects;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.Construction;

namespace ActiveAttributes.Core.Discovery
{
  public interface IStandaloneAspectDeclarationProvider : IAspectDeclarationProvider
  {
    IEnumerable<AspectDeclaration> GetDeclarations ();
  }

  public class StandaloneAspectDeclarationProvider : IStandaloneAspectDeclarationProvider
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;
    private readonly IStandaloneAdviceProvider _standaloneAdviceProvider;

    public StandaloneAspectDeclarationProvider (ITypeDiscoveryService typeDiscoveryService, IStandaloneAdviceProvider standaloneAdviceProvider)
    {
      _typeDiscoveryService = typeDiscoveryService;
      _standaloneAdviceProvider = standaloneAdviceProvider;
    }

    public IEnumerable<AspectDeclaration> GetDeclarations ()
    {
      return from Type aspectType in _typeDiscoveryService.GetTypes (typeof (IAspect), false)
             where !typeof (AspectAttributeBase).IsAssignableFrom (aspectType)
             let advices = _standaloneAdviceProvider.GetAdvices (aspectType)
             select new AspectDeclaration (null, new TypeAspectConstructionInfo (aspectType), advices);
    }
  }
}