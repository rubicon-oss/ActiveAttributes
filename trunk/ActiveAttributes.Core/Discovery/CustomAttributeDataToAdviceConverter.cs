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
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Discovery
{
  public interface ICustomAttributeDataToAdviceConverter
  {
    Advice GetAdvice (IEnumerable<ICustomAttributeNamedArgument> customAttributeNamedArguments);
  }

  public class CustomAttributeDataToAdviceConverter : ICustomAttributeDataToAdviceConverter
  {
    public Advice GetAdvice (IEnumerable<ICustomAttributeNamedArgument> customAttributeNamedArguments)
    {
      var dictionary = new Dictionary<string, Type>
                       {
                           { "ApplyToType", typeof (TypePointcut) },
                           { "ApplyToTypeName", typeof (TypeNamePointcut) },
                           { "ApplyToNamespace", typeof (NamespacePointcut) },
                           //
                           { "MemberNameFilter", typeof (MemberNamePointcut) },
                           { "MemberReturnTypeFilter", typeof (ReturnTypePointcut) },
                           { "MemberArgumentsFilter", typeof (ArgumentsPointcut) },
                           { "MemberVisibilityFilter", typeof (VisibilityPointcut) },
                           { "MemberCustomAttributeFilter", typeof (CustomAttributePointcut) }
                       };

      var name = default (string);
      var role = default (string);
      var execution = AdviceExecution.Undefined;
      var scope = AdviceScope.Undefined;
      var priority = 0;
      var pointcuts = new List<IPointcut>();

      foreach (var argument in customAttributeNamedArguments)
      {
        TryGetValue (argument, "Priority", ref priority);
        TryGetValue (argument, "Role", ref role);
        TryGetValue (argument, "Name", ref name);
        TryGetValue (argument, "Execution", ref execution);
        TryGetValue (argument, "Scope", ref scope);

        Type pointcutType;
        if (dictionary.TryGetValue(argument.MemberInfo.Name, out pointcutType))
        {
          var pointcut = pointcutType.CreateInstance<IPointcut> (argument.Value);
          pointcuts.Add (pointcut);
        }
      }

      return new Advice (execution, scope, priority, null, pointcuts, role, name);
    }

    private void TryGetValue<T> (ICustomAttributeNamedArgument argument, string memberName, ref T value)
    {
      if (argument.MemberInfo.Name == memberName)
        value = (T) argument.Value;
    }
  }
}