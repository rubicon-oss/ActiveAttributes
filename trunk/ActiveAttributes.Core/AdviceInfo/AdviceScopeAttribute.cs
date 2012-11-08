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

namespace ActiveAttributes.Core.AdviceInfo
{
  /// <summary>
  /// Defines the scope of an <see cref="AspectAttribute"/>.
  /// </summary>
  public enum AdviceScope
  {
    Undefined,

    /// <summary>
    /// The aspect is created once per attribute.
    /// </summary>
    Static,

    /// <summary>
    /// The aspect is created for each new target object.
    /// </summary>
    Instance
  }

  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class AdviceScopeAttribute : AdviceAttribute
  {
    private readonly AdviceScope _scope;

    public AdviceScopeAttribute (AdviceScope scope)
    {
      _scope = scope;
    }

    public AdviceScope Scope
    {
      get { return _scope; }
    }
  }
}