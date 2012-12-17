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
using ActiveAttributes.Aspects.Ordering;
using ActiveAttributes.Extensions;
using ActiveAttributes.Model;
using ActiveAttributes.Model.Ordering;
using Remotion.ServiceLocation;

namespace ActiveAttributes.Discovery
{
  [ConcreteImplementation (typeof (OrderingBuilder))]
  public interface IOrderingBuilder
  {
    IEnumerable<IOrdering> BuildOrderings (ICrosscutting crosscutting, ICustomAttributeProvider customAttributeProvider);
  }

  public class OrderingBuilder : IOrderingBuilder
  {
    private const string c_source = "OrderingBuilder";

    public IEnumerable<IOrdering> BuildOrderings (ICrosscutting crosscutting, ICustomAttributeProvider customAttributeProvider)
    {
      return customAttributeProvider.GetCustomAttributes<OrderingAttributeBase> (true).SelectMany (x => Build (crosscutting, x));
    }

    public virtual IEnumerable<IOrdering> Build (ICrosscutting crosscutting, OrderingAttributeBase ordering)
    {
      var position = ordering.Position;

      var typeOrdering = ordering as AspectTypeOrderingAttribute;
      if (typeOrdering != null)
        return Build (crosscutting.Type, typeOrdering.Aspects, position, (before, after) => new AspectTypeOrdering (before, after, c_source));

      var roleOrdering = ordering as AspectRoleOrderingAttribute;
      if (roleOrdering != null)
        return Build (crosscutting.Role, roleOrdering.Aspects, position, (before, after) => new AspectRoleOrdering (before, after, c_source));

      var nameOrdering = ordering as AdviceNameOrderingAttribute;
      if (nameOrdering != null)
        return Build (crosscutting.Name, nameOrdering.Advices, position, (before, after) => new AdviceNameOrdering (before, after, c_source));

      throw new Exception();
    }

    private IEnumerable<IOrdering> Build<T> (T this_, IEnumerable<T> others, Position position, Func<T, T, IOrdering> factory)
    {
      Func<T, T> before = other => position == Position.Before ? this_ : other;
      Func<T, T> after = other => position == Position.Before ? other : this_;
      return others.Select (x => factory (before (x), after (x)));
    }
  }
}