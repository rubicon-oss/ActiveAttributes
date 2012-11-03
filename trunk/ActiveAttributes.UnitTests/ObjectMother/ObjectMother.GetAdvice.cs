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
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using ActiveAttributes.Core.Interception.Invocations;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother2
  {
    public static Advice GetAdvice (
        MethodInfo method = null,
        string role = null,
        string name = null,
        IEnumerable<IPointcut> pointcuts = null,
        int priority = 0,
        AdviceExecution execution = 0,
        AdviceScope scope = 0)
    {
      method = method ?? GetMethodInfo();
      pointcuts = pointcuts ?? new IPointcut[0];

      return new Advice (execution, scope, priority, method, pointcuts, role, name);
    }

    public static Advice GetInvocationAdvice ()
    {
      var method = GetMethodInfo (parameterTypes: new[] { typeof (IInvocation) });
      return GetAdvice (method);
    }
  }
}