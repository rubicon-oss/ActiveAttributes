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
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.Utilities;

namespace ActiveAttributes.Weaving.Storage
{
  /// <summary>Creates expressions accessing an item in a global dictionary.</summary>
  public class SingletonStorage : IStorage
  {
    public static readonly Dictionary<string, object> Dictionary = new Dictionary<string, object>();
    private static readonly FieldInfo s_dictionaryFieldInfo = MemberInfoFromExpressionUtility.GetField (() => Dictionary);

    private readonly Expression _storageExpression;

    public SingletonStorage (string name, Type type, Expression initExpression)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var field = Expression.Field (null, s_dictionaryFieldInfo);
      var indexExpression = Expression.Property (field, "Item", Expression.Constant (name));
      _storageExpression = Expression.Convert (indexExpression, type);

      Expression.Lambda<Action> (Expression.Assign (indexExpression, initExpression)).Compile()();
    }

    public Expression CreateStorageExpression (Expression thisExpression)
    {
      return _storageExpression;
    }
  }
}