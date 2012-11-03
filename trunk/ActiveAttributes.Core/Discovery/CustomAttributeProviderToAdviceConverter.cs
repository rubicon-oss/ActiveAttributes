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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Attributes.AdviceInfo;
using ActiveAttributes.Core.Attributes.Pointcuts;
using ActiveAttributes.Core.Infrastructure;
using Castle.Core.Internal;

namespace ActiveAttributes.Core.Discovery
{
  public interface ICustomAttributeProviderToAdviceConverter
  {
    Advice GetAdvice (ICustomAttributeProvider customAttributeProvider);
  }

  public class CustomAttributeProviderToAdviceConverter : ICustomAttributeProviderToAdviceConverter
  {
    public Advice GetAdvice (ICustomAttributeProvider customAttributeProvider)
    {
      var priority = TryGetValue (customAttributeProvider, (AdvicePriorityAttribute x) => x.Priority);
      var role = TryGetValue (customAttributeProvider, (AdviceRoleAttribute x) => x.Role);
      var name = TryGetValue (customAttributeProvider, (AdviceNameAttribute x) => x.Name);
      var execution = TryGetValue (customAttributeProvider, (AdviceExecutionAttribute x) => x.Execution);
      var scope = TryGetValue (customAttributeProvider, (AdviceScopeAttribute x) => x.Scope);

      var pointcuts = customAttributeProvider.GetAttributes<PointcutAttributeBase>().Select (x => x.Pointcut);

      return new Advice (execution, scope, priority, customAttributeProvider as MethodInfo, pointcuts, role, name);
    }

    private T TryGetValue<TAttribute, T> (ICustomAttributeProvider customAttributeProvider, Func<TAttribute, T> factory)
        where TAttribute : Attribute
    {
      var attribute = customAttributeProvider.GetAttribute<TAttribute>();
      return attribute != null ? factory (attribute) : default (T);
    }
  }
}