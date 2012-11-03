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
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.Construction;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Discovery
{
  public interface IAspectDeclarationHelper
  {
    // TODO add support for 'mustInherit'
    IEnumerable<AspectDeclaration> GetAspectDeclarations (MemberInfo member);
  }

  public class AspectDeclarationProviderHelper : IAspectDeclarationHelper
  {
    private readonly IStandaloneAdviceProvider _standaloneAdviceProvider;
    private readonly ICustomAttributeDataToAdviceConverter _customAttributeDataToAdviceConverter;
    private readonly IAdviceMerger _adviceMerger;

    public AspectDeclarationProviderHelper (
        IStandaloneAdviceProvider standaloneAdviceProvider,
        ICustomAttributeDataToAdviceConverter customAttributeDataToAdviceConverter,
        IAdviceMerger adviceMerger)
    {
      ArgumentUtility.CheckNotNull ("standaloneAdviceProvider", standaloneAdviceProvider);
      ArgumentUtility.CheckNotNull ("customAttributeDataToAdviceConverter", customAttributeDataToAdviceConverter);
      ArgumentUtility.CheckNotNull ("adviceMerger", adviceMerger);

      _standaloneAdviceProvider = standaloneAdviceProvider;
      _customAttributeDataToAdviceConverter = customAttributeDataToAdviceConverter;
      _adviceMerger = adviceMerger;
    }

    public IEnumerable<AspectDeclaration> GetAspectDeclarations (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      var customAttributeDatas = TypePipeCustomAttributeData.GetCustomAttributes (member);
      foreach (var customAttributeData in customAttributeDatas)
      {
        if (!customAttributeData.IsAspectAttribute())
          continue;

        var attributeAdvice = _customAttributeDataToAdviceConverter.GetAdvice (customAttributeData.NamedArguments);
        var aspectType = customAttributeData.Constructor.DeclaringType;
        var aspectTypeAdvices = _standaloneAdviceProvider.GetAdvices (aspectType);
        var mergedAdvices = aspectTypeAdvices.Select (x => _adviceMerger.Merge (x, attributeAdvice));

        var aspectConstructionInfo = new CustomAttributeDataAspectConstructionInfo (customAttributeData);
        yield return new AspectDeclaration (null, aspectConstructionInfo, mergedAdvices);
      }
    }
  }
}