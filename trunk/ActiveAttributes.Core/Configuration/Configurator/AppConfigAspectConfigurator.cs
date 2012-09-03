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
using System.Configuration;
using ActiveAttributes.Core.Configuration.AppConfigElements;
using ActiveAttributes.Core.Configuration.Rules;

namespace ActiveAttributes.Core.Configuration.Configurator
{
  public class AppConfigAspectConfigurator : IAspectConfigurator
  {
    public void Initialize (IAspectConfiguration configuration)
    {
      return;
      var section = (AspectsConfigurationSection) ConfigurationManager.GetSection ("aspects");

      foreach (TypeRuleElement item in section.TypeRules)
      {
        var beforeType = Type.GetType (item.BeforeType, true);
        var afterType = Type.GetType (item.AfterType, true);
        var rule = new TypeOrderRule (beforeType, afterType);
        configuration.Rules.Add (rule);
      }
    }
  }
}