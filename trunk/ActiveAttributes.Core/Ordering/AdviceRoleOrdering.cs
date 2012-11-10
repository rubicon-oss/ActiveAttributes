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
using ActiveAttributes.Core.Discovery;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Ordering
{
  public interface IAdviceRoleOrdering : IAdviceOrdering
  {
    string BeforeRole { get; }
    string AfterRole { get; }
  }

  public class AdviceRoleOrdering : AdviceOrderingBase, IAdviceRoleOrdering
  {
    private readonly string _beforeRole;
    private readonly string _afterRole;

    public AdviceRoleOrdering (string beforeRole, string afterRole, string source)
        : base (ArgumentUtility.CheckNotNullOrEmpty ("source", source))
    {
      ArgumentUtility.CheckNotNullOrEmpty ("beforeRole", beforeRole);
      ArgumentUtility.CheckNotNullOrEmpty ("afterRole", afterRole);

      _beforeRole = beforeRole;
      _afterRole = afterRole;
    }

    public string BeforeRole
    {
      get { return _beforeRole; }
    }

    public string AfterRole
    {
      get { return _afterRole; }
    }

    public override bool DependVisit (IAdviceDependencyProvider provider, Advice advice1, Advice advice2)
    {
      return provider.DependsRole (advice1, advice2, this);
    }
  }

  public class AdviceRoleOrderingAttribute : AdviceOrderingAttributeBase
  {
    private readonly Position _position;
    private readonly string[] _aspectRoles;

    public AdviceRoleOrderingAttribute (Position position, params string[] aspectRoles)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("aspectRoles", aspectRoles);

      _position = position;
      _aspectRoles = aspectRoles;
    }

    public Position Position
    {
      get { return _position; }
    }

    public string[] AspectRoles
    {
      get { return _aspectRoles; }
    }
  }
}