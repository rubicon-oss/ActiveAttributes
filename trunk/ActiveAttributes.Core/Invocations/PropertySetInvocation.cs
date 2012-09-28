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
// 

using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class PropertySetInvocation<TInstance, TValue> : ActionInvocation<TInstance, TValue>, IPropertyInvocation
  {
    public PropertySetInvocation (PropertySetInvocationContext<TInstance, TValue> context, Action<TValue> action)
        : base (context, action)
    {
      Context = context;
    }

    IInvocationContext IInvocation.Context
    {
      get { return Context; }
    }

    public new IPropertyInvocationContext Context { get; private set; }
  }
}