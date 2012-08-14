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
using ActiveAttributes.Core.Assembly;

namespace ActiveAttributes.Core.Configuration.Rules
{
  public class TypeOrderRule : IComparer<IAspectDescriptor>
  {
    private readonly Type _type1;
    private readonly Type _type2;

    public TypeOrderRule (Type type1, Type type2)
    {
      _type1 = type1;
      _type2 = type2;
    }

    public int Compare (IAspectDescriptor x, IAspectDescriptor y)
    {
      var type1 = x.AspectType;
      var type2 = y.AspectType;

      if (_type1.IsAssignableFrom (type1) && _type2.IsAssignableFrom (type2))
        return -1;
      if (_type1.IsAssignableFrom (type2) && _type2.IsAssignableFrom (type1))
        return 1;

      return 0;
    }
  }
}