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
using ActiveAttributes.Advices;
using ActiveAttributes.Utilities;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Ordering
{
  /// <summary>
  /// Sorts a collection of items (i.e., mostly <see cref="IAspectDescriptor"/>s) according to the ordering rules defined in
  /// the <see cref="IActiveAttributesConfiguration"/>.
  /// </summary>
  [ConcreteImplementation (typeof (AdviceSequencer))]
  public interface IAdviceSequencer
  {
    IEnumerable<Advice> Sort (IEnumerable<Advice> advices);
  }

  public class AdviceSequencer : IAdviceSequencer
  {
    private readonly IAdviceDependencyProvider _adviceDependencyProvider;

    public AdviceSequencer (IAdviceDependencyProvider adviceDependencyProvider)
    {
      ArgumentUtility.CheckNotNull ("adviceDependencyProvider", adviceDependencyProvider);

      _adviceDependencyProvider = adviceDependencyProvider;
    }

    public IEnumerable<Advice> Sort (IEnumerable<Advice> advices)
    {
      var advicesAsList = advices.ToList();
      ArgumentUtility.CheckNotNull ("advices", advicesAsList);

      var dependencies = _adviceDependencyProvider.GetDependencies (advicesAsList);
      return advicesAsList.TopologicalSort (dependencies, throwIfOrderIsUndefined: true);
    }
  }
}