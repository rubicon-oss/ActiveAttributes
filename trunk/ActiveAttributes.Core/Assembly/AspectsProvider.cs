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
using ActiveAttributes.Core.Assembly.Descriptors;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;
using Remotion.FunctionalProgramming;
using Remotion.Reflection.TypeDiscovery;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  ///   Provides all aspects (including inherited) applied to a method.
  /// </summary>
  public class AspectsProvider : IAspectsProvider
  {
    private readonly IRelatedMethodFinder _relatedMethodFinder;

    public AspectsProvider ()
    {
      _relatedMethodFinder = new RelatedMethodFinder();
    }

    public IEnumerable<IAspectDescriptor> GetTypeLevelAspects (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      Func<MemberInfo, MemberInfo> getParent = memberInfo => ((Type) memberInfo).BaseType;
      Func<MemberInfo, bool> whileCondition = memberInfo => memberInfo != typeof (object);

      return GetAspects (type, getParent, whileCondition);
    }

    public IEnumerable<IAspectDescriptor> GetPropertyLevelAspects (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);

      var propertyInfo = methodInfo.GetRelatedPropertyInfo();
      if (propertyInfo == null)
        return Enumerable.Empty<IAspectDescriptor>();
      else
      {
        Func<MemberInfo, MemberInfo> getParent = memberInfo => ((PropertyInfo) memberInfo).GetOverridenProperty();
        Func<MemberInfo, bool> whileCondition = memberInfo => memberInfo != null;
        return GetAspects (propertyInfo, getParent, whileCondition);
      }
    }

    public IEnumerable<IAspectDescriptor> GetMethodLevelAspects (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);

      Func<MemberInfo, MemberInfo> getParent = memberInfo => _relatedMethodFinder.GetBaseMethod ((MethodInfo) memberInfo);
      Func<MemberInfo, bool> whileCondition = memberInfo => memberInfo != null;

      return GetAspects (methodInfo, getParent, whileCondition);
    }

    public IEnumerable<IAspectDescriptor> GetParameterLevelAspects (MethodInfo methodInfo)
    {
      var parameters = methodInfo.GetParameters();
      var descriptors = parameters.Select (GetParameterLevelTypes).ToArray();
      return descriptors
          .SelectMany (x => x)
          .Distinct()
          .Select (x => new TypeDescriptor (x, AspectScope.Static, 0))
          .Cast<IAspectDescriptor>();
    }

    private IEnumerable<Type> GetParameterLevelTypes (ParameterInfo parameterInfo)
    {
      var parameterAttributes = parameterInfo.GetCustomAttributes (true);
      return parameterAttributes
          .Select (parameterAttribute => parameterAttribute.GetType().GetCustomAttributes (true))
          .SelectMany (typeAttributes => typeAttributes.OfType<ApplyAspectAttribute>().Select (x => x.AspectType));
    }

    public IEnumerable<IAspectDescriptor> GetInterfaceLevelAspects (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);

      var ifaces = methodInfo.DeclaringType.GetInterfaces ();
      foreach (var iface in ifaces)
      {
        var map = methodInfo.DeclaringType.GetInterfaceMap (iface);
        var zipped = map.TargetMethods.Zip (map.InterfaceMethods, (TargetMember, InterfaceMember) => new { TargetMember, InterfaceMember });
        foreach (var item in zipped.Where (item => item.TargetMember == methodInfo))
        {
          return CustomAttributeData.GetCustomAttributes (item.InterfaceMember)
              .Where (x => x.IsAspectAttribute())
              .Select (x => new CustomDataDescriptor (x)).Cast<IAspectDescriptor>();
        }
      }
      return Enumerable.Empty<IAspectDescriptor>();
    }

    public IEnumerable<IAspectDescriptor> GetEventLevelAspects (MethodInfo methodInfo)
    {
      throw new NotImplementedException();
    }

    private IEnumerable<IAspectDescriptor> GetAspects (
        MemberInfo member, Func<MemberInfo, MemberInfo> getParent, Func<MemberInfo, bool> whileCondition)
    {
      var aspectsData = new List<CustomAttributeData>();
      var it = member;

      while (whileCondition (it))
      {
        var isBaseType = it != member;
        var customAttributeDatas = CustomAttributeData.GetCustomAttributes (it)
            .Where (x => x.IsAspectAttribute())
            .Where (x => !isBaseType || x.IsInheriting());

        aspectsData.AddRange (customAttributeDatas);

        it = getParent (it);
      }

      return aspectsData.Select (x => new CustomDataDescriptor (x)).Cast<IAspectDescriptor>();
    }
  }
}