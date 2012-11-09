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
using ActiveAttributes.Core.AdviceInfo;
using ActiveAttributes.Core.Discovery.Construction;
using ActiveAttributes.Core.Pointcuts;
using Castle.Core.Internal;
using Remotion.Utilities;
using ActiveAttributes.Core.Extensions;

namespace ActiveAttributes.Core.Discovery
{
  /// <summary>
  /// Transforms attributes declared on a <see cref="ICustomAttributeProvider"/> to an <see cref="Advice"/>.
  /// If the provider is a <see cref="Type"/> also sets the <see cref="Advice.Construction"/>.
  /// </summary>
  public interface ICustomAttributeProviderTransform
  {
    IAdviceBuilder GetAdviceBuilder (ICustomAttributeProvider customAttributeProvider, IAdviceBuilder parentAdviceBuilder);
  }

  public class CustomAttributeProviderTransform : ICustomAttributeProviderTransform
  {
    private readonly IAdviceBuilderFactory _adviceBuilderFactory;

    public CustomAttributeProviderTransform (IAdviceBuilderFactory adviceBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("adviceBuilderFactory", adviceBuilderFactory);

      _adviceBuilderFactory = adviceBuilderFactory;
    }

    public IAdviceBuilder GetAdviceBuilder (ICustomAttributeProvider customAttributeProvider, IAdviceBuilder parentAdviceBuilder)
    {
      ArgumentUtility.CheckNotNull ("customAttributeProvider", customAttributeProvider);

      var adviceBuilder = parentAdviceBuilder != null ? parentAdviceBuilder.Copy() : _adviceBuilderFactory.Create();

      var type = customAttributeProvider as Type;
      if (type != null)
        adviceBuilder.UpdateConstruction (new TypeConstruction (type));
      // TODO adviceBuilder.SetConstruction (new TypeConstruction (customAttributeProvider as Type));

      adviceBuilder.SetMethod (customAttributeProvider as MethodInfo);
      
      TrySetValue (customAttributeProvider, (AdviceNameAttribute x) => x.Name, adviceBuilder.SetName);
      TrySetValue (customAttributeProvider, (AdviceRoleAttribute x) => x.Role, adviceBuilder.SetRole);
      TrySetValue (customAttributeProvider, (AdviceExecutionAttribute x) => x.Execution, adviceBuilder.SetExecution);
      TrySetValue (customAttributeProvider, (AdviceScopeAttribute x) => x.Scope, adviceBuilder.SetScope);
      TrySetValue (customAttributeProvider, (AdvicePriorityAttribute x) => x.Priority, adviceBuilder.SetPriority);

      foreach (var pointcutAttribute in customAttributeProvider.GetCustomAttributes<PointcutAttributeBase> (true))
        adviceBuilder.AddPointcut (pointcutAttribute.Pointcut);

      return adviceBuilder;
    }

    private void TrySetValue<TAttribute, T> (ICustomAttributeProvider customAttributeProvider, Func<TAttribute, T> selector, Func<T, IAdviceBuilder> set)
        where TAttribute : Attribute
    {
      var attribute = customAttributeProvider.GetCustomAttributes<TAttribute> (true).SingleOrDefault();
      if (attribute != null)
        set (selector (attribute));
    }
  }
}