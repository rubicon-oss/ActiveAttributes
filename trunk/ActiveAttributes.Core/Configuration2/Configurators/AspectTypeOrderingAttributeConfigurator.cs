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
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Configuration2.CustomAttributes;
using ActiveAttributes.Core.Configuration2.Rules;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Configuration2.Configurators
{
  // TODO Introduce base class for AspectTypeOrderingAttributeConfigurator and AspectRoleOrderingAttributeConfigurator?
  // TODO Pro are the three foreach's they have in common
  // TODO Con is that AspectRoleOrderingAttributeConfigurator needs to get the role of the aspect type from the configuration
  /// <summary>
  /// Serves as a configurator, which adds <see cref="TypeOrderingRule"/>s through <see cref="AspectTypeOrderingAttribute"/>s.
  /// </summary>
  public class AspectTypeOrderingAttributeConfigurator : IActiveAttributesConfigurator
  {
    private readonly IEnumerable<Type> _aspectTypes;

    public AspectTypeOrderingAttributeConfigurator (IEnumerable<Type> aspectTypes)
    {
      ArgumentUtility.CheckNotNull ("aspectTypes", aspectTypes);
      Assertion.IsTrue (aspectTypes.All (x => typeof (AspectAttribute).IsAssignableFrom (x)));

      _aspectTypes = aspectTypes;
    }

    public void Initialize (IActiveAttributesConfiguration activeAttributesConfiguration)
    {
      foreach (var aspectType in _aspectTypes)
      {
        foreach (var typeOrderingAttribute in aspectType.GetCustomAttributes<AspectTypeOrderingAttribute>(inherit:true))
        {
          foreach (var otherAspectType in typeOrderingAttribute.AspectTypes)
          {
            Assertion.IsTrue (aspectType != otherAspectType);

            var beforeType = typeOrderingAttribute.Position == Position.Before
                                 ? aspectType
                                 : otherAspectType;
            var afterType = typeOrderingAttribute.Position == Position.Before
                                ? otherAspectType
                                : aspectType;
            var typeOrderingRule = new TypeOrderingRule (GetType ().Name, beforeType, afterType);
            activeAttributesConfiguration.AspectOrderingRules.Add (typeOrderingRule);
          }
        }
      }
    }
  }

  // TODO (high)
  /// <summary>
  /// Serves as a provider for all aspect types in a given assembly.
  /// </summary>
  public interface IAspectTypesProvider
  {
    IEnumerable<Type> GetAspectTypes (params System.Reflection.Assembly[] assemblies);
  }

  public class AspectTypesProvider : IAspectTypesProvider
  {
    public IEnumerable<Type> GetAspectTypes (params System.Reflection.Assembly[] assemblies)
    {
      // TODO (low) cache
      return from assembly in assemblies
             from type in assembly.GetTypes()
             where typeof (AspectAttribute).IsAssignableFrom (type)
             select type;
    }
  }

  public static class Extensions
  {
    public static IEnumerable<T> GetCustomAttributes<T> (this ICustomAttributeProvider customAttributeProvider, bool inherit) where T : Attribute
    {
      return customAttributeProvider.GetCustomAttributes (typeof (T), inherit).Cast<T>();
    }
  }
}