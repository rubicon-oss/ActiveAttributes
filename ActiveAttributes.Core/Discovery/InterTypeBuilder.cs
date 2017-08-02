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
using ActiveAttributes.Aspects;
using ActiveAttributes.Extensions;
using ActiveAttributes.Model;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Discovery
{
  [ConcreteImplementation (typeof (InterTypeBuilder))]
  public interface IInterTypeBuilder
  {
    IEnumerable<MemberIntroduction> AddMemberIntroductions (Aspect aspect);
    IEnumerable<MemberImport> AddMemberImports (Aspect aspect);
  }

  public class InterTypeBuilder : IInterTypeBuilder
  {

    public IEnumerable<MemberIntroduction> AddMemberIntroductions (Aspect aspect)
    {
      ArgumentUtility.CheckNotNull ("aspect", aspect);

      var methods = BuildIntroductions (aspect, aspect.Type.GetMethods ());
      var properties = BuildIntroductions (aspect, aspect.Type.GetProperties ());
      var events = BuildIntroductions (aspect, aspect.Type.GetEvents ());

      // TODO check all members of interfaces

      return methods.Concat (properties).Concat (events);
    }

    public IEnumerable<MemberImport> AddMemberImports (Aspect aspect)
    {
      ArgumentUtility.CheckNotNull ("aspect", aspect);

      foreach (var field in aspect.Type.GetFields ())
      {
        var importAttribute = field.GetCustomAttributes<ImportMemberAttribute> (true).SingleOrDefault ();
        if (importAttribute == null)
          continue;

        if (!typeof (Delegate).IsAssignableFrom (field.FieldType))
          throw new Exception (string.Format ("Field '{0}' must be of a delegate type", field.Name));

        var memberName = importAttribute.MemberName;
        var isRequired = importAttribute.IsRequired;

        yield return new MemberImport (field, memberName, isRequired, aspect);
      }
    }

    private IEnumerable<MemberIntroduction> BuildIntroductions (Aspect aspect, IEnumerable<MemberInfo> members)
    {
      foreach (var member in members)
      {
        var introduceAttribute = member.GetCustomAttributes<IntroduceMemberAttribute> (true).SingleOrDefault ();
        if (introduceAttribute != null)
        {
          var conflictAction = introduceAttribute.ConflictAction;
          yield return new MemberIntroduction (member, conflictAction, aspect);
        }
      }
    }
  }
}