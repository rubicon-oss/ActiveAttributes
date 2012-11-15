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
using ActiveAttributes.Advices;
using ActiveAttributes.Extensions;
using ActiveAttributes.Ordering.Providers;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Ordering
{
  /// <summary>Serves as a provider for dependencies between <see cref="Advice"/>s.</summary>
  [ConcreteImplementation (typeof (AdviceDependencyProvider))]
  public interface IAdviceDependencyProvider
  {
    /// <summary>
    /// Creates a collection of dependencies defined through <see cref="Tuple{Advice, Advice}"/> where the first advice must precede before the second.
    /// Calucation of dependencies will be dispatched first to the <see cref="IAdviceOrdering"/> and then back to the provider.
    /// </summary>
    IEnumerable<Tuple<Advice, Advice>> GetDependencies (IEnumerable<Advice> advices);

    bool DependsType (Advice advice1, Advice advice2, AdviceTypeOrdering ordering);
    bool DependsRole (Advice advice1, Advice advice2, AdviceRoleOrdering ordering);
  }

  public class AdviceDependencyProvider : IAdviceDependencyProvider
  {
    private readonly IEnumerable<IAdviceOrderingProvider> _orderingProviders;

    public AdviceDependencyProvider (IEnumerable<IAdviceOrderingProvider> orderingProviders)
    {
      ArgumentUtility.CheckNotNull ("orderingProviders", orderingProviders);

      _orderingProviders = orderingProviders;
    }

    public IEnumerable<Tuple<Advice, Advice>> GetDependencies (IEnumerable<Advice> advices)
    {
      ArgumentUtility.CheckNotNull ("advices", advices);

      var advicesAsCollection = advices.ConvertToCollection();
      var orderings = _orderingProviders.SelectMany (x => x.GetOrderings ()).ConvertToCollection ();

      var dependencies = from advice1 in advicesAsCollection
                         from advice2 in advicesAsCollection
                         where advice1 != advice2
                         from ordering in orderings
                         where ordering.DependVisit (this, advice1, advice2)
                         select Tuple.Create (advice1, advice2);

      return new HashSet<Tuple<Advice, Advice>> (dependencies);
    }

    public bool DependsType (Advice advice1, Advice advice2, AdviceTypeOrdering ordering)
    {
      return ordering.BeforeType.IsAssignableFrom (advice1.DeclaringType) && ordering.AfterType.IsAssignableFrom (advice2.DeclaringType);
    }

    public bool DependsRole (Advice advice1, Advice advice2, AdviceRoleOrdering ordering)
    {
      return advice1.Role.IsMatchWildcard (ordering.BeforeRole) && advice2.Role.IsMatchWildcard (ordering.AfterRole);
    }
  }
}