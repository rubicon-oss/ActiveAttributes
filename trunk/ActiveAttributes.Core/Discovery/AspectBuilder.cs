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
using ActiveAttributes.Annotations;
using ActiveAttributes.Aspects;
using ActiveAttributes.Extensions;
using ActiveAttributes.Infrastructure;
using ActiveAttributes.Infrastructure.Pointcuts;
using ActiveAttributes.Weaving.Construction;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Discovery
{
  [ConcreteImplementation (typeof (AspectBuilder))]
  public interface IAspectBuilder
  {
    Aspect Build (Type type);
    Aspect Build (ICustomAttributeData attributeData);
  }

  public class AspectBuilder : IAspectBuilder
  {
    private readonly IAspectElementBuilder _aspectElementBuilder;
    private readonly IPointcutBuilder _pointcutBuilder;

    public AspectBuilder (IAspectElementBuilder aspectElementBuilder, IPointcutBuilder pointcutBuilder)
    {
      _aspectElementBuilder = aspectElementBuilder;
      _pointcutBuilder = pointcutBuilder;
    }

    public Aspect Build (Type type)
    {
      var aspectAttribute = type.GetCustomAttributes<AspectAttribute> (true).SingleOrDefault();
      Assertion.IsNotNull (aspectAttribute);

      var construction = new TypeConstruction (type);
      var pointcut = _pointcutBuilder.GetPointcut (type);

      return Build (type, aspectAttribute, construction, pointcut);
    }

    public Aspect Build (ICustomAttributeData attributeData)
    {
      var attribute = attributeData.CreateAttribute<AspectAttributeBase>();
      // throw if no aspectattributebase

      var construction = new AttributeConstruction (attributeData);
      var pointcut = _pointcutBuilder.GetPointcut (attributeData);

      return Build (attributeData.Type, attribute, construction, pointcut);
    }

    private Aspect Build (Type type, IAspectInfo info, IAspectConstruction construction, IPointcut pointcut)
    {
      var scope = info.Scope;
      var activation = info.Activation;
      var role = info.Role;

      var advices = new List<Advice>();
      var introductions = new List<MemberIntroduction>();
      var imports = new List<MemberImport>();

      var aspect = new Aspect (type, scope, activation, role, construction, pointcut, advices, imports, introductions);

      _aspectElementBuilder.AddAdvices (aspect, advices);
      _aspectElementBuilder.AddMemberImports (aspect, imports);
      _aspectElementBuilder.AddMemberIntroductions (aspect, introductions);

      return aspect;
    }
  }
}
