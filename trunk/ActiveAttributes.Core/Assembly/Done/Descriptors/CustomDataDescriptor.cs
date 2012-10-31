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
using System.Reflection;
using System.Text;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Extensions;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly.Descriptors
{
  /// <summary>
  ///   Serves as a <see cref="IAspectDescriptor" /> based on <see cref="ICustomAttributeData" />.
  /// </summary>
  public class CustomAttributeDataAspectDescriptor : IAspectDescriptor
  {
    private readonly AspectAttribute _aspectAttribute;
    private readonly ICustomAttributeData _customAttributeData;

    public CustomAttributeDataAspectDescriptor (ICustomAttributeData customAttributeData)
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

    public Scope Scope
    {
      get { return _aspectAttribute.Scope; }
    }

    public Type Type
    {
      get { return _aspectAttribute.GetType(); }
    }

    public ConstructorInfo ConstructorInfo
    {
      get { return _customAttributeData.Constructor; }
    }

    public ReadOnlyCollection<object> ConstructorArguments
    {
      get { return _customAttributeData.ConstructorArguments; }
    }

    public ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> NamedArguments
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

      stringBuilder
          .Append (_aspectAttribute.GetType().Name)
          .Append ("(");

      var namedWithoutScopeAndPriority = _customAttributeData.NamedArguments
          .Where (x => x.MemberInfo.Name != "Scope" && x.MemberInfo.Name != "Priority");

      var arguments = new List<string>();
      arguments.AddRange (_customAttributeData.ConstructorArguments.Select (x => "{" + x.ToString() + "}"));
      arguments.Add ("Scope = " + Scope);
      arguments.Add ("Priority = " + Priority);
      arguments.AddRange (namedWithoutScopeAndPriority.Select (x => x.MemberInfo.Name + " = {" + x.Value + "}"));
      arguments.ForEach (x => stringBuilder.Append (x).Append (", "));

      stringBuilder.Remove (stringBuilder.Length - 2, 2);
      stringBuilder.Append (")");

      return stringBuilder.ToString();
    }
  }
}