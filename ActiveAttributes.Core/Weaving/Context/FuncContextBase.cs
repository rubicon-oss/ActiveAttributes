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
using System.Reflection;
using ActiveAttributes.Aspects;
using ActiveAttributes.Weaving.Invocation;
using Remotion.Utilities;

namespace ActiveAttributes.Weaving.Context
{
  public abstract class FuncContextBase<TInstance, TReturn> : ArgumentCollectionBase, IContext
  {
    private readonly MemberInfo _memberInfo;

    public readonly TInstance TypedInstance;
    public TReturn TypedReturnValue;

    protected FuncContextBase (MemberInfo memberInfo, TInstance instance)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);
      ArgumentUtility.CheckNotNull ("instance", instance);

      _memberInfo = memberInfo;
      TypedInstance = instance;
    }

    public MemberInfo MemberInfo
    {
      get { return _memberInfo; }
    }

    public object Instance
    {
      get { return TypedInstance; }
    }

    public IArgumentCollection Arguments
    {
      get { return this; }
    }

    public object ReturnValue
    {
      get { return TypedReturnValue; }
      set { TypedReturnValue = (TReturn) value; }
    }

    IReadOnlyArgumentCollection IReadOnlyContext.Arguments
    {
      get { return this; }
    }

    object IReadOnlyContext.ReturnValue
    {
      get { return TypedReturnValue; }
    }
  }
}