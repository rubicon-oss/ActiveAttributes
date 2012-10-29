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
using System.Collections.ObjectModel;
using System.Linq;
using ActiveAttributes.Core.Assembly.Done;
using ActiveAttributes.Core.Configuration2;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectDescriptorContainer : IAspectDescriptorContainer
  {
    private readonly IFieldWrapper _instanceField;
    private readonly IFieldWrapper _staticField;

    private readonly ReadOnlyCollection<IAspectDescriptor> _instanceAspects;
    private readonly ReadOnlyCollection<IAspectDescriptor> _staticAspects;

    private readonly ReadOnlyDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> _aspectStorageInfo;

    public AspectDescriptorContainer (IEnumerable<IAspectDescriptor> aspectDescriptors, IFieldWrapper instanceField, IFieldWrapper staticField)
    {
      ArgumentUtility.CheckNotNull ("aspectDescriptors", aspectDescriptors);
      ArgumentUtility.CheckNotNull ("instanceField", instanceField);
      ArgumentUtility.CheckNotNull ("staticField", staticField);
      Assertion.IsFalse (instanceField.Field.IsStatic);
      Assertion.IsTrue (staticField.Field.IsStatic);

      _instanceField = instanceField;
      _staticField = staticField;

      var aspectDescriptorsAsCollection = aspectDescriptors.ConvertToCollection();
      _instanceAspects = aspectDescriptorsAsCollection.Where (x => x.Scope == Scope.Instance).ToList().AsReadOnly();
      _staticAspects = aspectDescriptorsAsCollection.Where (x => x.Scope == Scope.Static).ToList().AsReadOnly();

      var instanceStorageInfo = _instanceAspects.Select ((x, i) => new { Descriptor = x, Info = Tuple.Create (instanceField, i) });
      var staticStorageInfo = _staticAspects.Select ((x, i) => new { Descriptor = x, Info = Tuple.Create (staticField, i) });
      _aspectStorageInfo = instanceStorageInfo.Concat (staticStorageInfo).ToDictionary (x => x.Descriptor, x => x.Info).AsReadOnly();
    }

    public IFieldWrapper InstanceField
    {
      get { return _instanceField; }
    }

    public IFieldWrapper StaticField
    {
      get { return _staticField; }
    }

    public ReadOnlyCollection<IAspectDescriptor> InstanceAspects
    {
      get { return _instanceAspects; }
    }

    public ReadOnlyCollection<IAspectDescriptor> StaticAspects
    {
      get { return _staticAspects; }
    }

    public ReadOnlyDictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>> AspectStorageInfo
    {
      get { return _aspectStorageInfo; }
    }
  }
}