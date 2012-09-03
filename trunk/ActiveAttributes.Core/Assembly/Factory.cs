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
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class Factory : IFactory
  {
    public ITypeProvider GetTypeProvider (MethodInfo methodInfo)
    {
      return new TypeProvider (methodInfo);
    }

    public IArrayAccessor GetInstanceAccessor (FieldInfo fieldInfo)
    {
      Assertion.IsFalse (fieldInfo.IsStatic);

      return new InstanceArrayAccessor (fieldInfo);
    }

    public IArrayAccessor GetStaticAccessor (FieldInfo fieldInfo)
    {
      Assertion.IsTrue (fieldInfo.IsStatic);

      return new StaticArrayAccessor (fieldInfo);
    }

    public MethodPatcher GetMethodPatcher (
        MutableMethodInfo mutableMethod,
        FieldInfo propertyInfoFieldInfo,
        FieldInfo eventInfoFieldInfo,
        FieldInfo methodInfoFieldInfo,
        FieldInfo delegateFieldInfo,
        IEnumerable<IAspectGenerator> aspects,
        ITypeProvider typeProvider)
    {
      return new MethodPatcher (
          mutableMethod, propertyInfoFieldInfo, eventInfoFieldInfo, methodInfoFieldInfo, delegateFieldInfo, aspects, typeProvider);
    }

    public IAspectGenerator GetGenerator (IArrayAccessor arrayAccessor, int index, IAspectDescriptor descriptor)
    {
      return new AspectGenerator (arrayAccessor, index, descriptor);
    }
  }
}