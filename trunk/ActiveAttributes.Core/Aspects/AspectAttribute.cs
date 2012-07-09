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
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using ActiveAttributes.Core.Configuration;

namespace ActiveAttributes.Core.Aspects
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
  public abstract class AspectAttribute : Attribute, ISerializable
  {
    protected AspectAttribute (AspectScope scope = AspectScope.Static)
    {
      Scope = scope;
    }

    #region ISerializable members

    protected AspectAttribute (SerializationInfo info, StreamingContext context)
    {
      Scope = (AspectScope) info.GetInt32 ("scope");
      Priority = info.GetInt32 ("priority");
    }

    [SecurityPermission (SecurityAction.Demand, SerializationFormatter = true)]
    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("scope", (int) Scope);
      info.AddValue ("priority", Priority);
    }

    #endregion

    #region IClonable members

    public object Clone ()
    {
      return MemberwiseClone();
    }

    #endregion

    public AspectScope Scope { get; set; }

    public int Priority { get; set; }

    public object If { get; set; }

    public virtual bool Validate (MethodInfo method)
    {
      return true;
    }
  }
}