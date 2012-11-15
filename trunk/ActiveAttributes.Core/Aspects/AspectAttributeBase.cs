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
using ActiveAttributes.Advices;
using ActiveAttributes.Pointcuts;

namespace ActiveAttributes.Aspects
{
  /// <summary>
  /// Serves as a base class for aspect attributes.
  /// </summary>
  /// <remarks>
  /// <see cref="AdviceScope"/> is set to <c>AdviceScope.Static</c>
  /// </remarks>
  [AdviceInfo (Scope = AdviceScope.Static)]
  public abstract class AspectAttributeBase : Attribute
  {
    #region AdviceInfo

    /// <summary>Defines the advice scope.</summary>
    public AdviceScope AdviceScope { get; set; }

    /// <summary>Defines the advice execution.</summary>
    public AdviceExecution AdviceExecution { get; set; }

    /// <summary>Defines the advice priority.</summary>
    public int AdvicePriority { get; set; }

    #endregion

    #region Pointcuts

    /// <summary>Defines a <see cref="NamespacePointcut"/>.</summary>
    public string ApplyToNamespace { get; set; }

    /// <summary>Defines a <see cref="TypeNamePointcut"/>.</summary>
    public string ApplyToTypeName { get; set; }

    /// <summary>Defines a <see cref="TypePointcut"/>.</summary>
    public Type ApplyToType { get; set; }

    /// <summary>Defines a <see cref="MemberNamePointcut"/>.</summary>
    public string MemberNameFilter { get; set; }

    /// <summary>Defines a <see cref="ReturnTypePointcut"/>.</summary>
    public Type MemberReturnTypeFilter { get; set; }

    /// <summary>Defines a <see cref="ArgumentTypePointcut"/>.</summary>
    public Type[] MemberArgumentsFilter { get; set; }

    /// <summary>Defines a <see cref="VisibilityPointcut"/>.</summary>
    public Visibility MemberVisibilityFilter { get; set; }

    /// <summary>Defines a <see cref="CustomAttributePointcut"/>.</summary>
    public Type MemberCustomAttributeFilter { get; set; }

    /// <summary>Defines a <see cref="ControlFlowPointcut"/>.</summary>
    public string ControlFlow { get; set; }

    #endregion
    }
}