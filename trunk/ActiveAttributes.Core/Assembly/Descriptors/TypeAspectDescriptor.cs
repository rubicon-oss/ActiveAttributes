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
using System.Collections.ObjectModel;
using System.Reflection;
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Configuration2;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly.Descriptors
{
  /// <summary>
  /// Serves as a <see cref="IAspectDescriptor"/> based on <see cref="System.Type"/> of an aspect.
  /// </summary>
  public class TypeAspectDescriptor : IAspectDescriptor
  {
    public TypeAspectDescriptor (Type aspectType, Scope scope, int priority)
    {
      Type = aspectType;
      Scope = scope;
      Priority = priority;
    }

    public int Priority { get; private set; }

    public Scope Scope { get; private set; }

    public Type Type { get; private set; }

    public ConstructorInfo ConstructorInfo
    {
      get { return Type.GetConstructor (Type.EmptyTypes); }
    }

    public ReadOnlyCollection<object> ConstructorArguments
    {
      get { return new ReadOnlyCollection<object> (new ICustomAttributeNamedArgument[0]); }
    }

    public ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> NamedArguments
    {
      get { return new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (new ICustomAttributeNamedArgument[0]); }
    }

    public bool Matches (MethodInfo method)
    {
      return true;
    }
  }
}