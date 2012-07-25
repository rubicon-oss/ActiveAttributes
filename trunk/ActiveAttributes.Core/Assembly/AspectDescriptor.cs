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
using System.Text.RegularExpressions;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectDescriptor : IAspectDescriptor
  {
    private readonly AspectAttribute _attribute;
    private readonly CustomAttributeData _customData;

    public AspectDescriptor (CustomAttributeData customData)
    {
      if (!typeof (AspectAttribute).IsAssignableFrom (customData.Constructor.DeclaringType))
        throw new ArgumentException ("CustomAttributeData must be from an AspectAttribute");

      _customData = customData;
      _attribute = (AspectAttribute) customData.CreateAttribute();
    }

    public int Priority
    {
      get { return _attribute.Priority; }
    }

    public AspectScope Scope
    {
      get { return _attribute.Scope; }
    }

    public Type AspectType
    {
      get { return _attribute.GetType(); }
    }

    public ConstructorInfo ConstructorInfo
    {
      get { return _customData.Constructor; }
    }
    public IList<CustomAttributeTypedArgument> ConstructorArguments
    {
      get { return _customData.ConstructorArguments; }
    }
    public IList<CustomAttributeNamedArgument> NamedArguments
    {
      get { return _customData.NamedArguments; }
    }

    public bool Matches (MethodInfo method)
    {
      if (_attribute.IfType != null && !MatchesType (_attribute.IfType, method))
        return false;
      if (_attribute.IfSignature != null && !MatchesSignature (_attribute.IfSignature, method))
        return false;

      return true;
    }

    private bool MatchesSignature (object signature, MethodInfo method)
    {
      if (signature is string)
      {
        var input = SignatureDebugStringGenerator.GetMethodSignature (method);
        var pattern = ConvertToPattern ((string) signature);
        var isMatch = Regex.IsMatch (input, pattern);
        return isMatch;
      }
      else
      {
        return false;
      }
    }

    private bool MatchesType (object type, MethodInfo method)
    {
      if (type is string)
      {
        var input = method.DeclaringType.FullName;
        var pattern = ConvertToPattern ((string) type);
        var isMatch = Regex.IsMatch (input, pattern);
        return isMatch;
      }
      else
      {
        return type == method.DeclaringType;
      }
    }

    private static string ConvertToPattern (string input)
    {
      return "^" +
             input
                 .Replace (".", "\\.")
                 .Replace ("+", "\\+")
                 .Replace ("*", ".*")
                 .Replace ("(", "\\(")
                 .Replace (")", "\\)")
                 .Replace ("void", "Void")
             + "$";
    }

    public override string ToString ()
    {
      var stringBuilder = new StringBuilder();
      stringBuilder.Append (_attribute.GetType().Name)
          .Append ("(");

      stringBuilder.Append (string.Join (", ", _customData.ConstructorArguments.Select (x => "{" + x.Value + "}").ToArray()));

      if (_customData.ConstructorArguments.Count > 0)
        stringBuilder.Append (", ");

      stringBuilder.Append (string.Join (", ", _customData.NamedArguments.Select (x => x.MemberInfo.Name + " = {" + x.TypedValue.Value + "}").ToArray()));

      if (_customData.NamedArguments.Count > 0)
        stringBuilder.Append (", ");

      stringBuilder.Append ("Scope = ").Append (Scope);

      stringBuilder.Append (")");

      return stringBuilder.ToString();
    }
  }
}