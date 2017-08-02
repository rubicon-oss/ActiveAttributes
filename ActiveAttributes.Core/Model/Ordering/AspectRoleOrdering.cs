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
using ActiveAttributes.Weaving;
using Remotion.Utilities;

namespace ActiveAttributes.Model.Ordering
{
  public class AspectRoleOrdering : OrderingBase
  {
    private readonly string _beforeRole;
    private readonly string _afterRole;

    public AspectRoleOrdering (string beforeRole, string afterRole, string source)
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

    public override bool Accept (CrosscuttingDependencyProvider dependencyProvider, ICrosscutting crosscutting1, ICrosscutting crosscutting2)
    {
      return dependencyProvider.VisitAspectRole (this, crosscutting1, crosscutting2);
    }
  }
}