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
using ActiveAttributes.Core.Attributes;
using ActiveAttributes.Core.Attributes.AdviceInfo;
using ActiveAttributes.Core.Attributes.Pointcuts;
using ActiveAttributes.Core.Infrastructure;
using Remotion.Utilities;
using Castle.Core.Internal;

namespace ActiveAttributes.Core.Assembly
{
  public interface IAdviceProvider
  {
    Advice GetAdvice (ICustomAttributeProvider adviceProvider);
  }

  public interface IAdviceMerger
  {
    Advice GetMergedAdvice (Advice previousAdvice, Advice newAdvice);
  }

  public class AdviceProvider : IAdviceProvider
  {
    private static readonly MethodInfo s_dummy = typeof (object).GetMethod ("GetType()");

    public Advice GetAdvice (ICustomAttributeProvider adviceProvider)
    {
      var scope = TryGetValue (adviceProvider, (AdviceScopeAttribute x) => x.Scope);
      var execution = TryGetValue (adviceProvider, (AdviceExecutionAttribute x) => x.Execution);
      var role = TryGetValue (adviceProvider, (AdviceRoleAttribute x) => x.Role);
      var name = TryGetValue (adviceProvider, (AdviceNameAttribute x) => x.Name);
      var priority = TryGetValue (adviceProvider, (AdvicePriorityAttribute x) => x.Priority);
      var pointcuts = adviceProvider.GetAttributes<PointcutAttributeBase> ().Select (x => x.Pointcut).ToList ();

      return new Advice (execution, scope, priority, adviceProvider as MethodInfo, pointcuts, role, name);
    }

    private TValue TryGetValue<TAttribute, TValue> (ICustomAttributeProvider provider, Func<TAttribute, TValue> func)
        where TAttribute : Attribute
    {
      var attribute = provider.GetAttributes<TAttribute> ().Single ();
      return attribute != null ? func (attribute) : default (TValue);
    }
  }


  public class Discovery
  {
    public IEnumerable<Advice> GetAdvices (Type aspectType)
    {
      ArgumentUtility.CheckNotNull ("aspectType", aspectType);
      Assertion.IsTrue (typeof (IAspect).IsAssignableFrom (aspectType));

      var adviceByType = GetAdvice(aspectType);
      var adviceMethods = aspectType.GetMethods().Where (x => x.GetAttributes<AdviceAttribute>().Any());

      foreach (var adviceMethod in adviceMethods)
      {
        var adviceByMethod = GetAdvice (adviceMethod);
        yield return MergeAdvices (adviceByType, adviceByMethod);
      }
    }

    // AdviceMerger
    private Advice MergeAdvices (Advice advice1, Advice advice2)
    {
      return advice2;
    }

    // AdviceTransformer
    private Advice GetAdvice (ICustomAttributeProvider adviceProvider)
    {
      return null;
    }

  }
}