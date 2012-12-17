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
using Remotion.Utilities;

namespace ActiveAttributes.Model
{
  public class MemberImport : AspectElementBase
  {
    private readonly FieldInfo _field;
    private readonly string _name;
    private readonly bool _isRequired;

    public MemberImport (FieldInfo field, string name, bool isRequired, Aspect aspect)
      : base (aspect)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("aspect", aspect);

      _field = field;
      _name = name;
      _isRequired = isRequired;
    }

    public FieldInfo Field
    {
      get { return _field; }
    }

    public Type Type
    {
      get { return _field.FieldType; }
    }

    public string Name
    {
      get { return _name; }
    }

    public bool IsRequired
    {
      get { return _isRequired; }
    }
  }
}