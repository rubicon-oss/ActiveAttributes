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
using ActiveAttributes.Core.Assembly.FieldWrappers;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.UnitTests.Expressions;

namespace ActiveAttributes.UnitTests.Assembly.FieldWrappers
{
  [TestFixture]
  public class StaticFieldWrapperTest
  {
    private static int _field;

    [Test]
    public void Initialization ()
    {
      var field = NormalizingMemberInfoFromExpressionUtility.GetField (() => _field);

      var fieldWrapper = new StaticFieldWrapper (field);

      Assert.That (fieldWrapper.Field, Is.SameAs (field));
    }

    [Test]
    public void GetAccessExpression ()
    {
      var field = NormalizingMemberInfoFromExpressionUtility.GetField (() => _field);

      var fieldWrapper = new StaticFieldWrapper (field);

      var expected = Expression.Field (null, field);
      var actual = fieldWrapper.GetAccessExpression (null);
      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }
  }
}