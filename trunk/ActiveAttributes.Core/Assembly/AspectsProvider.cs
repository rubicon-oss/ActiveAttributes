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
using ActiveAttributes.Core.Extensions;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

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

    public IEnumerable<IAspectDescriptor> GetAssemblyLevelAspects (System.Reflection.Assembly assembly)
    {
      return CustomAttributeData.GetCustomAttributes (assembly)
          .Where (x => x.IsAspectAttribute())
          .Select (x => new AspectDescriptor (x))
          .Cast<IAspectDescriptor>();
    }

    public IEnumerable<IAspectDescriptor> GetTypeLevelAspects (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      Func<MemberInfo, MemberInfo> getParent = memberInfo => ((Type) memberInfo).BaseType;
      Func<MemberInfo, bool> whileCondition = memberInfo => memberInfo != typeof (object);

      return GetAspects (type, getParent, whileCondition);
    }

    public IEnumerable<IAspectDescriptor> GetPropertyLevelAspects (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      Func<MemberInfo, MemberInfo> getParent = memberInfo => ((PropertyInfo) memberInfo).GetOverridenProperty ();
      Func<MemberInfo, bool> whileCondition = memberInfo => memberInfo != null;

      return GetAspects (propertyInfo, getParent, whileCondition);
    }

    public IEnumerable<IAspectDescriptor> GetMethodLevelAspects (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      Func<MemberInfo, MemberInfo> getParent = memberInfo => _relatedMethodFinder.GetBaseMethod ((MethodInfo) memberInfo);
      Func<MemberInfo, bool> whileCondition = memberInfo => memberInfo != null;

      return GetAspects (method, getParent, whileCondition);
    }

    private IEnumerable<IAspectDescriptor> GetAspects (
        MemberInfo member, Func<MemberInfo, MemberInfo> getParent, Func<MemberInfo, bool> whileCondition)
    {
      var aspectsData = new List<CustomAttributeData>();
      var it = member;

      while (whileCondition(it))
      {
        var isBaseType = it != member;
        var customAttributeDatas = CustomAttributeData.GetCustomAttributes (it)
            .Where (x => x.IsAspectAttribute ())
            .Where (x => !isBaseType || x.IsInheriting ());
        aspectsData.AddRange (customAttributeDatas);

        it = getParent (it);
      }

      return aspectsData.Select (x => new AspectDescriptor (x)).Cast<IAspectDescriptor> ();
    }

    // TODO: support aspects on interfaces?
    public IEnumerable<IAspectDescriptor> GetAspects (MethodInfo methodInfo)
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
          .Select (x => new AspectDescriptor (x))
          .Cast<IAspectDescriptor>()
          .Where (x => x.Matches (methodInfo));
          //.Where (x => ShouldApply (x, methodInfo));


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




    private IEnumerable<CustomAttributeData> GetPropertyLevelAspects (MethodInfo methodInfo)
    {
      var propertyName = methodInfo.Name.Substring (4);
      var propertyInfo = methodInfo.DeclaringType.GetProperty (propertyName);

      if (propertyInfo != null)
        return CustomAttributeData.GetCustomAttributes (propertyInfo);
      else
        return Enumerable.Empty<CustomAttributeData>();
    }
  }
}