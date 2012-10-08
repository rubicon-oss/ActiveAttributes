// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System.Collections.Generic;
using System.Reflection;

namespace ActiveAttributes.Core.Assembly.Providers
{
  public interface IAspectProvider2
  {
    IEnumerable<IAspectDescriptor> GetAspects (MemberInfo member);
  }
}