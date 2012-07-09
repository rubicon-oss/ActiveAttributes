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
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;

namespace ActiveAttributes.Core.Assembly.CompileTimeAspects
{
  public class CustomDataCompileTimeAspect : CompileTimeAspectBase
  {
    private readonly CustomAttributeData _customData;

    public CustomDataCompileTimeAspect (CustomAttributeData customData)
    {
      if (!typeof (AspectAttribute).IsAssignableFrom (customData.Constructor.DeclaringType))
        throw new ArgumentException ("CustomAttributeData must be from an AspectAttribute");

      _customData = customData;
    }

    public override CompileTimeAspectType CompileTimeType
    {
      get { return CompileTimeAspectType.CustomDataCompileTimeAspect; }
    }

    public override int Priority
    {
      get
      {
        // TODO: review
        var priorityArgument = _customData.NamedArguments.SingleOrDefault (x => x.MemberInfo.Name == "Priority");
        return priorityArgument.MemberInfo != null
                   ? (int) priorityArgument.TypedValue.Value
                   : 0;
      }
    }

    public override AspectScope Scope
    {
      get
      {
        var scopeArgument = _customData.NamedArguments.SingleOrDefault (x => x.MemberInfo.Name == "Scope");
        return scopeArgument.MemberInfo != null
                   ? (AspectScope) scopeArgument.TypedValue.Value
                   : 0;
      }
    }

    public override Type AspectType
    {
      get { return _customData.Constructor.DeclaringType; }
    }

    public override ConstructorInfo ConstructorInfo
    {
      get { return _customData.Constructor; }
    }
    public override IList<CustomAttributeTypedArgument> ConstructorArguments
    {
      get { return _customData.ConstructorArguments; }
    }
    public override IList<CustomAttributeNamedArgument> NamedArguments
    {
      get { return _customData.NamedArguments; }
    }

    public override object[] Arguments
    {
      get { throw new NotSupportedException(); }
    }

    public override object If
    {
      get
      {
        var ifArgument = _customData.NamedArguments.SingleOrDefault (x => x.MemberInfo.Name == "If");
        return ifArgument.MemberInfo != null
                   ? ifArgument.TypedValue.Value
                   : null;
      }
    }
  }
}