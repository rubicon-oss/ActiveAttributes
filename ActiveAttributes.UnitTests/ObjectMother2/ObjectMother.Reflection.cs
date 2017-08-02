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
using Remotion.Utilities;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother
  {
    private static readonly Type[] s_types = new[] { typeof (object), typeof (int), typeof (void), typeof (Type) };

    public static Type GetType_ ()
    {
      return GetRandom (s_types);
    }

    public static Type GetDeclaringType ()
    {
      var types = s_types.Where (x => x != typeof (void)).ToList();
      return GetRandom (types);
    }

    public static FieldInfo GetFieldInfo (
        Type type = null, string name = null, FieldAttributes attributes = FieldAttributes.Private, Type declaringType = null)
    {
      name = name ?? "_field";
      type = type ?? GetDeclaringType();
      declaringType = declaringType ?? GetDeclaringType();

      var stub = MockRepository.GeneratePartialMock<FieldInfo>();

      stub.Stub (x => x.Name).Return (name);
      stub.Stub (x => x.FieldType).Return (type);
      stub.Stub (x => x.Attributes).Return (attributes);
      stub.Stub (x => x.DeclaringType).Return (declaringType);

      return stub;
    }

    public static ConstructorInfo GetConstructorInfo (
        Type[] parameterTypes = null, Type declaringType = null, MethodAttributes attributes = MethodAttributes.Private)
    {
      Assertion.IsNull (parameterTypes);
      //parameterTypes = parameterTypes ?? GetMultiple (GetDeclaringType).ToArray();
      declaringType = declaringType ?? GetDeclaringType();

      var stub = MockRepository.GenerateStub<ConstructorInfo>();

      stub.Stub (x => x.DeclaringType).Return (declaringType);
      stub.Stub (x => x.Attributes).Return (attributes);

      return stub;
    }

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

    public static PropertyInfo GetPropertyInfo (
        Type type = null, string name = null, PropertyAttributes attributes = PropertyAttributes.None, Type declaringType = null)
    {
      name = name ?? "Property";
      type = type ?? GetDeclaringType();
      declaringType = declaringType ?? GetDeclaringType();

      var stub = MockRepository.GenerateStub<PropertyInfo>();

      stub.Stub (x => x.Name).Return (name);
      stub.Stub (x => x.PropertyType).Return (type);
      stub.Stub (x => x.Attributes).Return (attributes);
      stub.Stub (x => x.DeclaringType).Return (declaringType);

      return stub;
    }

    public static ParameterInfo GetParameterInfo (Type type = null, string name = null)
    {
      type = type ?? GetDeclaringType();
      name = name ?? "parameter";

      var stub = MockRepository.GenerateStub<ParameterInfo>();

      stub.Stub (x => x.ParameterType).Return (type);
      stub.Stub (x => x.Name).Return (name);

      return stub;
    }
  }
}