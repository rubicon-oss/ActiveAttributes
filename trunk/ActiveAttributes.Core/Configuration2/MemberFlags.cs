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

namespace ActiveAttributes.Core.Configuration2
{
  [Flags]
  public enum MemberFlags
  {
    Static = 1,
    NonStatic = 2,
    Final = 4,
    NonFinal = 8,
    Virtual = 16,
    NonVirtual = 32,
    NewSlot = 64,
    NonNewSlot = 128,
    Overridable = NonStatic | NonFinal | Virtual, // default
    All = ~0,
    None = 0,
  }
}