// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;

namespace ActiveAttributes.Core.Assembly
{
  public class CompileTimeAspect
  {
    private readonly CustomAttributeData _customData;

    public CompileTimeAspect (CustomAttributeData customData)
    {
      if (!typeof (AspectAttribute).IsAssignableFrom (customData.Constructor.DeclaringType))
        throw new ArgumentException ("CustomAttributeData must be from an AspectAttribute");

      _customData = customData;
    }

    public int Priority
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

    public AspectScope Scope
    {
      get
      {
        var priorityArgument = _customData.NamedArguments.SingleOrDefault (x => x.MemberInfo.Name == "Scope");
        return priorityArgument.MemberInfo != null
                   ? (AspectScope) priorityArgument.TypedValue.Value
                   : 0;
      }
    }

    public Type AspectType
    {
      get { return _customData.Constructor.DeclaringType; }
    }

    public ConstructorInfo ConstructorInfo
    {
      get { return _customData.Constructor; }
    }
    public IList<CustomAttributeTypedArgument> ConstructorArguments
    {
      get { return _customData.ConstructorArguments; }
    }
    public IList<CustomAttributeNamedArgument> NamedArguments
    {
      get { return _customData.NamedArguments; }
    }
  }
}