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
// 
using System;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.Utilities;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class InstanceAspectExpressionGeneratorTest
  {
    [Test]
    public void GetStorageExpression ()
    {
      var field = MemberInfoFromExpressionUtility.GetField (((DomainType obj) => obj.AspectField));
      var descriptorStub = MockRepository.GenerateStub<IAspectAttributeDescriptor>();
      var generator = new InstanceAspectExpressionGenerator (field, 1, descriptorStub);
      var thisExpression = Expression.Constant (new DomainType());

      var actual = generator.GetStorageExpression (thisExpression);
      var expected = Expression.ArrayAccess (Expression.Field (thisExpression, field), Expression.Constant (1));

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    public class DomainType
    {
      public AspectAttribute[] AspectField;
    }
  }
}