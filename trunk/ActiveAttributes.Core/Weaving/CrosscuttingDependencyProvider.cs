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
using ActiveAttributes.Extensions;
using ActiveAttributes.Model;
using ActiveAttributes.Model.Ordering;
using Remotion.Collections;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Weaving
{
  public class CrosscuttingDependencyProvider
  {
    public IEnumerable<Tuple<T, T>> GetDependencies<T> (IEnumerable<T> crosscuttings) where T : class, ICrosscutting
    {
      var crosscuttingAsList = crosscuttings.ConvertToCollection ();
      var orderings = crosscuttingAsList.SelectMany (x => x.Orderings).ConvertToCollection ();

      var dependencies = from crosscutting1 in crosscuttingAsList
                         from crosscutting2 in crosscuttingAsList
                         where crosscutting1 != crosscutting2
                         from ordering in orderings
                         where ordering.Accept (this, crosscutting1, crosscutting2)
                         select Tuple.Create (crosscutting1, crosscutting2);

      return new HashSet<Tuple<T, T>> (dependencies, EqualityComparer<Tuple<T, T>>.Default);
    }

    public bool VisitAspectType (AspectTypeOrdering ordering, ICrosscutting crosscutting1, ICrosscutting crosscutting2)
    {
      return ordering.BeforeType.IsAssignableFrom (crosscutting1.Type) && ordering.AfterType.IsAssignableFrom (crosscutting2.Type);
    }

    public bool VisitAspectRole (AspectRoleOrdering ordering, ICrosscutting crosscutting1, ICrosscutting crosscutting2)
    {
      return crosscutting1.Role.IsMatchWildcard (ordering.BeforeRole) && crosscutting2.Role.IsMatchWildcard (ordering.AfterRole);
    }

    public bool VisitAdviceName (AdviceNameOrdering ordering, ICrosscutting crosscutting1, ICrosscutting crosscutting2)
    {
      return crosscutting1.Name == ordering.BeforeAdvice && crosscutting2.Name == ordering.AfterAdvice;
    }
  }
}