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
using ActiveAttributes.Aspects;
using Remotion.Utilities;

namespace ActiveAttributes.Discovery.DeclarationProviders
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
      var aspectTypes = _typeDiscoveryService.GetTypes (typeof (IAspect), false).Cast<Type> ();
      var classTypes = aspectTypes.Where (x => x.IsClass).Where (x => !typeof (AspectAttributeBase).IsAssignableFrom (x));

      foreach (var classType in classTypes)
      {
        if (classType.GetConstructor (Type.EmptyTypes) == null)
        {
          var message = string.Format ("Cannot create an object of type '{0}' without parameterless constructor.", classType.Name);
          throw new InvalidOperationException (message);
        }

        foreach (var adviceBuilder in _classDeclarationProvider.GetAdviceBuilders (classType))
          yield return adviceBuilder;
      }
    }
  }
}