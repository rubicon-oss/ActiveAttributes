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
using ActiveAttributes.Infrastructure.Ordering;
using ActiveAttributes.Infrastructure.Pointcuts;
using ActiveAttributes.Weaving.Construction;
using Remotion.Utilities;

namespace ActiveAttributes.Infrastructure
{
  public class Aspect : ICrosscutting
  {
    private readonly Type _type;
    private readonly AspectScope _scope;
    private readonly AspectActivation _activation;
    private readonly IAspectConstruction _construction;
    private readonly ICrosscutting _crosscutting;
    private readonly IEnumerable<Advice> _advices;
    private readonly IEnumerable<MemberImport> _memberImports;
    private readonly IEnumerable<MemberIntroduction> _memberIntroductions;

    public Aspect (
        Type type,
        AspectScope scope,
        AspectActivation activation,
        IAspectConstruction construction,
        ICrosscutting crosscutting,
        IEnumerable<Advice> advices,
        IEnumerable<MemberImport> memberImports,
        IEnumerable<MemberIntroduction> memberIntroductions)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("construction", construction);
      ArgumentUtility.CheckNotNull ("crosscutting", crosscutting);
      ArgumentUtility.CheckNotNull ("advices", advices);
      ArgumentUtility.CheckNotNull ("memberImports", memberImports);
      ArgumentUtility.CheckNotNull ("memberIntroductions", memberIntroductions);

      _type = type;
      _scope = scope;
      _activation = activation;
      _construction = construction;
      _crosscutting = crosscutting;
      _advices = advices;
      _memberImports = memberImports;
      _memberIntroductions = memberIntroductions;
    }

    public Type Type
    {
      get { return _type; }
    }

    public AspectScope Scope
    {
      get { return _scope; }
    }

    public AspectActivation Activation
    {
      get { return _activation; }
    }

    public IAspectConstruction Construction
    {
      get { return _construction; }
    }

    public IPointcut Pointcut
    {
      get { return _crosscutting.Pointcut; }
    }

    public IEnumerable<Advice> Advices
    {
      get { return _advices; }
    }

    public IEnumerable<MemberImport> MemberImports
    {
      get { return _memberImports; }
    }

    public IEnumerable<Type> InterfaceIntroductions
    {
      get { return _type.GetInterfaces(); }
    }

    public IEnumerable<MemberIntroduction> MemberIntroductions
    {
      get { return _memberIntroductions; }
    }

    public string Role
    {
      get { return _crosscutting.Role; }
    }

    public IEnumerable<IOrdering> Orderings
    {
      get { return _crosscutting.Orderings; }
    }

    public int Priority
    {
      get { return _crosscutting.Priority; }
    }

    string ICrosscutting.Name
    {
      get { throw new NotImplementedException(); }
    }
  }
}