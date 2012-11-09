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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Discovery.Construction;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Discovery.DeclarationProviders
{
  public class AspectClassDeclarationProvider : IAssemblyLevelDeclarationProvider
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;
    private readonly IClassDeclarationProvider _classDeclarationProvider;

    public AspectClassDeclarationProvider (ITypeDiscoveryService typeDiscoveryService, IClassDeclarationProvider classDeclarationProvider)
    {
      ArgumentUtility.CheckNotNull ("typeDiscoveryService", typeDiscoveryService);
      ArgumentUtility.CheckNotNull ("classDeclarationProvider", classDeclarationProvider);

      _typeDiscoveryService = typeDiscoveryService;
      _classDeclarationProvider = classDeclarationProvider;
    }

    public IEnumerable<IAdviceBuilder> GetDeclarations ()
    {
      return from Type aspectType in _typeDiscoveryService.GetTypes (typeof (IAspect), false)
             let construction = new TypeConstruction (aspectType)
             where !typeof (AspectAttributeBase).IsAssignableFrom (aspectType)
             from adviceBuilder in _classDeclarationProvider.GetAdviceBuilders (aspectType)
             select adviceBuilder.UpdateConstruction (construction);
    }
  }
}