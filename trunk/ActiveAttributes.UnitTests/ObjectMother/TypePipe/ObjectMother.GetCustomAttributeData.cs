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
using System.Linq;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother2
  {
    public static ICustomAttributeData GetCustomAttributeData (Type declaringType = null, ICustomAttributeNamedArgument[] namedArguments = null)
    {
      var constructorInfo = GetConstructorInfo (declaringType: declaringType);
      var constructorArguments = new object[0].ToList().AsReadOnly();
      var namedArguments2 = new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (namedArguments ?? new ICustomAttributeNamedArgument[0]);

      var stub = MockRepository.GenerateStub<ICustomAttributeData>();

      stub.Stub (x => x.Constructor).Return (constructorInfo);
      stub.Stub (x => x.ConstructorArguments).Return (constructorArguments);
      stub.Stub (x => x.NamedArguments).Return (namedArguments2);

      return stub;
    }
  }
}