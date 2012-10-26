﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Utilities;

namespace ActiveAttributes.Core.Configuration2.Configurators
{
  /// <summary>
  /// Serves as a configurator, which adds roles for <see cref="AspectAttribute"/>s through <see cref="AspectRoleAttribute"/>s.
  /// </summary>
  public class AspectRoleAttributeConfigurator : IActiveAttributesConfigurator
  {
    private readonly IEnumerable<Type> _aspectTypes;

    public AspectRoleAttributeConfigurator (IEnumerable<Type> aspectTypes)
    {
      ArgumentUtility.CheckNotNull ("aspectTypes", aspectTypes);
      Assertion.IsTrue (aspectTypes.All (x => typeof (AspectAttribute).IsAssignableFrom (x)));

      _aspectTypes = aspectTypes;
    }

    public void Initialize (IActiveAttributesConfiguration activeAttributesConfiguration)
    {
      foreach (var aspectType in _aspectTypes)
      {
        var aspectRoleAttribute = aspectType.GetCustomAttributes<AspectRoleAttribute> (inherit: true).SingleOrDefault();
        if (aspectRoleAttribute != null)
        {
          var roleName = aspectRoleAttribute.RoleName;
          activeAttributesConfiguration.AspectRoles.Add (aspectType, roleName);
        }
      }
    }
  }
}