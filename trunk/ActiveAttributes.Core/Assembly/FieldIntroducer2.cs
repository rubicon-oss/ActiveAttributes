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
using ActiveAttributes.Core.Assembly.FieldWrapper;
using ActiveAttributes.Core.Extensions;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (FieldIntroducer2))]
  public interface IFieldIntroducer2
  {
    IFieldWrapper AddField (MutableType mutableType, Type type, string name, FieldAttributes attributes);
  }

  public class FieldIntroducer2 : IFieldIntroducer2
  {
    public IFieldWrapper AddField (MutableType mutableType, Type type, string name, FieldAttributes attributes)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      var field = mutableType.AddField (type, name, attributes);
      return attributes.HasFlags (FieldAttributes.Static)
                 ? (IFieldWrapper) new StaticFieldWrapper (field)
                 : new InstanceFieldWrapper (field);
    }
  }
}