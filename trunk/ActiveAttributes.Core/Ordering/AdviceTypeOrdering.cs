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
using Remotion.Collections;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Ordering
{
  public interface IAdviceTypeOrdering : IAdviceOrdering
  {
    Type BeforeType { get; }
    Type AfterType { get; }
  }

  public class AdviceTypeOrdering : AdviceOrderingBase, IAdviceTypeOrdering
  {
    private readonly Type _beforeType;
    private readonly Type _afterType;

    public AdviceTypeOrdering (Type beforeType, Type afterType, string source)
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

    public override bool Depends (IAdviceDependencyProvider provider, Advice advice1, Advice advice2)
    {
      return provider.DependsType (advice1, advice2, this);
    }
  }

  public class AdviceTypeOrderingAttribute : AdviceOrderingAttributeBase
  {
    private readonly Position _position;
    private readonly Type[] _aspectTypes;

    public AdviceTypeOrderingAttribute (Position position, params Type[] aspectTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("aspectTypes", aspectTypes);

      _position = position;
      _aspectTypes = aspectTypes;
    }

    public Position Position
    {
      get { return _position; }
    }

    public Type[] AspectTypes
    {
      get { return _aspectTypes; }
    }
  }
}