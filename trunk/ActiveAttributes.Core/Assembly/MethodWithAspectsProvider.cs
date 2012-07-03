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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Extensions;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  public class MethodWithAspectsProvider
  {
    public IEnumerable<Tuple<MutableMethodInfo, IEnumerable<AspectAttribute>>> GetMethodsWithAspects (MutableType mutableType)
    {
      foreach (var mutableMethod in mutableType.AllMutableMethods.ToArray())
      {
        var attributes = mutableMethod.GetCustomAttributes (typeof (MethodInterceptionAspectAttribute), true);
        var aspects = new List<AspectAttribute> (attributes.Cast<AspectAttribute>());

        if (mutableMethod.IsCompilerGenerated())
        {
          var propertyName = mutableMethod.Name.Substring (4);
          var propertyInfo = mutableType.UnderlyingSystemType.GetProperty (propertyName);
          // TODO
          //var attributes = CustomAttributeData.GetCustomAttributes (propertyInfo)
          //    .Where (cad => typeof (PropertyInterceptionAspectAttribute).IsAssignableFrom (cad.Constructor.DeclaringType));
          attributes = propertyInfo.GetCustomAttributes (typeof (PropertyInterceptionAspectAttribute), true);
          aspects.AddRange (attributes.Cast<AspectAttribute>());
        }

        if (aspects.Count > 0)
          yield return new Tuple<MutableMethodInfo, IEnumerable<AspectAttribute>> (mutableMethod, aspects);
      }
    }
  }

}