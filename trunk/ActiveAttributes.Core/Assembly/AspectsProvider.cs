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
using System.Text.RegularExpressions;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.CompileTimeAspects;
using ActiveAttributes.Core.Extensions;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Provides all aspects (including inherited) applied to a method.
  /// </summary>
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
      // TODO RM-4942, RM-4943: When custom attribute support is added to the type pipe, omit the "UnderlyingSystemMethodInfo"
      if (methodInfo is MutableMethodInfo)
        methodInfo = ((MutableMethodInfo) methodInfo).UnderlyingSystemMethodInfo;

      var customAttributeDatas = new List<CustomAttributeData> ();
      var iteratingMethodInfo = methodInfo;
      do
      {
        var isBaseType = !iteratingMethodInfo.Equals(methodInfo);
        customAttributeDatas.AddRange (GetAspects (iteratingMethodInfo, isBaseType));
      } while ((iteratingMethodInfo = _relatedMethodFinder.GetBaseMethod (iteratingMethodInfo)) != null);

      var aspects = customAttributeDatas
          .Select (x => new CustomDataCompileTimeAspect (x))
          .Cast<CompileTimeAspectBase>()
          .Where (x => ShouldApply (x, methodInfo));

      return aspects;
    }

    private IEnumerable<CustomAttributeData> GetAspects (MethodInfo methodInfo, bool isBaseType)
    {
      var customAttributeDatas = new List<CustomAttributeData> ();

      var methodLevelAspects = CustomAttributeData.GetCustomAttributes (methodInfo);
      customAttributeDatas.AddRange (methodLevelAspects);

      if (methodInfo.IsCompilerGenerated ())
      {
        var propertyLevelAspects = GetPropertyLevelAspects(methodInfo);
        customAttributeDatas.AddRange (propertyLevelAspects);
      }

      var typeLevelAspects = CustomAttributeData.GetCustomAttributes (methodInfo.DeclaringType);
      customAttributeDatas.AddRange (typeLevelAspects);

      var assemblyLevelAspects = CustomAttributeData.GetCustomAttributes (methodInfo.DeclaringType.Assembly);
      customAttributeDatas.AddRange (assemblyLevelAspects);

      return customAttributeDatas
          .Where (x => typeof (AspectAttribute).IsAssignableFrom (x.Constructor.DeclaringType))
          .Where (x => !isBaseType || x.IsInheriting());
    }

    private bool ShouldApply (CompileTimeAspectBase aspect, MethodInfo methodInfo)
    {
      if (aspect.IfType != null && !ShouldApplyOnType (aspect.IfType, methodInfo))
        return false;

      if (aspect.IfSignature != null && !ShouldApplyOnSignature (aspect.IfSignature, methodInfo))
        return false;

      return true;
    }

    private bool ShouldApplyOnSignature (object signature, MethodInfo methodInfo)
    {
      if (signature is string)
      {
        var input = SignatureDebugStringGenerator.GetMethodSignature (methodInfo);
        var pattern = ConvertToPattern ((string) signature);
        var isMatch = Regex.IsMatch (input, pattern);
        return isMatch;
      }
      else
      {
        return false;
      }
    }

    private bool ShouldApplyOnType (object type, MethodInfo methodInfo)
    {
      if (type is string)
      {
        var input = methodInfo.DeclaringType.FullName;
        var pattern = ConvertToPattern ((string) type);
        var isMatch = Regex.IsMatch (input, pattern);
        return isMatch;
      }
      else
      {
        return type == methodInfo.DeclaringType;
      }
    }

    private static string ConvertToPattern (string input)
    {
      return "^" +
             input
                 .Replace (".", "\\.")
                 .Replace ("+", "\\+")
                 .Replace ("*", ".*")
                 .Replace ("(", "\\(")
                 .Replace (")", "\\)")
                 .Replace ("void", "Void")
             + "$";
    }

    private IEnumerable<CustomAttributeData> GetPropertyLevelAspects (MethodInfo methodInfo)
    {
      var propertyName = methodInfo.Name.Substring (4);
      var propertyInfo = methodInfo.DeclaringType.GetProperty (propertyName);

      if (propertyInfo != null)
      {
        var customDatas = CustomAttributeData.GetCustomAttributes (propertyInfo);
        return customDatas;
      }
      else
      {
        return Enumerable.Empty<CustomAttributeData>();
      }
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