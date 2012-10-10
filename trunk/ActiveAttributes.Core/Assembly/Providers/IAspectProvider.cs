// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System.Collections.Generic;
using System.Reflection;

namespace ActiveAttributes.Core.Assembly.Providers
{
  /// <summary>
  /// Provides aspects applied to a given target.
  /// </summary>
  public interface IAspectProvider
  {
    IEnumerable<IAspectDescriptor> GetAspects (MemberInfo member);
  }
}