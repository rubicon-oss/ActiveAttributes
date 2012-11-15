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
using System.Linq;
using ActiveAttributes.Extensions;
using Remotion.Utilities;

namespace ActiveAttributes.Discovery.DeclarationProviders
{
  public class AssemblyAttributeDeclarationProvider : IAssemblyLevelDeclarationProvider
  {
    private readonly IAspectTypesProvider _aspectTypesProvider;
    private readonly IAttributeDeclarationProvider _attributeDeclarationProvider;

    public AssemblyAttributeDeclarationProvider (IAspectTypesProvider aspectTypesProvider, IAttributeDeclarationProvider attributeDeclarationProvider)
    {
      ArgumentUtility.CheckNotNull ("aspectTypesProvider", aspectTypesProvider);
      ArgumentUtility.CheckNotNull ("attributeDeclarationProvider", attributeDeclarationProvider);

      _aspectTypesProvider = aspectTypesProvider;
      _attributeDeclarationProvider = attributeDeclarationProvider;
    }

    public IEnumerable<IAdviceBuilder> GetDeclarations ()
    {
      // TODO get all assemblies?
      var types = _aspectTypesProvider.GetAspectAttributeTypes();
      var assemblies = types.Distinct (x => x.Assembly);
      var declarations = assemblies.Select (x => _attributeDeclarationProvider.GetAdviceBuilders (x));
      return declarations.SelectMany (x => x);
    }
  }
}