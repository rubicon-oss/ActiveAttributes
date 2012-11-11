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
using ActiveAttributes.Assembly.Storages;
using Microsoft.Scripting.Ast;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Assembly.Storages
{
  [TestFixture]
  public class StaticStorageTest
  {
    [Test]
    public void GetStorageExpression ()
    {
      var field = ObjectMother.GetFieldInfo (attributes: FieldAttributes.Static);
      var storage = new StaticStorage (field);

      var result = storage.GetStorageExpression (null);

      Assert.That (result, Is.InstanceOf<MemberExpression>());
      var memberExpression = (MemberExpression) result;
      Assert.That (memberExpression.Member, Is.SameAs (field));
      Assert.That (memberExpression.Expression, Is.Null);
    }
  }
}