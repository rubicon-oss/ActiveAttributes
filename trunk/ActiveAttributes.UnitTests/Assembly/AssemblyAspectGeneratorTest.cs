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
using System.Linq;
using ActiveAttributes.Core;
using ActiveAttributes.Core.Assembly;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AssemblyAspectGeneratorTest
  {
    [Test]
    public void GetStorageExpression ()
    {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      var descriptor = MockRepository.GenerateMock<IAspectDescriptor>();
      var generator = new AssemblyAspectGenerator (assembly, 0, descriptor);

      var propertyInfo = typeof (AssemblyAspectManager).GetProperties().Single();
      var expected =
          Expression.ArrayAccess (
              Expression.Property (
                  Expression.Property (null, propertyInfo),
                  "Item",
                  Expression.Constant (assembly)),
              Expression.Constant (0));

      var result = generator.GetStorageExpression (null);

      ExpressionTreeComparer.CheckAreEqualTrees (expected, result);
    }
  }
}