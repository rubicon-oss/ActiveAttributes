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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Extensions
{
  public static class PropertyInfoExtensions
  {
    public static PropertyInfo GetOverridenProperty (this PropertyInfo propertyInfo)
    {
      var typeSequence = propertyInfo.DeclaringType.BaseType.CreateSequence (x => x.BaseType);

      var getMethodInfo = propertyInfo.GetGetMethod (true);
      var setMethodInfo = propertyInfo.GetSetMethod (true);


      var getMethodBase = getMethodInfo.GetBaseDefinition();
      var setMethodBase = setMethodInfo.GetBaseDefinition();

      var bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;
      return typeSequence
          .SelectMany (x => x.GetProperties (bindingFlags))
          .Where (x => x.GetGetMethod (true).GetBaseDefinition() == getMethodBase || x.GetSetMethod (true).GetBaseDefinition() == setMethodBase)
          .FirstOrDefault();
    }



    public static PropertyInfo GetProperty<TObject, TProperty> (Expression<Func<TObject, TProperty>> expression)
    {
      var memberExpression = expression.Body as MemberExpression;
      var propertyInfos = typeof (TObject).GetProperties (BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
      var enumerable = propertyInfos.Where (x => x.GetOverridenProperty() == memberExpression.Member);
      return enumerable.SingleOrDefault ();
    }
  }
}