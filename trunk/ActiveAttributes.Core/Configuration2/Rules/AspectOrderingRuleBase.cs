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
using System.Text;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Configuration2.Rules
{
  /// <summary>
  /// Serves as a base class for <see cref="IAspectOrderingRule"/>s.
  /// </summary>
  public abstract class AspectOrderingRuleBase : IAspectOrderingRule
  {
    protected AspectOrderingRuleBase (string source)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("source", source);

      Source = source;
    }

    public string Source { get; private set; }

    public abstract int Compare (IAspectDescriptor x, IAspectDescriptor y);

    protected string ToString (string before, string after)
    {
      var stringBuilder = new StringBuilder();
      stringBuilder
          .Append (GetType().Name)
          .Append (" [")
          .Append (Source)
          .Append ("]: ")
          .Append (before)
          .Append (" -> ")
          .Append (after);
      return stringBuilder.ToString();
    }
  }
}