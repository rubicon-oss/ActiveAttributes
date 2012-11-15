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
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother
  {
    public static MethodBaseBodyContextBase GetBodyContextBase (
        MutableType declaringType = null, IEnumerable<ParameterExpression> parameterExpressions = null, bool isStatic = false)
    {
      declaringType = declaringType ?? GetMutableType();
      parameterExpressions = parameterExpressions ?? new ParameterExpression[0];
      var memberSelector = new MemberSelector (new BindingFlagsEvaluator());

      return MockRepository.GenerateStub<MethodBaseBodyContextBase> (declaringType, parameterExpressions, isStatic, memberSelector);
    }

  }
}