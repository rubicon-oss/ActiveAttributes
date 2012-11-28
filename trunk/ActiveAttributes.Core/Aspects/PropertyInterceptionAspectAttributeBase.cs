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
using ActiveAttributes.Interception.Invocations;
using ActiveAttributes.Pointcuts;

namespace ActiveAttributes.Aspects
{
  /// <summary>
  /// Serves as a base class for property intercepting aspect attributes.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
  public abstract class PropertyInterceptionAspectAttributeBase : InterceptionAspectAttributeBase
  {
    /// <summary>The method that is invoked instead of the property getter.</summary>
    /// <param name="invocation">The invocation.</param>
    //[MemberNamePointcut ("get_*")]
    [PropertyGetPointcut]
    public abstract void OnInterceptGet (IInvocation invocation);

    /// <summary>The method that is invoked instead of the property setter.</summary>
    /// <param name="invocation">The invocation.</param>
    //[MemberNamePointcut ("set_*")]
    [PropertySetPointcut]
    public abstract void OnInterceptSet (IInvocation invocation);
  }
}