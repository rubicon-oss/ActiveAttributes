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
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Declaration
{
  [ConcreteImplementation (typeof (AttributeDeclarationProvider))]
  public interface IAttributeDeclarationProvider
  {
    IEnumerable<IAdviceBuilder> GetAdviceBuilders (ICustomAttributeData customAttributeData);
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

    public IEnumerable<IAdviceBuilder> GetAdviceBuilders (ICustomAttributeData customAttributeData)
    {
      var aspectType = customAttributeData.Constructor.DeclaringType;
      var aspectTypeAdviceBuilders = _classDeclarationProvider.GetAdviceBuilders (aspectType).ConvertToCollection();
      return _customAttributeDataTransform.UpdateAdviceBuilders (customAttributeData, aspectTypeAdviceBuilders);
    }
  }
}