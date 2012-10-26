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
using ActiveAttributes.Core.Aspects;

namespace ActiveAttributes.Core.Configuration2.CustomAttributes
{
  /// <summary>
  /// Attribute that is used on other <see cref="Attribute"/>s in order to signalize that a given <see cref="AspectAttribute"/> should be applied.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
  public class ApplyAspectAttribute : Attribute
  {
    public ApplyAspectAttribute (Type aspectType)
    {
      AspectType = aspectType;
      if (!typeof (AspectAttribute).IsAssignableFrom (aspectType))
        throw new ArgumentException ("Type must derive from 'AspectAttribute'", "aspectType");
    }

    public Type AspectType { get; private set; }
  }
}