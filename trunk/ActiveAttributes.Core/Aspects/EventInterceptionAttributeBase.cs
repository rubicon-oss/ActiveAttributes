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
using ActiveAttributes.Aspects.Ordering;
using ActiveAttributes.Aspects.Pointcuts;
using ActiveAttributes.Model.Ordering;
using ActiveAttributes.Weaving.Invocation;

namespace ActiveAttributes.Aspects
{
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Event, Inherited = true, AllowMultiple = true)]
  public abstract class EventInterceptionAttributeBase : AspectAttributeBase
  {
    protected EventInterceptionAttributeBase (AspectScope scope, string role = StandardRoles.Unspecified)
        : base (scope, role) { }

    [Advice (AdviceExecution.Around)]
    [MethodExecutionPointcut (Name = "invoke_*")]
    public abstract void OnInvoke (IInvocation invocation);

    [Advice (AdviceExecution.Around)]
    [MethodExecutionPointcut (Name = "add_*")]
    public abstract void OnAdd (IInvocation invocation);

    [Advice (AdviceExecution.Around)]
    [MethodExecutionPointcut (Name = "remove_*")]
    public abstract void OnRemove (IInvocation invocation);
  }
}