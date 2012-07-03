// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ActiveAttributes.Core.Aspects;

using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectsProvider
  {
    private readonly IRelatedMethodFinder _relatedMethodFinder;

    public AspectsProvider ()
    {
      _relatedMethodFinder = new RelatedMethodFinder();
    }

    // TODO: support aspects on interfaces?
    public IEnumerable<CompileTimeAspectBase> GetAspects (MethodInfo methodInfo)
    {
      // first level method aspects
      foreach (var compileTimeAspect in GetMethodLevelAspects (methodInfo, isBaseType: false))
        yield return compileTimeAspect;

      var declaringType = methodInfo.DeclaringType;
      var applyAspectsAttributes = declaringType.GetCustomAttributes (typeof (ApplyAspectAttribute), false).Cast<ApplyAspectAttribute>();
      foreach (var applyAspectsAttribute in applyAspectsAttributes)
      {
        yield return new TypeArgsCompileTimeAspect (applyAspectsAttribute.AspectType, applyAspectsAttribute.Arguments);
      }

      // inherited level method aspects
      while ((methodInfo = _relatedMethodFinder.GetBaseMethod (methodInfo)) != null)
      {
        foreach (var compileTimeAspect in GetMethodLevelAspects (methodInfo, isBaseType: true))
          yield return compileTimeAspect;
      }
    }

    private IEnumerable<CompileTimeAspectBase> GetMethodLevelAspects (MethodInfo methodInfo, bool isBaseType)
    {
      var customDatas = CustomAttributeData.GetCustomAttributes (methodInfo);
      if (isBaseType)
        customDatas = customDatas.Where (x => x.IsInheriting()).ToArray();

      return customDatas.Select (customData => new CustomDataCompileTimeAspect (customData)).Cast<CompileTimeAspectBase>();
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