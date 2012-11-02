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
using ActiveAttributes.Core.Attributes.Aspects;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (AspectAttributeToPointcutConverter))]
  public interface IAspectAttributeToPointcutConverter
  {
    IEnumerable<IPointcut> GetPointcuts (AspectBaseAttribute aspectAttribute);
  }

  public class AspectAttributeToPointcutConverter : IAspectAttributeToPointcutConverter
  {
    public IEnumerable<IPointcut> GetPointcuts (AspectBaseAttribute aspectAttribute)
    {
      ArgumentUtility.CheckNotNull ("aspectAttribute", aspectAttribute);

      if (aspectAttribute.ApplyToType != null)
        yield return new TypePointcut (aspectAttribute.ApplyToType);

      if (aspectAttribute.ApplyToTypeName != null)
        yield return new TypeNamePointcut (aspectAttribute.ApplyToTypeName);

      if (aspectAttribute.MemberNameFilter != null)
        yield return new MemberNamePointcut (aspectAttribute.MemberNameFilter);
    }
  }
}