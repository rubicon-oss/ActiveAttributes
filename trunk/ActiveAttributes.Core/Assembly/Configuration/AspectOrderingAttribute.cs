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

namespace ActiveAttributes.Core.Assembly.Configuration
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = true)]
  public class AspectOrderingAttribute : Attribute
  {
    public AspectOrderingAttribute (OrderPosition position, params Type[] aspectTypes)
    {
      Position = position;
      AspectTypes = aspectTypes;
    }

    public AspectOrderingAttribute (OrderPosition position, params string[] aspectRoles)
    {
      Position = position;
      AspectRoles = aspectRoles;
    }

    public OrderPosition Position { get; private set; }

    public Type[] AspectTypes { get; private set; }
    public string[] AspectRoles { get; private set; }
  }
}