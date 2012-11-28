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
using System.Linq;
using ActiveAttributes.Aspects;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using ActiveAttributes.Extensions;

namespace ActiveAttributes.Utilities
{
  /// <summary>Serves as a helper for <see cref="ICustomAttributeData"/>s.</summary>
  [ConcreteImplementation (typeof (CustomAttributeDataHelper))]
  public interface ICustomAttributeDataHelper
  {
    /// <summary>Determines if the attribute is inheriting by checking the its <see cref="AttributeUsageAttribute"/>.</summary>
    bool IsInheriting (ICustomAttributeData customAttributeData);

    /// <summary>Determines if the attribute allows multiple use by checking the its <see cref="AttributeUsageAttribute"/>.</summary>
    bool AllowsMultiple (ICustomAttributeData customAttributeData);

    /// <summary>Determines if the attribute is an aspect attribute deriving from <see cref="AspectAttributeBase"/>.</summary>
    bool IsAspectAttribute (ICustomAttributeData customAttribute);
  }

  public class CustomAttributeDataHelper : ICustomAttributeDataHelper
  {
    public bool IsInheriting (ICustomAttributeData customAttributeData)
    {
      ArgumentUtility.CheckNotNull ("customAttributeData", customAttributeData);

      return GetAttributeUsage (customAttributeData).Inherited;
    }

    public bool AllowsMultiple (ICustomAttributeData customAttributeData)
    {
      ArgumentUtility.CheckNotNull ("customAttributeData", customAttributeData);

      return GetAttributeUsage (customAttributeData).AllowMultiple;
    }

    public bool IsAspectAttribute (ICustomAttributeData customAttribute)
    {
      ArgumentUtility.CheckNotNull ("customAttribute", customAttribute);

      return typeof (AspectAttributeBase).IsAssignableFrom (customAttribute.Constructor.DeclaringType);
    }

    private AttributeUsageAttribute GetAttributeUsage (ICustomAttributeData customAttributeData)
    {
      var attributeType = customAttributeData.Constructor.DeclaringType;
      return attributeType.GetCustomAttributes<AttributeUsageAttribute> (inherit: true).Single();
    }
  }
}