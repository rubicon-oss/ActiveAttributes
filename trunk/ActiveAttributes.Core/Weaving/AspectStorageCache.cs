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
using ActiveAttributes.Model;
using ActiveAttributes.Weaving.Storage;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (AspectStorageCache), Lifetime = LifetimeKind.Singleton)]
  public interface IAspectStorageCache
  {
    IStorage GetOrAddStorage (Aspect aspect, JoinPoint joinPoint);
  }

  public class AspectStorageCache : IAspectStorageCache
  {
    private readonly Dictionary<object, IStorage> _storageDictionary = new Dictionary<object, IStorage>();

    private readonly IAspectStorageBuilder _aspectStorageBuilder;
    private readonly IAspectCacheKeyProvider _aspectCacheKeyProvider;

    public AspectStorageCache (IAspectStorageBuilder aspectStorageBuilder, IAspectCacheKeyProvider aspectCacheKeyProvider)
    {
      _aspectStorageBuilder = aspectStorageBuilder;
      _aspectCacheKeyProvider = aspectCacheKeyProvider;
    }

    public IStorage GetOrAddStorage (Aspect aspect, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("aspect", aspect);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      var cacheKey = _aspectCacheKeyProvider.CreateCacheKey (aspect, joinPoint);
      if (cacheKey == null)
        return _aspectStorageBuilder.CreateStorage (aspect, joinPoint);

      IStorage storage;
      if (!_storageDictionary.TryGetValue (cacheKey, out storage))
      {
        storage = _aspectStorageBuilder.CreateStorage (aspect, joinPoint);
        _storageDictionary.Add (cacheKey, storage);
      }

      return storage;
    }
  }
}