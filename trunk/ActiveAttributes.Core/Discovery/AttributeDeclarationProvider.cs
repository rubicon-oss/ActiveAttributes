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
using ActiveAttributes.Annotations;
using ActiveAttributes.Aspects;
using ActiveAttributes.Infrastructure;
using ActiveAttributes.Weaving;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Discovery
{
  [ConcreteImplementation (typeof (AttributeDeclarationProvider))]
  public interface IAttributeDeclarationProvider
  {
    IEnumerable<Aspect> GetDeclaration (System.Reflection.Assembly assembly);
    IEnumerable<Aspect> GetDeclaration (MemberInfo memberInfo);
  }

  public class AttributeDeclarationProvider : IAttributeDeclarationProvider
  {
    private readonly IAspectBuilder _aspectBuilder;

    public AttributeDeclarationProvider (IAspectBuilder aspectBuilder)
    {
      _aspectBuilder = aspectBuilder;
    }

    public IEnumerable<Aspect> GetDeclaration (System.Reflection.Assembly assembly)
    {
      return GetDeclaration (TypePipeCustomAttributeData.GetCustomAttributes (assembly));
    }

    public IEnumerable<Aspect> GetDeclaration (MemberInfo memberInfo)
    {
      return GetDeclaration (TypePipeCustomAttributeData.GetCustomAttributes (memberInfo));
    }

    private IEnumerable<Aspect> GetDeclaration (IEnumerable<ICustomAttributeData> customAttributeData)
    {
      return customAttributeData
          .Where (x => typeof (AspectAttributeBase).IsAssignableFrom (x.Type))
          .Select (_aspectBuilder.Build);
    }
  }
}