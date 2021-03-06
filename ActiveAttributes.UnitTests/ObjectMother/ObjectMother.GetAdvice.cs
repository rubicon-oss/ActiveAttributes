﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using ActiveAttributes.Advices;
using ActiveAttributes.Pointcuts;
using ActiveAttributes.Weaving.Construction;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother
  {
    public static Advice GetAdvice (
        IAspectConstruction construction = null,
        MethodInfo method = null,
        string role = null,
        string name = null,
        AdviceExecution execution = AdviceExecution.Around,
        AdviceScope scope = AdviceScope.Static,
        int priority = 0,
        IEnumerable<IPointcut> pointcuts = null)
    {
      construction = construction ?? GetConstruction();
      method = method ?? GetMethodInfo();
      name = name ?? "name";
      role = role ?? "role";
      pointcuts = pointcuts ?? new IPointcut[0];

      return new Advice (construction, method, name, role, execution, scope, priority, pointcuts);
    }
  }
}