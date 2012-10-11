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
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly.Providers
{
  public class MethodLevelAspectProvider : IMethodLevelAspectProvider
  {
    private readonly IRelatedMethodFinder _methodFinder;

    public MethodLevelAspectProvider ()
    {
      _methodFinder = new RelatedMethodFinder(); // TODO inject
    }

    public IEnumerable<IAspectDescriptor> GetAspects (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      var methodSequence = method.CreateSequence (x => _methodFinder.GetBaseMethod (x)).Cast<MemberInfo>();

      return AspectProvider.GetAspects (method, methodSequence);
    }
  }
}