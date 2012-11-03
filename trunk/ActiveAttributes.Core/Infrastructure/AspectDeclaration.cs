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
using ActiveAttributes.Core.Infrastructure.Construction;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Infrastructure
{
  public class AspectDeclaration
  {
    private readonly Type _targetType;
    private readonly IAspectConstructionInfo _constructionInfo;
    private readonly IEnumerable<Advice> _advices;

    public AspectDeclaration (Type targetType, IAspectConstructionInfo constructionInfo, IEnumerable<Advice> advices)
    {
      ArgumentUtility.CheckNotNull ("constructionInfo", constructionInfo);
      ArgumentUtility.CheckNotNull ("advices", advices);

      _targetType = targetType;
      _constructionInfo = constructionInfo;
      _advices = advices.ToList().AsReadOnly();
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public IAspectConstructionInfo ConstructionInfo
    {
      get { return _constructionInfo; }
    }

    public IEnumerable<Advice> Advices
    {
      get { return _advices; }
    }
  }
}