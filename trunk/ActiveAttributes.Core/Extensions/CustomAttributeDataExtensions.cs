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
using System.Linq;
using System.Reflection;

namespace ActiveAttributes.Core.Extensions
{
  public static class CustomAttributeDataExtensions
  {
    /// <summary>
    /// Creates the actual <see cref="Attribute"/> described by a <see cref="CustomAttributeData"/> object.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Attribute CreateAttribute (this CustomAttributeData data)
    {
      var argumentConverter = new Func<CustomAttributeTypedArgument, object> (ConvertTypedArgumentToObject);

      var arguments = data.ConstructorArguments.Select (argumentConverter);
      var attribute = (Attribute) data.Constructor.Invoke (arguments.ToArray());

      foreach (var namedArgument in data.NamedArguments)
      {
        var typedArgument = namedArgument.TypedValue;
        var argument = ConvertTypedArgumentToObject (typedArgument);

        var memberInfo = namedArgument.MemberInfo;

        if (memberInfo is PropertyInfo)
          (((PropertyInfo) memberInfo)).SetValue (attribute, argument, null);
        if (memberInfo is FieldInfo)
          (((FieldInfo) memberInfo)).SetValue (attribute, argument);
      }

      return attribute;
    }

    public static T ConvertTo<T> (
        this CustomAttributeTypedArgument argument, Func<Type, object, T> elementConstructor, Func<Type, IEnumerable<T>, T> arrayConstructor)
    {
      var argumentType = argument.ArgumentType;
      if (!argumentType.IsArray)
      {
        return elementConstructor (argumentType, argument.Value);
      }
      else
      {
        var typedArguments = (ReadOnlyCollection<CustomAttributeTypedArgument>) argument.Value;
        var elements = typedArguments.Select (x => x.ConvertTo (elementConstructor, arrayConstructor));
        return arrayConstructor (argumentType.GetElementType (), elements);
      }
    }

    private static object ConvertTypedArgumentToObject (CustomAttributeTypedArgument typedArgument)
    {
      return typedArgument.ConvertTo (CreateElement, CreateArray);
    }

    private static object CreateElement (Type type, object obj)
    {
      return obj;
    }

    private static object CreateArray (Type type, IEnumerable<object> objs)
    {
      var source = objs.ToArray();
      var destination = Array.CreateInstance (type, source.Length);
      Array.Copy (source, destination, source.Length);
      return destination;
    }
  }
}