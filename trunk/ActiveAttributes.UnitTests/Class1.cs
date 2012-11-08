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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.UnitTests
{
  [TestFixture]
  public class Class1
  {
    [Test]
    public void name ()
    {
      var s = "";
      var param = Expression.Variable (typeof (string));
      var methodInfo = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => Method (out s));
      var expr = Expression.Call (Expression.Constant (this), methodInfo, param);
      var block = Expression.Block (
          new[] { param },
          expr,
          param);
      var lambda = Expression.Lambda<Func<string>> (block).Compile();
      var x = lambda();
      Assert.That (x, Is.EqualTo ("muH"));
    }

    private void Method (out string t)
    {
      t = "muH";
    }
  }
}