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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ActiveAttributes.Core.Assembly;
using Remotion.Collections;

namespace ActiveAttributes.Core.Configuration
{
  public interface IAspectConfiguration
  {
    ReadOnlyCollection<IComparer<IAspectDescriptor>> Rules { get; }

    void AddRule (IComparer<IAspectDescriptor> rule);
    void RemoveRule (IComparer<IAspectDescriptor> rule);

    ReadOnlyDictionary<Type, string> Roles { get; }

    void SetRole (string name);
    void UnsetRole (string name);
  }

  public interface IOrderRule : IComparer<IAspectDescriptor>
  {
  }

  public class AspectConfiguration : IAspectConfiguration
  {
    private readonly IList<IComparer<IAspectDescriptor>> _rules;
    private readonly IDictionary<Type, string> _roles;

    public AspectConfiguration ()
    {
      _rules = new List<IComparer<IAspectDescriptor>>();
      _roles = new Dictionary<Type, string>();
    }

    public ReadOnlyCollection<IComparer<IAspectDescriptor>> Rules
    {
      get { return new ReadOnlyCollection<IComparer<IAspectDescriptor>> (_rules); }
    }

    public void AddRule (IComparer<IAspectDescriptor> rule)
    {
    }

    public void RemoveRule (IComparer<IAspectDescriptor> rule)
    {
    }

    public ReadOnlyDictionary<Type, string> Roles
    {
      get { return new ReadOnlyDictionary<Type, string> (_roles); }
    }

    public void SetRole (string name)
    {
    }

    public void UnsetRole (string name)
    {
    }
  }
}