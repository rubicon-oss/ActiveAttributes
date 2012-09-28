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
// 

using System;
using System.Collections.Generic;
using System.Linq;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Configuration.Rules;

namespace ActiveAttributes.Core.Assembly.Configuration.Configurators
{
  public class PresetAspectConfigurator : IAspectConfigurator
  {
    private readonly System.Reflection.Assembly[] _assemblies;

    public PresetAspectConfigurator (params System.Reflection.Assembly[] assemblies)
    {
      _assemblies = assemblies;
    }

    public void Initialize (IAspectConfiguration configuration)
    {
      var aspectTypes = from assembly in _assemblies
                        from type in assembly.GetTypes()
                        where typeof (AspectAttribute).IsAssignableFrom (type)
                        select type;

      foreach (var aspectType in aspectTypes)
      {
        var roleAttribute = aspectType.GetCustomAttributes (typeof (AspectRoleAttribute), true).Cast<AspectRoleAttribute>().SingleOrDefault();
        if (roleAttribute != null)
          configuration.Roles.Add (aspectType, roleAttribute.RoleName);

        var presets = aspectType.GetCustomAttributes (typeof (AspectOrderingAttribute), true).Cast<AspectOrderingAttribute>();

        var type = aspectType;
        foreach (var orderRule in presets.SelectMany (x => GetOrderRules (type, x, configuration)))
          configuration.Rules.Add (orderRule);
      }
    }

    private IEnumerable<IOrderRule> GetOrderRules (Type aspectType, AspectOrderingAttribute preset, IAspectConfiguration configuration)
    {
      if (preset.AspectTypes != null)
        return GetTypeOrderRules (aspectType, preset);
      else if (preset.AspectRoles != null)
        return GetRoleOrderRules (aspectType, preset, configuration);
      else
        throw new Exception ("should not reach"); // TODO
    }

    private IEnumerable<IOrderRule> GetTypeOrderRules (Type aspectType, AspectOrderingAttribute preset)
    {
      return (from otherType in preset.AspectTypes
              let beforeType = preset.Position == OrderPosition.Before ? aspectType : otherType
              let afterType = preset.Position == OrderPosition.Before ? otherType : aspectType
              select new TypeOrderRule (aspectType.Name, beforeType, afterType)).Cast<IOrderRule>();
    }

    private IEnumerable<IOrderRule> GetRoleOrderRules (Type aspectType, AspectOrderingAttribute preset, IAspectConfiguration configuration)
    {
      string aspectRole;
      if (!configuration.Roles.TryGetValue (aspectType, out aspectRole))
        throw new Exception ("role not set on " + aspectType.Name); // TODO

      return (from otherRole in preset.AspectRoles
              let beforeRole = preset.Position == OrderPosition.Before ? aspectRole : otherRole
              let afterRole = preset.Position == OrderPosition.Before ? otherRole : aspectRole
              select new RoleOrderRule (aspectType.Name, beforeRole, afterRole, configuration)).Cast<IOrderRule>();
    }
  }
}
