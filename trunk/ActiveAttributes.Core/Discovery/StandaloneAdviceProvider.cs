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
using ActiveAttributes.Core.Infrastructure;

namespace ActiveAttributes.Core.Discovery
{
  /// <summary>
  /// Uses every public method as advice method
  /// </summary>
  public interface IStandaloneAdviceProvider
  {
    IEnumerable<Advice> GetAdvices (Type aspectType);
  }

  public class StandaloneAdviceProvider : IStandaloneAdviceProvider
  {
    private readonly IAdviceMerger _adviceMerger;
    private readonly ICustomAttributeProviderToAdviceConverter _customAttributeConverter;

    public StandaloneAdviceProvider (IAdviceMerger adviceMerger, ICustomAttributeProviderToAdviceConverter customAttributeConverter)
    {
      _adviceMerger = adviceMerger;
      _customAttributeConverter = customAttributeConverter;
    }

    public IEnumerable<Advice> GetAdvices (Type aspectType)
    {
      var typeAdvice = _customAttributeConverter.GetAdvice (aspectType);

      return from m in aspectType.GetMethods (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
             let methodAdvice = _customAttributeConverter.GetAdvice (m)
             select _adviceMerger.Merge (typeAdvice, methodAdvice);
    }
  }
}