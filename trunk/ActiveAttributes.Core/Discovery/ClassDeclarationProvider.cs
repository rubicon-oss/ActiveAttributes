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
using ActiveAttributes.Core.Assembly;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Discovery
{
  public interface IClassDeclarationProvider
  {
    IEnumerable<IAdviceBuilder> GetAdviceBuilders (Type aspectType, IAdviceBuilder parentAdviceBuilder = null);
  }

  public class ClassDeclarationProvider : IClassDeclarationProvider
  {
    private readonly ICustomAttributeProviderTransform _customAttributeConverter;

    public ClassDeclarationProvider (ICustomAttributeProviderTransform customAttributeConverter)
    {
      ArgumentUtility.CheckNotNull ("customAttributeConverter", customAttributeConverter);

      _customAttributeConverter = customAttributeConverter;
    }

    public IEnumerable<IAdviceBuilder> GetAdviceBuilders (Type aspectType, IAdviceBuilder parentAdviceBuilder = null)
    {
      ArgumentUtility.CheckNotNull ("aspectType", aspectType);

      // TODO need to apply constructionInfo for class advice declaration
      var typeAdviceBuilder = _customAttributeConverter.GetAdviceBuilder (aspectType, parentAdviceBuilder);

      return from m in aspectType.GetMethods (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
             let methodAdviceBuilder = _customAttributeConverter.GetAdviceBuilder (m, typeAdviceBuilder)
             select methodAdviceBuilder;
    }
  }
}