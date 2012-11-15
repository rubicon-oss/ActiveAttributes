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
  [AdviceInfo (Scope = AdviceScope.Static)]
  public abstract class AspectAttributeBase : Attribute
  {
    #region AdviceInfo

    public AdviceScope AdviceScope { get; set; }

    public AdviceExecution AdviceExecution { get; set; }

    public int AdvicePriority { get; set; }

    #endregion

    #region Pointcuts

    public string ApplyToNamespace { get; set; }

    public string ApplyToTypeName { get; set; }

    public Type ApplyToType { get; set; }

    public string MemberNameFilter { get; set; }

    public Type MemberReturnTypeFilter { get; set; }

    public Type[] MemberArgumentsFilter { get; set; }

    public Visibility MemberVisibilityFilter { get; set; }

    public Type MemberCustomAttributeFilter { get; set; }

    public string ControlFlow { get; set; }

    #endregion
    }
}