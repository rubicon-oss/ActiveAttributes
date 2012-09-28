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
using System.Reflection;
using ActiveAttributes.Core.Assembly.Configuration;

namespace ActiveAttributes.Core.Assembly.Descriptors
{
  /// <summary>
  /// An <see cref="IAspectDescriptor"/> that is based on the <see cref="Type"/> of an aspect.
  /// </summary>
  public class TypeDescriptor : IAspectDescriptor
  {
    public TypeDescriptor (Type aspectType, AspectScope scope, int priority)
    {
      AspectType = aspectType;
      Scope = scope;
      Priority = priority;
    }

    public int Priority { get; private set; }

    public AspectScope Scope { get; private set; }

    public Type AspectType { get; private set; }

    public ConstructorInfo ConstructorInfo
    {
      get { return AspectType.GetConstructor (Type.EmptyTypes); }
    }

    public IList<CustomAttributeTypedArgument> ConstructorArguments
    {
      get { return new CustomAttributeTypedArgument[0]; }
    }

    public IList<CustomAttributeNamedArgument> NamedArguments
    {
      get { return new CustomAttributeNamedArgument[0]; }
    }

    public bool Matches (MethodInfo method)
    {
      return true;
    }
  }
}