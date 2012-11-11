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
using Microsoft.Scripting.Ast;
using Remotion.Utilities;

namespace ActiveAttributes.Assembly.Storages
{
  /// <summary>
  /// Generates an expression that provides access to a static field.
  /// </summary>
  public class StaticStorage : IStorage
  {
    private readonly FieldInfo _field;

    public StaticStorage (FieldInfo field)
    {
      ArgumentUtility.CheckNotNull ("field", field);
      Assertion.IsTrue (field.IsStatic);
      
      _field = field;
    }

    public FieldInfo Field
    {
      get { return _field; }
    }

    public Expression GetStorageExpression (Expression thisExpression)
    {
      return Expression.Field (null, _field);
    }

    public bool IsStatic
    {
      get { return true; }
    }
  }
}