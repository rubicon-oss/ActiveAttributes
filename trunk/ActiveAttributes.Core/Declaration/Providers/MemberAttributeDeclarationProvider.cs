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
using ActiveAttributes.Utilities;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Declaration.Providers
{
  [ConcreteImplementation (typeof (MemberAttributeDeclarationProvider))]
  public interface IMemberAttributeDeclarationProvider
  {
    IEnumerable<IAdviceBuilder> GetAdviceBuilders (MemberInfo baseMember, IEnumerable<MemberInfo> relatedMembers);
  }

  public class MemberAttributeDeclarationProvider : IMemberAttributeDeclarationProvider
  {
    private readonly IAttributeDeclarationProvider _attributeDeclarationProvider;
    private readonly ICustomAttributeDataHelper _customAttributeDataHelper;

    public MemberAttributeDeclarationProvider (
        IAttributeDeclarationProvider attributeDeclarationProvider,
        ICustomAttributeDataHelper customAttributeDataHelper)
    {
      ArgumentUtility.CheckNotNull ("attributeDeclarationProvider", attributeDeclarationProvider);
      ArgumentUtility.CheckNotNull ("customAttributeDataHelper", customAttributeDataHelper);

      _attributeDeclarationProvider = attributeDeclarationProvider;
      _customAttributeDataHelper = customAttributeDataHelper;
    }

    public IEnumerable<IAdviceBuilder> GetAdviceBuilders (MemberInfo baseMember, IEnumerable<MemberInfo> relatedMembers)
    {
      var customAttributeDatas = new List<ICustomAttributeData>();

      foreach (var member in relatedMembers)
      {
        var currentMember = member;
        var customAttributes = TypePipeCustomAttributeData.GetCustomAttributes (currentMember).ToArray();
        var customAttributeDatasOnMember = customAttributes
            .Where (x1 => _customAttributeDataHelper.IsAspectAttribute (x1))
            .Where (x => CheckInheriting (baseMember, currentMember, x))
            .Where (x => CheckAllowsMultiple (x, customAttributeDatas));

        customAttributeDatas.AddRange (customAttributeDatasOnMember);
      }

      return customAttributeDatas.SelectMany (x4 => _attributeDeclarationProvider.GetAdviceBuilders (x4));
    }

    private bool CheckAllowsMultiple (ICustomAttributeData x, IEnumerable<ICustomAttributeData> datas)
    {
      return _customAttributeDataHelper.AllowsMultiple (x) || datas.All (y => y.Constructor.DeclaringType != x.Constructor.DeclaringType);
    }

    private bool CheckInheriting<T> (T baseMember, T currentMember, ICustomAttributeData customAttributeData)
    {
      return _customAttributeDataHelper.IsInheriting (customAttributeData) || currentMember.Equals (baseMember);
    }
  }
}