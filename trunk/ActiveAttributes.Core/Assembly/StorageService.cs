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
using ActiveAttributes.Assembly.Storages;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Assembly
{
  /// <summary>Serves as a service for introduction of <see cref="IStorage"/>s.</summary>
  [ConcreteImplementation (typeof (StorageService))]
  public interface IStorageService
  {
    /// <summary>Introduces a <see cref="StaticStorage"/> for a given mutable type with a certain storage type and name.</summary>
    IStorage AddStaticStorage (MutableType mutableType, Type type, string name);

    /// <summary>Introduces a <see cref="InstanceStorage"/> for a given mutable type with a certain storage type and name.</summary>
    IStorage AddInstanceStorage (MutableType mutableType, Type type, string name);

    /// <summary>Introduces a <see cref="GlobalStorage"/> with a certain storage type.</summary>
    IStorage AddGlobalStorage (Type type);
  }

  public class StorageService : IStorageService
  {
    private readonly Dictionary<Guid, object> _globalStorages = new Dictionary<Guid, object>();

    private int _counter;

    public IStorage AddStaticStorage (MutableType mutableType, Type type, string name)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      var field = mutableType.AddField (name + _counter++, type, FieldAttributes.Private | FieldAttributes.Static);
      return new StaticStorage (field);
    }

    public IStorage AddInstanceStorage (MutableType mutableType, Type type, string name)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      var field = mutableType.AddField (name + _counter++, type);
      return new InstanceStorage (field);
    }

    public IStorage AddGlobalStorage (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return new GlobalStorage (_globalStorages, type);
    }

  }
}