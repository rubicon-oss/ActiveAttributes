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
  public class AspectTypeOrdering : OrderingBase
  {
    private readonly Type _beforeType;
    private readonly Type _afterType;

    public AspectTypeOrdering (Type beforeType, Type afterType, string source)
        : base (ArgumentUtility.CheckNotNullOrEmpty ("source", source))
    {
      ArgumentUtility.CheckNotNull ("beforeType", beforeType);
      ArgumentUtility.CheckNotNull ("afterType", afterType);

      _beforeType = beforeType;
      _afterType = afterType;
    }

    public Type BeforeType
    {
      get { return _beforeType; }
    }

    public Type AfterType
    {
      get { return _afterType; }
    }

    public override bool Accept (CrosscuttingDependencyProvider dependencyProvider, ICrosscutting crosscutting1, ICrosscutting crosscutting2)
    {
      return dependencyProvider.VisitAspectType (this, crosscutting1, crosscutting2);
    }
  }
}