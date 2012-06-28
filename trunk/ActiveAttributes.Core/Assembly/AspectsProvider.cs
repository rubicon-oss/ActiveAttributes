// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectsProvider
  {
    private IRelatedMethodFinder _relatedMethodFinder;

    public AspectsProvider ()
    {
      _relatedMethodFinder = new RelatedMethodFinder();
    }

    // TODO: support aspects on interfaces?
    public IEnumerable<CompileTimeAspect> GetAspects (MethodInfo methodInfo)
    {
      do
      {
        var customDatas = CustomAttributeData.GetCustomAttributes (methodInfo);

        foreach (var customData in customDatas.Where(x => x.IsInheriting()))
          yield return new CompileTimeAspect (customData);
      } while ((methodInfo = _relatedMethodFinder.GetBaseMethod (methodInfo)) != null);
    }
  }

  public static class CustomAttributeDataExtensions
  {
    public static bool IsInheriting (this CustomAttributeData customAttributeData)
    {
      var attributeType = customAttributeData.Constructor.DeclaringType;
      var attributeUsageAttr = attributeType.GetCustomAttributes (typeof (AttributeUsageAttribute), true).Cast<AttributeUsageAttribute>().Single();
      return attributeUsageAttr.Inherited;
    }
  }
}