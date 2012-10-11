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
using ActiveAttributes.Core.Assembly.Configuration;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectExpressionGeneratorFactory
  {
    public IEnumerable<IAspectGenerator> GetGenerators (
        IArrayAccessor instanceAccessor, IArrayAccessor staticAccessor, IEnumerable<IAspectDescriptor> aspects)
    {
      ArgumentUtility.CheckNotNull ("instanceAccessor", instanceAccessor);
      ArgumentUtility.CheckNotNull ("staticAccessor", staticAccessor);
      ArgumentUtility.CheckNotNull ("aspects", aspects);
      Assertion.IsTrue (!instanceAccessor.IsStatic);
      Assertion.IsTrue (staticAccessor.IsStatic);

      var aspectsAsCollection = aspects.ConvertToCollection();
      var instanceGenerators = GetGenerators (instanceAccessor, aspectsAsCollection, AspectScope.Instance);
      var staticGenerators = GetGenerators (staticAccessor, aspectsAsCollection, AspectScope.Static);

      return instanceGenerators.Concat (staticGenerators);
    }

    private IEnumerable<IAspectGenerator> GetGenerators (IArrayAccessor accessor, IEnumerable<IAspectDescriptor> aspects, AspectScope aspectScope)
    {
      return aspects.Where (x => x.Scope == aspectScope).Select ((x, i) => new AspectGenerator (accessor, i, x)).Cast<IAspectGenerator>();
    }
  }
}