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

namespace ActiveAttributes.Advices
{
  /// <summary>
  /// Declares information about advices regarding their name, role, execution, scope, and priority.
  /// Missing informationen will be derived from parent members (i.e., the declaring type or the base method of an advice).
  /// </summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class AdviceInfoAttribute : Attribute
  {
    /// <summary>The name of the advice.</summary>
    public string Name { get; set; }
    /// <summary>The role of the advice.</summary>
    public string Role { get; set; }

    /// <summary>The execution of the advice.</summary>
    public AdviceExecution Execution { get; set; }
    /// <summary>The scope of the advice.</summary>
    public AdviceScope Scope { get; set; }
    /// <summary>The priority of the advice.</summary>
    public int Priority { get; set; }
  }
}