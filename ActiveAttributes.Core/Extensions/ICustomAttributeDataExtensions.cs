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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Aspects;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Extensions
{
  public static class ICustomAttributeDataExtensions
  {
    /// <summary>
    /// Converts a <see cref="CustomAttributeTypedArgument"/> using an <paramref name="arrayConstructor"/> for array values and a
    /// <paramref name="elementConstructor"/> for element values.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="argument">The <see cref="CustomAttributeTypedArgument"/></param>
    /// <param name="elementConstructor">A delegate creating a new value based on the value and its type.</param>
    /// <param name="arrayConstructor">A delegate creating a new array based on its type and values.</param>
    /// <returns>An instance of <typeparam name="T"/> using the element and array constructor.</returns>
    public static T ConvertTo<T> (
        this ICustomAttributeNamedArgument argument, Func<Type, object, T> elementConstructor, Func<Type, IEnumerable<T>, T> arrayConstructor)
    {
      ArgumentUtility.CheckNotNull ("argument", argument);
      ArgumentUtility.CheckNotNull ("elementConstructor", elementConstructor);
      ArgumentUtility.CheckNotNull ("arrayConstructor", arrayConstructor);

      var argumentType = argument.MemberType;
      if (!argumentType.IsArray)
        return elementConstructor (argumentType, argument.Value);
      else
      {
        var elementType = argumentType.GetElementType();
        var objects = (IEnumerable<object>) argument.Value;
        return arrayConstructor (elementType, objects.Select (x => elementConstructor (elementType, x)));
      }
    }

    /// <summary>
    /// Determines if the <see cref="CustomAttributeData"/> describes an <see cref="AspectAttribute"/>.
    /// </summary>
    public static bool IsAspectAttribute (this ICustomAttributeData customAttributeData)
    {
      return typeof (AspectAttributeBase).IsAssignableFrom (customAttributeData.Constructor.DeclaringType);
    }

    /// <summary>
    /// Determines if the <see cref="CustomAttributeData"/> is inheriting.
    /// </summary>
    public static bool IsInheriting (this ICustomAttributeData customAttributeData)
    {
      ArgumentUtility.CheckNotNull ("customAttributeData", customAttributeData);
      Assertion.IsTrue (customAttributeData.Constructor.DeclaringType != null);

      var attributeType = customAttributeData.Constructor.DeclaringType;
      var attributeUsageAttribute = attributeType.GetCustomAttributes<AttributeUsageAttribute> (inherit: true).Single();
      return attributeUsageAttribute.Inherited;
    }

    /// <summary>
    /// Determines if the <see cref="CustomAttributeData"/> allows multiple use.
    /// </summary>
    public static bool AllowsMultiple (this ICustomAttributeData customAttributeData)
    {
      ArgumentUtility.CheckNotNull ("customAttributeData", customAttributeData);
      Assertion.IsTrue (customAttributeData.Constructor.DeclaringType != null);

      var attributeType = customAttributeData.Constructor.DeclaringType;
      var attributeUsageAttribute = attributeType.GetCustomAttributes<AttributeUsageAttribute> (inherit: true).Single();
      return attributeUsageAttribute.AllowMultiple;
    }

    public static T CreateAttribute<T> (this ICustomAttributeData customAttributeData) where T : Attribute
    {
      ArgumentUtility.CheckNotNull ("customAttributeData", customAttributeData);

      return (T) CreateAttribute (customAttributeData);
    }

    /// <summary>
    /// Creates the actual <see cref="Attribute"/> described by an instance of <see cref="CustomAttributeData"/>.
    /// </summary>
    /// <param name="customAttributeData">The data describing the attribute.</param>
    /// <returns>The attribute.</returns>
    public static Attribute CreateAttribute (this ICustomAttributeData customAttributeData)
    {
      ArgumentUtility.CheckNotNull ("customAttributeData", customAttributeData);
      Assertion.IsTrue (customAttributeData.NamedArguments != null);

      var arguments = customAttributeData.ConstructorArguments.ToArray();
      var attribute = (Attribute) customAttributeData.Constructor.Invoke (arguments);

      foreach (var namedArgument in customAttributeData.NamedArguments)
      {
        var argument = ConvertTypedArgumentToObject (namedArgument);
        SetMemberValue (namedArgument.MemberInfo, attribute, argument);
      }

      return attribute;
    }

    private static void SetMemberValue (MemberInfo member, object target, object value)
    {
      var propertyInfo = member as PropertyInfo;
      if (propertyInfo != null)
        propertyInfo.SetValue (target, value, null);

      var fieldInfo = member as FieldInfo;
      if (fieldInfo != null)
        fieldInfo.SetValue (target, value);
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
      var source = objs.ToArray();
      var destination = Array.CreateInstance (type, source.Length);
      Array.Copy (source, destination, source.Length);
      return destination;
    }
  }
}