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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.FieldWrapper;
using Microsoft.Scripting.Ast;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public static class ObjectMother
  {
    private static readonly Random s_random;

    static ObjectMother ()
    {
      s_random = new Random();
    }

    public static Type GetType_ ()
    {
      // TODO create array of types and use random item
      return typeof (object);
    }

    public static Type GetInstantiatableType(Type type = null)
    {
      //if (type != null) Assertion.IsFalse (type.IsAbstract);

      return type ?? typeof (object);
    }

    public static Type GetAssignableType (Type assignableType, Type type = null)
    {
      if (type != null) Assertion.IsTrue (assignableType.IsAssignableFrom (type));

      return type ?? assignableType;
    }

    public static ThisExpression GetThisExpression (Type type = null)
    {
      return new ThisExpression (type ?? typeof (object));
    }

    public static BodyContextBase GetBodyContextBase (MutableType declaringType = null, IEnumerable<ParameterExpression> parameterExpressions = null)
    {
      declaringType = declaringType ?? GetMutableType();
      parameterExpressions = parameterExpressions ?? new ParameterExpression[0];
      var isStatic = false;
      var memberSelector = new MemberSelector (new BindingFlagsEvaluator());

      return MockRepository.GenerateStub<BodyContextBase> (declaringType, parameterExpressions, isStatic, memberSelector);
    }

    public static MutableType GetMutableType (Type type = null)
    {
      return MutableTypeObjectMother.CreateForExistingType (type);
    }

    public static FieldInfo GetFieldInfo (
        Type type = null, string name = null, FieldAttributes attributes = FieldAttributes.Private, Type declaringType = null)
    {
      type = GetInstantiatableType (type);
      name = name ?? "field";
      declaringType = declaringType ?? GetType_();

      var mock = MockRepository.GeneratePartialMock<FieldInfo>();

      mock
          .Stub (x => x.DeclaringType)
          .Return (declaringType);
      mock
          .Stub (x => x.Name)
          .Return (name);
      mock
          .Stub (x => x.FieldType)
          .Return (type);
      mock
          .Stub (x => x.Attributes)
          .Return (attributes);

      return mock;
    }

    public static PropertyInfo GetPropertyInfo (
        Type declaringType = null, string name = null, Type type = null, PropertyAttributes attributes = PropertyAttributes.None)
    {
      declaringType = declaringType ?? GetType_();
      name = name ?? "field";
      type = GetInstantiatableType (type);

      var mock = MockRepository.GeneratePartialMock<PropertyInfo>();

      mock
          .Stub (x => x.DeclaringType)
          .Return (declaringType);
      mock
          .Stub (x => x.Name)
          .Return (name);
      mock
          .Stub (x => x.PropertyType)
          .Return (type);
      mock
          .Stub (x => x.Attributes)
          .Return (attributes);

      return mock;
    }

    public static MethodInfo GetMethodInfo (Type declaringType = null,
                                            string name = null, Type returnType = null, IEnumerable<Type> parameterTypes = null, MethodAttributes attributes = MethodAttributes.Private)
    {
      declaringType = declaringType ?? GetType_();
      name = name ?? "Method";
      returnType = returnType ?? GetType_();
      parameterTypes = parameterTypes ?? GetMultiple (() => GetInstantiatableType(), s_random.Next (0, 5));

      var mock = MockRepository.GeneratePartialMock<MethodInfo>();

      mock
          .Stub (x => x.DeclaringType)
          .Return (declaringType);
      mock
          .Stub (x => x.Name)
          .Return (name);
      mock
          .Stub (x => x.Attributes)
          .Return (attributes);
      mock
          .Stub (x => x.ReturnType)
          .Return (returnType);
      mock
          .Stub (x => x.GetParameters ())
          .Return (parameterTypes.Select ((x, i) => GetParameterInfo (parameterType: x, position: i)).ToArray());

      return mock;
    }

    public static MemberExpression GetMemberExpression (Type type = null)
    {
      type = GetInstantiatableType (type);
      var field = GetFieldInfo (type, attributes: FieldAttributes.Static);

      return Expression.Field (null, field);
    }

    public static ParameterExpression GetParameterExpression (Type type = null, string name = null)
    {
      type = GetInstantiatableType (type);
      name = name ?? "parameter" + s_random.Next (0, 100);

      return Expression.Parameter (type, name);
    }

    public static IFieldWrapper GetFieldWrapper (Type type = null, FieldAttributes attributes = FieldAttributes.Private, Type declaringType = null, ThisExpression thisExpression = null)
    {
      var field = GetFieldInfo (type, attributes: attributes, declaringType: declaringType);
      thisExpression = thisExpression ?? GetThisExpression (field.DeclaringType);
      return GetFieldWrapper (field, thisExpression);
    }

    public static IFieldWrapper GetFieldWrapper (FieldInfo field, ThisExpression thisExpression = null)
    {
      thisExpression = field.IsStatic ? null : thisExpression ?? GetThisExpression (field.DeclaringType);

      var mock = MockRepository.GenerateStub<IFieldWrapper>();
      mock
          .Stub (x => x.Field)
          .Return (field);
      mock
          .Stub (x => x.GetMemberExpression (Arg<Expression>.Is.Anything))
          .Return (Expression.Field (thisExpression, field));

      return mock;
    }

    public static NewExpression GetNewExpression (Type type = null)
    {
      type = GetInstantiatableType (type);
      var constructor = type.GetConstructors().First();
      var parameters = constructor.GetParameters().Select(x => Expression.Parameter(x.ParameterType));
      return Expression.New (constructor, parameters.Cast<Expression>());
    }

    private static ParameterInfo GetParameterInfo (string name = null, Type parameterType = null, int position = -1)
    {
      position = position >= 0 ? position : s_random.Next (0, 5);
      name = name ?? "parameter" + position;
      parameterType = GetInstantiatableType (parameterType);

      var mock = MockRepository.GenerateStrictMock<ParameterInfo>();

      mock
          .Expect (x => x.Name)
          .Return (name).Repeat.Any();
      mock
          .Expect (x => x.ParameterType)
          .Return (parameterType).Repeat.Any();
      mock
          .Expect (x => x.Position)
          .Return (position).Repeat.Any();

      return mock;
    }

    private static MethodSignature GetMethodSignature (Type returnType = null, IEnumerable<Type> parameterTypes = null, int genericParameterCount = -1)
    {
      returnType = GetInstantiatableType (returnType);
      parameterTypes = parameterTypes ?? GetMultiple (() => GetInstantiatableType(), s_random.Next (0, 5));
      genericParameterCount = genericParameterCount >= 0 ? genericParameterCount : s_random.Next (0, 5);

      return new MethodSignature (returnType, parameterTypes, genericParameterCount);
    }

    private static IEnumerable<T> GetMultiple<T> (Func<T> factory, int count)
    {
      return Enumerable.Range (0, count).Select (x => factory());
    }

    public static MutableMethodInfo GetMutableMethodInfo (MethodInfo methodInfo = null)
    {
      return methodInfo == null
                 ? MutableMethodInfoObjectMother.Create()
                 : MutableMethodInfoObjectMother.CreateForExisting (originalMethodInfo: methodInfo);
    }
  }
}