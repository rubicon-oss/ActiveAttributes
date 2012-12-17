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
using System.Reflection;
using ActiveAttributes.Model;
using Remotion.Utilities;

namespace ActiveAttributes.Discovery
{
  public class CompositeDeclarationProvider : IDeclarationProvider
  {
    private readonly IEnumerable<IAssemblyLevelDeclarationProvider> _assemblyLevelAdviceDeclarationProviders;
    private readonly IEnumerable<ITypeLevelDeclarationProvider> _typeLevelAdviceDeclarationProviders;
    private readonly IEnumerable<IMethodLevelDeclarationProvider> _methodLevelAdviceDeclarationProviders;

    public CompositeDeclarationProvider (
        IEnumerable<IAssemblyLevelDeclarationProvider> assemblyLevelAdviceDeclarationProviders,
        IEnumerable<ITypeLevelDeclarationProvider> typeLevelAdviceDeclarationProviders,
        IEnumerable<IMethodLevelDeclarationProvider> methodLevelAdviceDeclarationProviders)
    {
      ArgumentUtility.CheckNotNull ("assemblyLevelAdviceDeclarationProviders", assemblyLevelAdviceDeclarationProviders);
      ArgumentUtility.CheckNotNull ("typeLevelAdviceDeclarationProviders", typeLevelAdviceDeclarationProviders);
      ArgumentUtility.CheckNotNull ("methodLevelAdviceDeclarationProviders", methodLevelAdviceDeclarationProviders);

      _assemblyLevelAdviceDeclarationProviders = assemblyLevelAdviceDeclarationProviders;
      _typeLevelAdviceDeclarationProviders = typeLevelAdviceDeclarationProviders;
      _methodLevelAdviceDeclarationProviders = methodLevelAdviceDeclarationProviders;
    }

    public IEnumerable<Aspect> GetDeclarations ()
    {
      return _assemblyLevelAdviceDeclarationProviders.SelectMany (x => x.GetDeclarations());
    }

    public IEnumerable<Aspect> GetDeclarations (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _typeLevelAdviceDeclarationProviders.SelectMany (x => x.GetDeclarations (type));
    }

    public IEnumerable<Aspect> GetDeclarations (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      return _methodLevelAdviceDeclarationProviders.SelectMany (x => x.GetDeclarations (method));
    }
  }
}