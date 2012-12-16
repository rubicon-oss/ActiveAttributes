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
using System.Reflection;
using ActiveAttributes.Annotations;
using ActiveAttributes.Extensions;
using System.Linq;
using ActiveAttributes.Infrastructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Discovery
{
  [ConcreteImplementation (typeof(AspectElementBuilder))]
  public interface IAspectElementBuilder
  {
    void AddAdvices (Aspect aspect, ICollection<Advice> advices);
    void AddMemberIntroductions (Aspect aspect, ICollection<MemberIntroduction> introductions);
    void AddMemberImports (Aspect aspect, ICollection<MemberImport> imports);
  }

  public class AspectElementBuilder : IAspectElementBuilder
  {
    private readonly IPointcutBuilder _pointcutBuilder;

    public AspectElementBuilder (IPointcutBuilder pointcutBuilder)
    {
      ArgumentUtility.CheckNotNull ("pointcutBuilder", pointcutBuilder);

      _pointcutBuilder = pointcutBuilder;
    }

    public void AddAdvices (Aspect aspect, ICollection<Advice> advices)
    {
      ArgumentUtility.CheckNotNull ("aspect", aspect);
      ArgumentUtility.CheckNotNull ("advices", advices);

      foreach (var method in aspect.Type.GetMethods())
      {
        var adviceAttribute = method.GetCustomAttributes<AdviceAttribute> (true).SingleOrDefault();
        if (adviceAttribute == null)
          continue;

        var execution = adviceAttribute.Execution;
        var pointcut = _pointcutBuilder.GetPointcut (method);

        advices.Add (new Advice (method, execution, aspect, pointcut));
      }
    }

    public void AddMemberIntroductions (Aspect aspect, ICollection<MemberIntroduction> introductions)
    {
      ArgumentUtility.CheckNotNull ("aspect", aspect);
      ArgumentUtility.CheckNotNull ("introductions", introductions);

      foreach (var method in aspect.Type.GetMethods())
        TryAddMemberIntroduction(aspect, introductions, method);
      foreach (var property in aspect.Type.GetProperties())
        TryAddMemberIntroduction(aspect, introductions, property);
      foreach (var event_ in aspect.Type.GetEvents())
        TryAddMemberIntroduction(aspect, introductions, event_);

      // TODO check all members of interfaces
    }

    public void AddMemberImports (Aspect aspect, ICollection<MemberImport> imports)
    {
      ArgumentUtility.CheckNotNull ("aspect", aspect);
      ArgumentUtility.CheckNotNull ("imports", imports);

      foreach (var field in aspect.Type.GetFields())
      {
        var importAttribute = field.GetCustomAttributes<ImportMemberAttribute> (true).SingleOrDefault ();
        if (importAttribute == null)
          continue;

        if (!typeof (Delegate).IsAssignableFrom (field.FieldType))
          throw new Exception (string.Format ("Field '{0}' must be of a delegate type", field.Name));

        var memberName = importAttribute.MemberName;
        var isRequired = importAttribute.IsRequired;

        imports.Add (new MemberImport (memberName, isRequired, aspect));
      }
    }

    private void TryAddMemberIntroduction (Aspect aspect, ICollection<MemberIntroduction> introductions, MemberInfo member)
    {
      var introduceAttribute = member.GetCustomAttributes<IntroduceMemberAttribute> (true).SingleOrDefault();
      if (introduceAttribute != null)
      {
        var conflictAction = introduceAttribute.ConflictAction;
        introductions.Add (new MemberIntroduction (member, conflictAction, aspect));
      }
    }
  }
}