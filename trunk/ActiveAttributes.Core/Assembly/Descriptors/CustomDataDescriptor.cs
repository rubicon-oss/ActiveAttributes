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
using System.Text;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;

namespace ActiveAttributes.Core.Assembly.Descriptors
{
  public class CustomDataDescriptor : IAspectDescriptor
  {
    private readonly AspectAttribute _aspectAttribute;
    private readonly CustomAttributeData _customAttributeData;

    public CustomDataDescriptor (CustomAttributeData customAttributeData)
    {
      if (!typeof (AspectAttribute).IsAssignableFrom (customAttributeData.Constructor.DeclaringType))
        throw new ArgumentException ("CustomAttributeData must be from an AspectAttribute");

      _customAttributeData = customAttributeData;
      _aspectAttribute = (AspectAttribute) customAttributeData.CreateAttribute();
    }

    public int Priority
    {
      get { return _aspectAttribute.Priority; }
    }

    public AspectScope Scope
    {
      get { return _aspectAttribute.Scope; }
    }

    public Type AspectType
    {
      get { return _aspectAttribute.GetType(); }
    }

    public ConstructorInfo ConstructorInfo
    {
      get { return _customAttributeData.Constructor; }
    }

    public IList<CustomAttributeTypedArgument> ConstructorArguments
    {
      get { return _customAttributeData.ConstructorArguments; }
    }

    public IList<CustomAttributeNamedArgument> NamedArguments
    {
      get { return _customAttributeData.NamedArguments; }
    }

    public bool Matches (MethodInfo method)
    {
      return _aspectAttribute.Matches (method);
    }

    public override string ToString ()
    {
      var stringBuilder = new StringBuilder();
      stringBuilder.Append (_aspectAttribute.GetType().Name)
          .Append ("(");

      stringBuilder.Append (string.Join (", ", _customAttributeData.ConstructorArguments.Select (x => "{" + x.Value + "}").ToArray()));

      if (_customAttributeData.ConstructorArguments.Count > 0)
        stringBuilder.Append (", ");

      stringBuilder.Append (
          string.Join (", ", _customAttributeData.NamedArguments.Select (x => x.MemberInfo.Name + " = {" + x.TypedValue.Value + "}").ToArray()));

      if (_customAttributeData.NamedArguments.Count > 0)
        stringBuilder.Append (", ");

      stringBuilder.Append ("Scope = ").Append (Scope);

      stringBuilder.Append (")");

      return stringBuilder.ToString();
    }
  }
}