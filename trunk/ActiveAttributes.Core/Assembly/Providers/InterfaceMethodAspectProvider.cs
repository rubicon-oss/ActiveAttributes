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
    public IEnumerable<IAspectDescriptor> GetAspects (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      Assertion.IsTrue (member.DeclaringType != null);

      var declaringType = member.DeclaringType;
      var interfaces = declaringType.GetInterfaces();
      var interfaceMappings = interfaces.Select (declaringType.GetInterfaceMap);
      var targetInterfaceZip = interfaceMappings.SelectMany (
          x =>
          x.TargetMethods.Zip (
              x.InterfaceMethods, (targetMember, interfaceMember) => new { TargetMember = targetMember, InterfaceMember = interfaceMember }));

      var interfaceMembers = targetInterfaceZip.Where (x => x.TargetMember == member).Select (x => x.InterfaceMember).Cast<MemberInfo>();
      var interfaceMembersWithDummmy = new[] { MethodInfo.GetCurrentMethod() }.Concat (interfaceMembers);
      return AspectProvider.GetAspects (null, interfaceMembersWithDummmy);
    }
  }
}