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
using ActiveAttributes.Core.Assembly.Configuration;
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

      var arguments = _customAttributeData.ConstructorArguments;
      var argumentsString = string.Join (", ", arguments.Select (x => "\"" + x.Value + "\"").ToArray());
      stringBuilder.Append (argumentsString);
      if (arguments.Count > 0)
        stringBuilder.Append (", ");

      var namedArguments = _customAttributeData.NamedArguments.Where (x => x.MemberInfo.Name != "Scope" && x.MemberInfo.Name != "Priority").ToList();
      var namedArgumentsString = string.Join (", ", namedArguments.Select (x => x.MemberInfo.Name + " = \"" + x.TypedValue.Value + "\"").ToArray());
      stringBuilder.Append (namedArgumentsString);
      if (namedArguments.Count > 0)
        stringBuilder.Append (", ");

      stringBuilder
          .Append ("Scope = ")
          .Append (Scope).Append (", ")
          .Append ("Priority = ")
          .Append (Priority);

      stringBuilder.Append (")");

      return stringBuilder.ToString();
    }
  }
}