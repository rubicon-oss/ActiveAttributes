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
using ActiveAttributes.Core.Assembly;
using Remotion.Collections;
using Remotion.ServiceLocation;

namespace ActiveAttributes.Core.Configuration2
{
  /// <summary>
  /// Serves as a merger for dependencies between <see cref="IAspectDescriptor"/>s.
  /// </summary>
  /// <remarks>
  /// Dependencies are merged by applying only the non-conflicting subset of new to the previous dependencies.
  /// </remarks>
  [ConcreteImplementation (typeof (AspectDependencyMerger))]
  public interface IAspectDependencyMerger
  {
    IEnumerable<Tuple<IAspectDescriptor, IAspectDescriptor>> MergeDependencies (
        IEnumerable<Tuple<IAspectDescriptor, IAspectDescriptor>> previousDependencies,
        IEnumerable<Tuple<IAspectDescriptor, IAspectDescriptor>> newDependencies);
  }
}