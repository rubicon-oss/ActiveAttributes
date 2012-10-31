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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Configuration2.AspectDependencyProviders
{
  public class PriorityDependencyProvider : IAspectDependencyProvider
  {
    private readonly IActiveAttributesConfiguration _activeAttributesConfiguration;

    public PriorityDependencyProvider (IActiveAttributesConfiguration activeAttributesConfiguration)
    {
      ArgumentUtility.CheckNotNull ("activeAttributesConfiguration", activeAttributesConfiguration);

      _activeAttributesConfiguration = activeAttributesConfiguration;
    }

    public IEnumerable<Tuple<IAspectDescriptor, IAspectDescriptor>> GetDependencies (IEnumerable<IAspectDescriptor> aspectDescriptors)
    {
      ArgumentUtility.CheckNotNull ("aspectDescriptors", aspectDescriptors);

      var aspectDescriptorsAsCollection = aspectDescriptors.ConvertToCollection ();

      var combinations = (from aspectDescriptor1 in aspectDescriptorsAsCollection
                          from aspectDescriptor2 in aspectDescriptorsAsCollection
                          select new
                                 {
                                     Aspect1 = aspectDescriptor1,
                                     Aspect2 = aspectDescriptor2
                                 }).ToList ();

      foreach (var combination in combinations)
      {
        var compared = combination.Aspect1.Priority.CompareTo (combination.Aspect2.Priority);
        if (compared == 1)
          yield return Tuple.Create (combination.Aspect1, combination.Aspect2);
        else if (compared == -1)
          yield return Tuple.Create (combination.Aspect2, combination.Aspect1);
      }
    }
  }
}