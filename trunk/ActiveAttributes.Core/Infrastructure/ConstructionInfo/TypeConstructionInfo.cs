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
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using System.Linq;

namespace ActiveAttributes.Core.Infrastructure.ConstructionInfo
{
  public class TypeConstructionInfo : IConstructionInfo
  {
    private readonly ConstructorInfo _constructorInfo;
    private readonly ReadOnlyCollection<object> _constructorArguments;
    private readonly ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> _namedArguments;

    public TypeConstructionInfo (Type aspectType)
    {
      if (!typeof (IAspect).IsAssignableFrom (aspectType))
        throw new Exception ("TODO (low) exception text");

      _constructorInfo = aspectType.GetConstructor (Type.EmptyTypes);

      if (_constructorInfo == null)
        throw new Exception ("TODO (low) exception text");

      _constructorArguments = new object[0].ToList().AsReadOnly();
      _namedArguments = new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (new ICustomAttributeNamedArgument[0]);
    }

    public ConstructorInfo ConstructorInfo
    {
      get { return _constructorInfo; }
    }

    public ReadOnlyCollection<object> ConstructorArguments
    {
      get { return new ReadOnlyCollection<object> (new object[0]); }
    }

    public ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> NamedArguments
    {
      get { return _namedArguments; }
    }
  }
}