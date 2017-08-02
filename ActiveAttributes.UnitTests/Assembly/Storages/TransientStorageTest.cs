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
using ActiveAttributes.Weaving.Storage;
using Microsoft.Scripting.Ast;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Assembly.Storages
{
  [TestFixture]
  public class TransientStorageTest
  {
    //[Test]
    //public void CreateStorageExpression ()
    //{
    //  var newExpression = Expression.New (typeof (object));
    //  var storage = new TransientStorage (newExpression);

    //  var result = storage.CreateStorageExpression (null);

    //  var lambda = Expression.Lambda<Func<object>> (result).Compile();
    //  var obj1 = lambda();
    //  var obj2 = lambda();

    //  Assert.That (obj1, Is.Not.EqualTo (obj2));
    //}
  }
}