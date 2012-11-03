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
using System.Linq;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Infrastructure;

namespace ActiveAttributes.Core.Discovery
{
  public interface IAdviceMerger
  {
    Advice Merge (Advice advice1, Advice advice2);
  }

  public class AdviceMerger : IAdviceMerger
  {
    public Advice Merge (Advice advice1, Advice advice2)
    {
      if (advice1.Priority != 0 && advice2.Priority != 0)
        throw new Exception ("TODO");

      if (!advice1.Name.IsNullOrEmpty() && !advice2.Name.IsNullOrEmpty())
        throw new Exception ("TODO");

      if (!advice1.Role.IsNullOrEmpty() && !advice2.Role.IsNullOrEmpty())
        throw new Exception ("TODO");

      if (advice1.Execution != 0 && advice2.Execution != 0)
        throw new Exception ("TODO");

      if (advice1.Scope != 0 && advice2.Scope != 0)
        throw new Exception ("TODO");

      var method = advice1.Method ?? advice2.Method;
      var scope = advice1.Scope != 0 ? advice1.Scope : advice2.Scope;
      var execution = advice1.Execution != 0 ? advice1.Execution : advice2.Execution;
      var priority = advice1.Priority != 0 ? advice1.Priority : advice2.Priority;
      var name = !advice1.Name.IsNullOrEmpty() ? advice1.Name : advice2.Name;
      var role = !advice1.Role.IsNullOrEmpty() ? advice1.Role : advice2.Role;

      var pointcuts = advice1.Pointcuts.Concat (advice2.Pointcuts).ToList();

      if (pointcuts.Distinct (x => x.GetType()).Count() != pointcuts.Count)
        throw new Exception ("TODO");

      return new Advice (execution, scope, priority, method, pointcuts, role, name);
    }
  }
}