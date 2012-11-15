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
using System.Reflection;
using ActiveAttributes.Advices;
using ActiveAttributes.Declaration;
using ActiveAttributes.Declaration.Construction;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother
  {
    public static IAdviceBuilder GetAdviceBuilder (
        IConstruction construction = null,
        MethodInfo method = null,
        AdviceExecution execution = AdviceExecution.Before,
        AdviceScope scope = AdviceScope.Instance)
    {
      construction = construction ?? GetConstruction();
      method = method ?? GetMethodInfo();
      return new AdviceBuilder()
          .SetConstruction (construction)
          .SetMethod (method)
          .SetExecution (execution)
          .SetScope (scope);
    }
  }
}