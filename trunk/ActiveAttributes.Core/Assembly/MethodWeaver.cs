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
using System.Reflection;
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Infrastructure;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  public interface IAspectAttributeDiscovery
  {
    IEnumerable<AspectDeclaration> GetDeclaredByAttributes (ICustomAttributeProvider customAttributeProvider);
    IEnumerable<AspectDeclaration> GetDeclaredByTypes (params System.Reflection.Assembly[] assemblies);
  }

  public class TypeWeaver : IActiveAttributesTypeWeaver
  {
    public TypeWeaver (IConstructorInitializationService constructorInitializationService)
    {
      
    }

    public void ModifyType (MutableType mutableType)
    {

    }
  }

  public class MethodWeaver
  {
    private readonly IAdviceSequencer _adviceSequencer;
    private readonly IConstructorInitializationService _constructorInitializationService;
    private readonly IMethodInterceptionService _methodInterceptionService;

    public MethodWeaver (IAdviceSequencer adviceSequencer, IConstructorInitializationService constructorInitializationService, IMethodInterceptionService methodInterceptionService)
    {
      _adviceSequencer = adviceSequencer;
      _constructorInitializationService = constructorInitializationService;
      _methodInterceptionService = methodInterceptionService;
    }

    public void Weave (MutableMethodInfo method, IEnumerable<Advice> advices)
    {
      //_constructorInitializationService.
    }
  }
}