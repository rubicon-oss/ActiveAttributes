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

namespace ActiveAttributes.Core.Assembly.Configuration.Rules
{
  public class RoleOrderRule : IOrderRule
  {
    private readonly IAspectConfiguration _configuration;

    private readonly string _role1;
    private readonly string _role2;

    public RoleOrderRule (string role1, string role2, IAspectConfiguration configuration)
    {
      _role1 = role1;
      _role2 = role2;
      _configuration = configuration;
    }

    public int Compare (IAspectGenerator x, IAspectGenerator y)
    {
      var type1 = x.Descriptor.AspectType;
      var type2 = y.Descriptor.AspectType;

      if (!_configuration.Roles.ContainsKey (type1) || !_configuration.Roles.ContainsKey (type2))
        return 0;

      var role1 = _configuration.Roles[type1];
      var role2 = _configuration.Roles[type2];

      if (role1 == _role1 && role2 == _role2)
        return -1;
      if (role2 == _role1 && role1 == _role2)
        return 1;

      return 0;
    }
  }
}