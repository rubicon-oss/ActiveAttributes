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
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Configuration;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectExpressionGeneratorFactory
  {
    public IEnumerable<IAspectGenerator> GetAspectGenerators (
        IEnumerable<IAspectDescriptor> aspects, AspectScope scope, FieldInfo fieldInfo)
    {
      return aspects.Select ((x, i) => GetAspectGenerator(x, scope, fieldInfo, i));
    }

    private IAspectGenerator GetAspectGenerator (IAspectDescriptor descriptor, AspectScope scope, FieldInfo fieldInfo, int index)
    {
      switch (scope)
      {
        case AspectScope.Instance: return new InstanceAspectGenerator (fieldInfo, index, descriptor);
        case AspectScope.Static: return new StaticAspectGenerator (fieldInfo, index, descriptor);
        default: throw new ArgumentOutOfRangeException ("scope");
      }
    }
  }
}