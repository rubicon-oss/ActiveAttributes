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
using ActiveAttributes.Core.Assembly;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Configuration2.Rules
{
  /// <summary>
  /// A <see cref="IAspectOrderingRule"/> based on the role of an aspect.
  /// </summary>
  public class RoleOrderingRule : AspectOrderingRuleBase
  {
    private readonly IActiveAttributesConfiguration _activeAttributesConfiguration;

    private readonly string _role1;
    private readonly string _role2;

    public RoleOrderingRule (string source, string role1, string role2, IActiveAttributesConfiguration activeAttributesConfiguration)
        : base (source)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("role1", role1);
      ArgumentUtility.CheckNotNullOrEmpty ("role2", role2);
      ArgumentUtility.CheckNotNull ("activeAttributesConfiguration", activeAttributesConfiguration);

      _role1 = role1;
      _role2 = role2;
      _activeAttributesConfiguration = activeAttributesConfiguration;
    }

    public override int Compare (IAspectDescriptor x, IAspectDescriptor y)
    {
      var type1 = x.Type;
      var type2 = y.Type;

      if (!_activeAttributesConfiguration.AspectRoles.ContainsKey (type1) || !_activeAttributesConfiguration.AspectRoles.ContainsKey (type2))
        return 0;

      var role1 = _activeAttributesConfiguration.AspectRoles[type1];
      var role2 = _activeAttributesConfiguration.AspectRoles[type2];

      if (role1 == _role1 && role2 == _role2)
        return -1;
      else if (role2 == _role1 && role1 == _role2)
        return 1;
      else
        return 0;
    }

    public override bool Equals (object obj)
    {
      if (ReferenceEquals (null, obj))
        return false;
      if (ReferenceEquals (this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals ((RoleOrderingRule) obj);
    }

    public override int GetHashCode ()
    {
      unchecked
      {
        var hashCode = (_activeAttributesConfiguration != null ? _activeAttributesConfiguration.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (_role1 != null ? _role1.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (_role2 != null ? _role2.GetHashCode() : 0);
        return hashCode;
      }
    }

    public override string ToString ()
    {
      return ToString (_role1, _role2);
    }

    private bool Equals (RoleOrderingRule other)
    {
      return Equals (_activeAttributesConfiguration, other._activeAttributesConfiguration) && string.Equals (_role1, other._role1)
             && string.Equals (_role2, other._role2);
    }
  }
}