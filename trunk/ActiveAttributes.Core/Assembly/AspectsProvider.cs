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
using ActiveAttributes.Core.Extensions;
using Remotion.FunctionalProgramming;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
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

    public IEnumerable<IAspectDescriptor> GetTypeLevelAspects (MutableType mutableType)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      var type = mutableType.UnderlyingSystemType;

      Func<MemberInfo, MemberInfo> getParent = memberInfo => ((Type) memberInfo).BaseType;
      Func<MemberInfo, bool> whileCondition = memberInfo => memberInfo != typeof (object);
      var fromType = GetAspects (type, getParent, whileCondition);

      var assemblies = GetAssemblies();
      var fromAssemblies = assemblies
          .SelectMany (CustomAttributeData.GetCustomAttributes)
          .Where (x => x.IsAspectAttribute())
          .Select (x => new AspectDescriptor (x))
          .Cast<IAspectDescriptor>();

      return fromType.Concat (fromAssemblies);
    }

    public IEnumerable<IAspectDescriptor> GetPropertyLevelAspects (MutableMethodInfo mutableMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      var method = mutableMethod.UnderlyingSystemMethodInfo;

      var propertyInfo = method.GetRelatedPropertyInfo();
      if (propertyInfo == null)
        return Enumerable.Empty<IAspectDescriptor>();
      else
      {
        Func<MemberInfo, MemberInfo> getParent = memberInfo => ((PropertyInfo) memberInfo).GetOverridenProperty();
        Func<MemberInfo, bool> whileCondition = memberInfo => memberInfo != null;
        return GetAspects (propertyInfo, getParent, whileCondition);
      }
    }

    public IEnumerable<IAspectDescriptor> GetMethodLevelAspects (MutableMethodInfo mutableMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      var method = mutableMethod.UnderlyingSystemMethodInfo;

      Func<MemberInfo, MemberInfo> getParent = memberInfo => _relatedMethodFinder.GetBaseMethod ((MethodInfo) memberInfo);
      Func<MemberInfo, bool> whileCondition = memberInfo => memberInfo != null;

      return GetAspects (method, getParent, whileCondition);
    }

    public IEnumerable<IAspectDescriptor> GetInterfaceLevelAspects (MutableMethodInfo mutableMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      var method = mutableMethod.UnderlyingSystemMethodInfo;

      var ifaces = method.DeclaringType.GetInterfaces();
      foreach (var iface in ifaces)
      {
        var map = method.DeclaringType.GetInterfaceMap (iface);
        var zipped = map.TargetMethods.Zip (map.InterfaceMethods, (TargetMember, InterfaceMember) => new { TargetMember, InterfaceMember });
        foreach (var item in zipped)
        {
          if (item.TargetMember != method)
            continue;

          return CustomAttributeData.GetCustomAttributes (item.InterfaceMember)
              .Where (x => x.IsAspectAttribute())
              .Select (x => new AspectDescriptor (x)).Cast<IAspectDescriptor>();
        }
      }
      return Enumerable.Empty<IAspectDescriptor>();
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

      return aspectsData.Select (x => new AspectDescriptor (x)).Cast<IAspectDescriptor>();
    }

    private IEnumerable<System.Reflection.Assembly> GetAssemblies ()
    {
      var assemblyLoader = new FilteringAssemblyLoader(new LoadAllAssemblyLoaderFilter());
      var assemblyFinder = new AssemblyFinder (SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (true, assemblyLoader), assemblyLoader);
      var assemblies = assemblyFinder.FindAssemblies ();
      return assemblies;
    }
  }
}