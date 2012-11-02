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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother2
  {
    public static MethodInfo GetMethodInfo (
        string name = null,
        Type returnType = null,
        IEnumerable<Type> parameterTypes = null,
        MethodAttributes attributes = MethodAttributes.Private,
        Type declaringType = null)
    {
      name = name ?? "Method";
      returnType = returnType ?? GetType_();
      parameterTypes = parameterTypes ?? GetMultiple (GetDeclaringType);
      declaringType = declaringType ?? GetDeclaringType();

      var parameters = parameterTypes.Select ((x, i) => GetParameterInfo (x, "para" + i)).ToArray();

      var stub = MockRepository.GenerateStub<MethodInfo>();

      stub.Stub (x => x.Name).Return (name);
      stub.Stub (x => x.ReturnType).Return (returnType);
      stub.Stub (x => x.GetParameters()).Return (parameters);
      stub.Stub (x => x.Attributes).Return (attributes);
      stub.Stub (x => x.DeclaringType).Return (declaringType);

      return stub;
    }
  }
}