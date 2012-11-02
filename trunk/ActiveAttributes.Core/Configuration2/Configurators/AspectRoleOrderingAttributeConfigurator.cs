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
using System.Linq;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration2.CustomAttributes;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Infrastructure.Orderings;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Configuration2.Configurators
{
  /// <summary>
  /// Serves as a configurator, which adds <see cref="RoleOrdering"/>s through <see cref="AspectRoleOrderingAttribute"/>s.
  /// </summary>
  public class AspectRoleOrderingAttributeConfigurator : IActiveAttributesConfigurator
  {
    private readonly IEnumerable<Type> _aspectTypes;

    public AspectRoleOrderingAttributeConfigurator (IEnumerable<Type> aspectTypes)
    {
      ArgumentUtility.CheckNotNull ("aspectTypes", aspectTypes);
      Assertion.IsTrue (aspectTypes.All (x => typeof (AspectAttribute).IsAssignableFrom (x)));

      _aspectTypes = aspectTypes;
    }

    /// <inheritdoc />
    /// <exception cref="Exception">
    /// The <see cref="AspectAttribute"/> applied with the <see cref="AspectRoleOrderingAttribute"/> doesn't provide a <see cref="AspectRoleAttribute"/>.
    /// </exception>
    public void Initialize (IActiveAttributesConfiguration activeAttributesConfiguration)
    {
      foreach (var aspectType in _aspectTypes)
      {
        var typeOrderingAttributes = aspectType.GetCustomAttributes<AspectRoleOrderingAttribute> (inherit: true).ConvertToCollection();
        if (!typeOrderingAttributes.Any())
          continue;

        string aspectRole;
        if (!activeAttributesConfiguration.AspectRoles.TryGetValue (aspectType, out aspectRole))
          throw new Exception ("TODO (low) exception text");

        foreach (var roleOrderingAttribute in typeOrderingAttributes)
        {
          foreach (var otherAspectRole in roleOrderingAttribute.AspectRoles)
          {
            Assertion.IsTrue (aspectRole != otherAspectRole);

            var beforeRole = roleOrderingAttribute.Position == Position.Before
                                 ? aspectRole
                                 : otherAspectRole;
            var afterRole = roleOrderingAttribute.Position == Position.Before
                                ? otherAspectRole
                                : aspectRole;
            var typeOrderingRule = new RoleOrdering (GetType().Name, beforeRole, afterRole, activeAttributesConfiguration);
            activeAttributesConfiguration.AspectOrderingRules.Add (typeOrderingRule);
          }
        }
      }
    }
  }
}