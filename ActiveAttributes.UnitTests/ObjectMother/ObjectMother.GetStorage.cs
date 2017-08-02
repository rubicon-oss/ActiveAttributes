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
using ActiveAttributes.Weaving.Storage;
using Microsoft.Scripting.Ast;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother
  {
    public static IStorage GetStorage (Type type = null, FieldAttributes attributes = FieldAttributes.Private)
    {
      var expression = GetExpression (type);

      var stub = MockRepository.GenerateStub<IStorage>();

      stub.Stub (x => x.CreateStorageExpression (Arg<Expression>.Is.Anything)).Return (expression);

      return stub;
    }
  }
}