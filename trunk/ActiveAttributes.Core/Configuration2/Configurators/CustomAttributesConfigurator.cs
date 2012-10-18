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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Configuration2.CustomAttributes;
using ActiveAttributes.Core.Configuration2.Rules;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Configuration2.Configurators
{
  public class CustomAttributesConfigurator : IActiveAttributesConfigurator
  {
    private readonly IEnumerable<System.Reflection.Assembly> _assemblies;

    public CustomAttributesConfigurator (IEnumerable<System.Reflection.Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("assemblies", assemblies);

      _assemblies = assemblies;
    }

    public void Initialize (IActiveAttributesConfiguration activeAttributesConfiguration)
    {
      ArgumentUtility.CheckNotNull ("activeAttributesConfiguration", activeAttributesConfiguration);

      var aspectTypes = from assembly in _assemblies
                        from type in assembly.GetTypes()
                        where typeof (AspectAttribute).IsAssignableFrom (type)
                        select type;

      foreach (var aspectType in aspectTypes)
      {
        var roleAttribute = aspectType.GetCustomAttributes (typeof (AspectRoleAttribute), true).Cast<AspectRoleAttribute>().SingleOrDefault();
        if (roleAttribute != null)
          activeAttributesConfiguration.AspectRoles.Add (aspectType, roleAttribute.RoleName);

        var presets = aspectType.GetCustomAttributes (typeof (AspectTypeOrderingAttribute), true).Cast<AspectTypeOrderingAttribute>();

        var type = aspectType;
        foreach (var orderRule in presets.SelectMany (x => GetOrderRules (type, x, activeAttributesConfiguration)))
          activeAttributesConfiguration.AspectOrderingRules.Add (orderRule);
      }
    }

    private IEnumerable<IAspectOrderingRule> GetOrderRules (
        Type aspectType, AspectTypeOrderingAttribute preset, IActiveAttributesConfiguration activeAttributesConfiguration)
    {
      yield break;
      //Assertion.IsTrue (preset.AspectTypes != null || preset.AspectRoles != null);
      //Assertion.IsFalse (preset.AspectTypes != null && preset.AspectRoles != null);

      //return preset.AspectTypes != null
      //           ? GetTypeOrderRules (aspectType, preset)
      //           : GetRoleOrderRules (aspectType, preset, activeAttributesConfiguration);
    }

    private IEnumerable<IAspectOrderingRule> GetTypeOrderRules (Type aspectType, AspectTypeOrderingAttribute preset)
    {
      return (from otherType in preset.AspectTypes
              let beforeType = preset.Position == Position.Before ? aspectType : otherType
              let afterType = preset.Position == Position.Before ? otherType : aspectType
              select new TypeOrderingRule (aspectType.Name, beforeType, afterType)).Cast<IAspectOrderingRule>();
    }

    private IEnumerable<IAspectOrderingRule> GetRoleOrderRules (
        Type aspectType, AspectTypeOrderingAttribute preset, IActiveAttributesConfiguration activeAttributesConfiguration)
    {
      string aspectRole;
      if (!activeAttributesConfiguration.AspectRoles.TryGetValue (aspectType, out aspectRole))
      {
        var message = string.Format ("Role for type '{0}' not set.", aspectType.Name);
        throw new ConfigurationErrorsException (message);
      }

      yield break;
      //return (from otherRole in preset.AspectRoles
      //        let beforeRole = preset.Position == OrderPosition.Before ? aspectRole : otherRole
      //        let afterRole = preset.Position == OrderPosition.Before ? otherRole : aspectRole
      //        select new RoleOrderingRule (aspectType.Name, beforeRole, afterRole, activeAttributesConfiguration)).Cast<IAspectOrderingRule>();
    }
  }
}