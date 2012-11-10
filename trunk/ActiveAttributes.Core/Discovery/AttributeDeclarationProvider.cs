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
using System.Reflection;
using ActiveAttributes.Core.Discovery.Construction;
using ActiveAttributes.Core.Extensions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Discovery
{
  public interface IAttributeDeclarationProvider
  {
    // TODO add support for 'mustInherit'
    IEnumerable<IAdviceBuilder> GetAdviceBuilders (MemberInfo member);
  }

  public class AttributeDeclarationProvider : IAttributeDeclarationProvider
  {
    private readonly IClassDeclarationProvider _classDeclarationProvider;
    private readonly ICustomAttributeDataTransform _customAttributeDataTransform;

    public AttributeDeclarationProvider (
        IClassDeclarationProvider classDeclarationProvider,
        ICustomAttributeDataTransform customAttributeDataTransform)
    {
      ArgumentUtility.CheckNotNull ("classDeclarationProvider", classDeclarationProvider);
      ArgumentUtility.CheckNotNull ("customAttributeDataTransform", customAttributeDataTransform);

      _classDeclarationProvider = classDeclarationProvider;
      _customAttributeDataTransform = customAttributeDataTransform;
    }

    public IEnumerable<IAdviceBuilder> GetAdviceBuilders (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      var customAttributeDatas = TypePipeCustomAttributeData.GetCustomAttributes (member);
      foreach (var customAttributeData in customAttributeDatas)
      {
        if (!customAttributeData.IsAspectAttribute ())
          continue;

        var aspectType = customAttributeData.Constructor.DeclaringType;
        var aspectTypeAdviceBuilders = _classDeclarationProvider.GetAdviceBuilders (aspectType).ConvertToCollection();

        var attributeAdviceBuilders = _customAttributeDataTransform.UpdateAdviceBuilders (customAttributeData, aspectTypeAdviceBuilders);

        var constructionInfo = new CustomAttributeDataConstruction (customAttributeData);
        foreach (var attributeAdviceBuilder in attributeAdviceBuilders)
          yield return attributeAdviceBuilder.SetConstruction (constructionInfo);
      }
    }
  }
}