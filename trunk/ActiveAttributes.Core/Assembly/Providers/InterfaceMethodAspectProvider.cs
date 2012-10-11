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
using System.Linq;
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly.Providers
{
  public class InterfaceMethodAspectProvider : IMethodLevelAspectProvider
  {
    public IEnumerable<IAspectDescriptor> GetAspects (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      Assertion.IsNotNull (method.DeclaringType);

      var declaringType = method.DeclaringType;
      var interfaces = declaringType.GetInterfaces();
      var interfaceMappings = interfaces.Select (declaringType.GetInterfaceMap);
      var targetInterfaceZip = interfaceMappings.SelectMany (
          x =>
          x.TargetMethods.Zip (
              x.InterfaceMethods, (targetMember, interfaceMember) => new { TargetMember = targetMember, InterfaceMember = interfaceMember }));

      var interfaceMembers = targetInterfaceZip.Where (x => x.TargetMember == method).Select (x => x.InterfaceMember).Cast<MemberInfo>();
      // TODO remove dummy
      var interfaceMembersWithDummmy = new[] { MethodInfo.GetCurrentMethod() }.Concat (interfaceMembers);
      return AspectProvider.GetAspects (null, interfaceMembersWithDummmy);
    }
  }
}