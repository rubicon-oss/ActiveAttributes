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
using Microsoft.Scripting.Ast;

namespace ActiveAttributes.Core.Assembly.Storages
{
  public class GlobalStorage : IStorage
  {
    private readonly Expression _storageExpression;

    public GlobalStorage (IDictionary<Guid, object> dictionary, Type type)
    {
      var guid = Guid.NewGuid();
      var indexExpression = Expression.Property (Expression.Constant (dictionary), "Item", Expression.Constant (guid));
      _storageExpression = Expression.Convert (indexExpression, type);

      var assignExpression = Expression.Assign (indexExpression, Expression.Convert (Expression.Default (type), typeof (object)));
      var lambda = Expression.Lambda<Action> (assignExpression);
      lambda.Compile()();
    }

    public FieldInfo Field { get; private set; }

    public Expression GetStorageExpression (Expression thisExpression)
    {
      return _storageExpression;
    }
  }
}