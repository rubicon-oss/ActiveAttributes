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
using ActiveAttributes.Aspects;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Declaration.Construction
{
  public class AttributeConstruction : IConstruction
  {
    private readonly ConstructorInfo _constructorInfo;
    private readonly ReadOnlyCollection<object> _constructorArguments;
    private readonly ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> _namedArguments;

    public AttributeConstruction (ICustomAttributeData customAttributeData)
    {
      var constructorInfo = customAttributeData.Constructor;
      Assertion.IsTrue (typeof (AspectAttributeBase).IsAssignableFrom (constructorInfo.DeclaringType));

      _constructorInfo = constructorInfo;
      _constructorArguments = customAttributeData.ConstructorArguments;
      _namedArguments = customAttributeData.NamedArguments;
    }

    public ConstructorInfo ConstructorInfo
    {
      get { return _constructorInfo; }
    }

    public ReadOnlyCollection<object> ConstructorArguments
    {
      get { return _constructorArguments; }
    }

    public ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> NamedArguments
    {
      get { return _namedArguments; }
    }
  }
}