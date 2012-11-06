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
using Remotion.Utilities;

namespace ActiveAttributes.Core.Attributes.Intertype
{
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public abstract class IntertypeBaseAttribute : Attribute
  {
    private readonly string _memberName;

    protected IntertypeBaseAttribute (string memberName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("memberName", memberName);

      _memberName = memberName;
    }

    public string MemberName
    {
      get { return _memberName; }
    }
  }
}