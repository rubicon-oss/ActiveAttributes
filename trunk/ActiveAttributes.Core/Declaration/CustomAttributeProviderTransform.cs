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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Advices;
using ActiveAttributes.Assembly;
using ActiveAttributes.Declaration.Construction;
using ActiveAttributes.Extensions;
using ActiveAttributes.Pointcuts;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Declaration
{
  [ConcreteImplementation (typeof (CustomAttributeProviderTransform))]
  public interface ICustomAttributeProviderTransform
  {
    IAdviceBuilder GetAdviceBuilder (ICustomAttributeProvider customAttributeProvider, IAdviceBuilder parentAdviceBuilder = null);
  }

  public class CustomAttributeProviderTransform : ICustomAttributeProviderTransform
  {
    private readonly IAdviceBuilderFactory _adviceBuilderFactory;

    public CustomAttributeProviderTransform (IAdviceBuilderFactory adviceBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("adviceBuilderFactory", adviceBuilderFactory);

      _adviceBuilderFactory = adviceBuilderFactory;
    }

    public IAdviceBuilder GetAdviceBuilder (ICustomAttributeProvider customAttributeProvider, IAdviceBuilder parentAdviceBuilder = null)
    {
      ArgumentUtility.CheckNotNull ("customAttributeProvider", customAttributeProvider);

      var adviceBuilder = parentAdviceBuilder != null ? parentAdviceBuilder.Copy() : _adviceBuilderFactory.Create();

      var type = customAttributeProvider as Type;
      if (type != null)
        adviceBuilder.SetConstruction (new TypeConstruction (type));
      // TODO adviceBuilder.SetConstruction (new TypeConstruction (customAttributeProvider as Type));

      adviceBuilder.SetMethod (customAttributeProvider as MethodInfo);

      var adviceAttribute = customAttributeProvider.GetCustomAttributes<AdviceInfoAttribute>(true).SingleOrDefault();
      if (adviceAttribute != null)
      {
        TrySetValue (adviceBuilder.SetName, adviceAttribute.Name);
        TrySetValue (adviceBuilder.SetRole, adviceAttribute.Role);
        TrySetValue (adviceBuilder.SetExecution, adviceAttribute.Execution);
        TrySetValue (adviceBuilder.SetScope, adviceAttribute.Scope);
        TrySetValue (adviceBuilder.SetPriority, adviceAttribute.Priority);
      }

      AddPointcuts (customAttributeProvider, adviceBuilder);

      return adviceBuilder;
    }

    private void TrySetValue<T> (Func<T, IAdviceBuilder> set, T value)
    {
      if (value != null && !value.Equals (default (T)))
        set (value);
    }

    private static void AddPointcuts (ICustomAttributeProvider customAttributeProvider, IAdviceBuilder adviceBuilder)
    {
      var pointcutAttributes = customAttributeProvider.GetCustomAttributes<PointcutAttributeBase> (true);
      foreach (var pointcutAttribute in pointcutAttributes)
      {
        IPointcut pointcut;

        var methodPointcutAttribute = pointcutAttribute as MethodPointcutAttribute;
        if (methodPointcutAttribute != null)
        {
          var method = ParseMethodPointcut(customAttributeProvider, methodPointcutAttribute);
          pointcut = new MethodPointcut (method);
        }
        else
          pointcut = pointcutAttribute.Pointcut;

        adviceBuilder.AddPointcut (pointcut);
      }
    }

    private static MethodInfo ParseMethodPointcut (ICustomAttributeProvider customAttributeProvider, MethodPointcutAttribute methodPointcutAttribute)
    {
      var declaringType = customAttributeProvider as Type ?? ((MethodInfo) customAttributeProvider).DeclaringType;
      var methodName = methodPointcutAttribute.MethodName;
      var method = declaringType.GetMethod (methodName, BindingFlags.Public | BindingFlags.Static);

      if (method == null)
      {
        var message = string.Format ("Pointcut method '{0}' is missing or not declared as static.", methodName);
        throw new InvalidOperationException (message);
      }

      var parameter = method.GetParameters().SingleOrDefault();
      if (parameter == null || parameter.ParameterType != typeof (JoinPoint) || method.ReturnType != typeof (bool))
      {
        var message = string.Format ("Pointcut method '{0}' must have JoinPoint as argument and bool as return type.", methodName);
        throw new InvalidOperationException (message);
      }

      return method;
    }
  }
}