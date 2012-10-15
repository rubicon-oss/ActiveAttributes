﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using ActiveAttributes.Core.Aspects;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Extensions
{
  public static class ICustomAttributeDataExtensions
  {
    /// <summary>
    /// Determines if the <see cref="CustomAttributeData"/> is inheriting.
    /// </summary>
    public static bool IsInheriting (this ICustomAttributeData customAttributeData)
    {
      ArgumentUtility.CheckNotNull ("customAttributeData", customAttributeData);
      Assertion.IsTrue (customAttributeData.Constructor.DeclaringType != null);

      var attributeType = customAttributeData.Constructor.DeclaringType;
      var attributeUsageAttr = attributeType.GetCustomAttributes (typeof (AttributeUsageAttribute), true).Cast<AttributeUsageAttribute> ().Single ();
      return attributeUsageAttr.Inherited;
    }

    /// <summary>
    /// Determines if the <see cref="CustomAttributeData"/> describes an <see cref="AspectAttribute"/>.
    /// </summary>
    public static bool IsAspectAttribute (this ICustomAttributeData customAttributeData)
    {
      return typeof (AspectAttribute).IsAssignableFrom (customAttributeData.Constructor.DeclaringType);
    }

    /// <summary>
    /// Creates the actual <see cref="Attribute"/> described by an instance of <see cref="CustomAttributeData"/>.
    /// </summary>
    /// <param name="data">The data describing the attribute.</param>
    /// <returns>The attribute.</returns>
    public static Attribute CreateAttribute (this ICustomAttributeData data)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      Assertion.IsTrue (data.NamedArguments != null);

      Func<ICustomAttributeNamedArgument, object> argumentConverter = ConvertTypedArgumentToObject;

      var arguments = data.ConstructorArguments;
      var attribute = (Attribute) data.Constructor.Invoke (arguments.ToArray ());

      foreach (var namedArgument in data.NamedArguments)
      {
        var argument = ConvertTypedArgumentToObject (namedArgument);

        var memberInfo = namedArgument.MemberInfo;

        var propertyInfo = memberInfo as PropertyInfo;
        if (propertyInfo != null)
          propertyInfo.SetValue (attribute, argument, null);

        var fieldInfo = memberInfo as FieldInfo;
        if (fieldInfo != null)
          fieldInfo.SetValue (attribute, argument);
      }

      return attribute;
    }
    /// <summary>
    /// Converts a <see cref="CustomAttributeTypedArgument"/> using an <paramref name="arrayConstructor"/> for array values and a
    /// <paramref name="elementConstructor"/> for element values.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="argument">The <see cref="CustomAttributeTypedArgument"/></param>
    /// <param name="elementConstructor">A delegate creating a new value based on the value and its type.</param>
    /// <param name="arrayConstructor">A delegate creating a new array based on its type and values.</param>
    /// <returns>``0.</returns>
    public static T ConvertTo<T> (
        this ICustomAttributeNamedArgument argument, Func<Type, object, T> elementConstructor, Func<Type, IEnumerable<T>, T> arrayConstructor)
    {
      var argumentType = argument.MemberType;
      if (!argumentType.IsArray)
      {
        return elementConstructor (argumentType, argument.Value);
      }
      else
      {
        var typedArguments = (ReadOnlyCollection<ICustomAttributeNamedArgument>) argument.Value;
        var elements = typedArguments.Select (x => x.ConvertTo (elementConstructor, arrayConstructor));
        return arrayConstructor (argumentType.GetElementType (), elements);
      }
    }

    private static object ConvertTypedArgumentToObject (ICustomAttributeNamedArgument typedArgument)
    {
      return typedArgument.ConvertTo (CreateElement, CreateArray);
    }

    private static object CreateElement (Type type, object obj)
    {
      return obj;
    }

    private static object CreateArray (Type type, IEnumerable<object> objs)
    {
      var source = objs.ToArray ();
      var destination = Array.CreateInstance (type, source.Length);
      Array.Copy (source, destination, source.Length);
      return destination;
    }
  }
}