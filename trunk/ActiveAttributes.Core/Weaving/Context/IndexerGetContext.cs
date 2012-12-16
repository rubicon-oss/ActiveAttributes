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

namespace ActiveAttributes.Weaving.Context
{
  public class IndexerGetContext<TInstance, TIndex, TValue> : FuncContext<TInstance, TIndex, TValue>
  {
    private readonly PropertyInfo _propertyInfo;

    public IndexerGetContext (PropertyInfo propertyInfo, TInstance instance, TIndex index)
        : base (propertyInfo, instance, index)
    {
      _propertyInfo = propertyInfo;
    }

    public object Index
    {
      get { return this[0]; }
      set { this[0] = value; }
    }

    public object Value
    {
      get { return ReturnValue; }
      set { ReturnValue = value; }
    }

    public bool IsIndexer
    {
      get { return true; }
    }

    public new PropertyInfo MemberInfo
    {
      get { return _propertyInfo; }
    }
  }
}