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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Construction;
using ActiveAttributes.Core.Pointcuts;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Discovery
{
  /// <summary>
  /// Transforms <see cref="ICustomAttributeNamedArgument"/>s declared in <see cref="ICustomAttributeData"/>s to an <see cref="Advice"/>.
  /// </summary>
  public interface ICustomAttributeDataTransform
  {
    IAdviceBuilder GetAdviceBuilder (ICustomAttributeData customAttributeData);
  }

  public class CustomAttributeDataTransform : ICustomAttributeDataTransform
  {
    private static readonly Dictionary<string, Type> s_dictionary
        = new Dictionary<string, Type>
          {
              { "ApplyToType", typeof (TypePointcut) },
              { "ApplyToTypeName", typeof (TypeNamePointcut) },
              { "ApplyToNamespace", typeof (NamespacePointcut) },
              //
              { "MemberNameFilter", typeof (MemberNamePointcut) },
              { "MemberReturnTypeFilter", typeof (ReturnTypePointcut) },
              { "MemberArgumentFilter", typeof (ArgumentTypePointcut) },
              { "MemberVisibilityFilter", typeof (VisibilityPointcut) },
              { "MemberCustomAttributeFilter", typeof (CustomAttributePointcut) }
          };

    private readonly IAdviceBuilderFactory _adviceBuilderFactory;

    public CustomAttributeDataTransform (IAdviceBuilderFactory adviceBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("adviceBuilderFactory", adviceBuilderFactory);

      _adviceBuilderFactory = adviceBuilderFactory;
    }

    public IAdviceBuilder GetAdviceBuilder (ICustomAttributeData customAttributeData)
    {
      ArgumentUtility.CheckNotNull ("customAttributeData", customAttributeData);

      var adviceBuilder = _adviceBuilderFactory.Create();

      var constructionInfo = new CustomAttributeDataConstruction (customAttributeData);
      adviceBuilder.SetConstruction (constructionInfo);

      foreach (var argument in customAttributeData.NamedArguments)
      {
        TrySetValue (argument, "Name", new Func<string, IAdviceBuilder> (adviceBuilder.SetName));
        TrySetValue (argument, "Role", new Func<string, IAdviceBuilder> (adviceBuilder.SetRole));
        TrySetValue (argument, "Execution", new Func<AdviceExecution, IAdviceBuilder> (adviceBuilder.SetExecution));
        TrySetValue (argument, "Scope", new Func<AdviceScope, IAdviceBuilder> (adviceBuilder.SetScope));
        TrySetValue (argument, "Priority", new Func<int, IAdviceBuilder> (adviceBuilder.SetPriority));

        Type pointcutType;
        if (s_dictionary.TryGetValue (argument.MemberInfo.Name, out pointcutType))
        {
          var pointcut = pointcutType.CreateInstance<IPointcut> (argument.Value);
          adviceBuilder.AddPointcut (pointcut);
        }
      }

      return adviceBuilder;
    }

    private void TrySetValue<T> (ICustomAttributeNamedArgument namedArgument, string memberName, Func<T, IAdviceBuilder> set)
    {
      if (namedArgument.MemberInfo.Name == memberName)
        set ((T) namedArgument.Value);
    }
  }
}