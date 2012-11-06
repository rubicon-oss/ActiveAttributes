﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Attributes.Aspects;
using ActiveAttributes.Core.Infrastructure.Construction;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother2
  {
    public static IConstruction GetConstruction (
        ConstructorInfo constructor = null,
        ReadOnlyCollection<object> constructorArguments = null,
        ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> namedArguments = null)
    {
      constructor = constructor ?? GetConstructorInfo ();
      constructorArguments = constructorArguments ?? new object[0].ToList().AsReadOnly();
      namedArguments = namedArguments ?? new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (new ICustomAttributeNamedArgument[0]);

      var stub = MockRepository.GenerateStub<IConstruction>();

      stub.Stub (x => x.ConstructorInfo).Return (constructor);
      stub.Stub (x => x.ConstructorArguments).Return (constructorArguments);
      stub.Stub (x => x.NamedArguments).Return (namedArguments);

      return stub;
    }

    public static IConstruction GetConstructionByType (Type constructionType)
    {
      var constructions =
          new IConstruction[]
          {
              new CustomAttributeDataConstruction (GetCustomAttributeData (typeof (DummyAspectAttribute))),
              new TypeConstruction (typeof (DummyAspectAttribute))
          };
      var aspectConstructionInfo = constructions.SingleOrDefault (x => x.GetType() == constructionType);
      Assertion.IsNotNull (constructionType);
      return aspectConstructionInfo;
    }

    class DummyAspectAttribute : AspectAttributeBase {}
  }
}