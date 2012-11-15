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
using ActiveAttributes.Aspects;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.ServiceLocation;

namespace ActiveAttributes.Discovery.DeclarationProviders
{
  [ConcreteImplementation (typeof (AspectTypesProvider))]
  public interface IAspectTypesProvider
  {
    IEnumerable<Type> GetAspectClassTypes ();
    IEnumerable<Type> GetAspectAttributeTypes ();
  }

  public class AspectTypesProvider : IAspectTypesProvider
  {
    private readonly AssemblyFinderTypeDiscoveryService _assemblyFinderTypeDiscoveryService;

    public AspectTypesProvider ()
    {
      var callingAssembly = System.Reflection.Assembly.GetCallingAssembly ();
      var rootAssembly = new RootAssembly (callingAssembly, true);
      var loadAllAssemblyLoaderFilter = new LoadAllAssemblyLoaderFilter ();
      var filteringAssemblyLoader = new FilteringAssemblyLoader (loadAllAssemblyLoaderFilter);
      var fixedRootAssemblyFinder = new FixedRootAssemblyFinder (rootAssembly);
      var assemblyFinder = new AssemblyFinder (fixedRootAssemblyFinder, filteringAssemblyLoader);
      _assemblyFinderTypeDiscoveryService = new AssemblyFinderTypeDiscoveryService (assemblyFinder);
    }

    public IEnumerable<Type> GetAspectClassTypes ()
    {
      return _assemblyFinderTypeDiscoveryService.GetTypes (typeof (IAspect), false).OfType<Type>().Where (x => x.IsClass);
    }

    public IEnumerable<Type> GetAspectAttributeTypes ()
    {
      return _assemblyFinderTypeDiscoveryService.GetTypes (typeof (AspectAttributeBase), false).OfType<Type>().Where (x => !x.IsAbstract);
    }
  }
}