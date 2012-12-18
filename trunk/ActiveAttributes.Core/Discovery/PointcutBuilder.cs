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
using System.Reflection;
using ActiveAttributes.Aspects.Pointcuts;
using ActiveAttributes.Extensions;
using System.Linq;
using ActiveAttributes.Model;
using ActiveAttributes.Model.Pointcuts;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Discovery
{
  [ConcreteImplementation (typeof (PointcutBuilder))]
  public interface IPointcutBuilder
  {
    IPointcut Build (ICustomAttributeProvider customAttributeProvider);
    IPointcut Build (ICustomAttributeData customAttributeData);
  }

  public class PointcutBuilder : IPointcutBuilder
  {
    private static readonly Dictionary<string, Type> s_namedArgumentsDictionary
        = new Dictionary<string, Type>
          {
              { "TypePointcut", typeof (TypePointcut) },
              { "MemberNamePointcut", typeof (MemberNamePointcut) }
          };

    public IPointcut Build (ICustomAttributeProvider customAttributeProvider)
    {

      var attributes = customAttributeProvider.GetCustomAttributes (true);
      var pointcuts = attributes.OfType<PointcutAttributeBase>().Select (x => Convert (customAttributeProvider, x)).ToList();

      if (pointcuts.Count == 0)
        return new TruePointcut();

      var hasNot = attributes.OfType<NotAttribute>().Any();
      var hasOr = attributes.OfType<OrAttribute>().Any();
      var hasAnd = attributes.OfType<AndAttribute>().Any();

      if (pointcuts.Count == 1)
      {
        var pointcut = pointcuts.Single ();

        if (hasOr || hasAnd)
          throw new Exception (string.Format ("Can't use AND or OR on '{0}' with single pointcut", customAttributeProvider));

        return !hasNot ? pointcut : new NotPointcut (pointcut);
      }
      else
      {
        if (hasNot)
          throw new Exception (string.Format ("Can't use NOT on '{0}' with multiple pointcuts", customAttributeProvider));

        if (hasOr && hasAnd)
          throw new Exception (string.Format ("Can't use AND and OR on '{0}'", customAttributeProvider));

        if (!hasOr && !hasAnd)
          throw new Exception (string.Format ("Must use logical operator on '{0}'", customAttributeProvider));

        return hasOr ? (IPointcut) new AnyPointcut (pointcuts) : new AllPointcut (pointcuts);
      }
    }

    public IPointcut Build (ICustomAttributeData customAttributeData)
    {
      var pointcuts = new List<IPointcut>();
      foreach (var argument in customAttributeData.NamedArguments)
      {
        Type pointcutType;
        if (s_namedArgumentsDictionary.TryGetValue (argument.MemberInfo.Name, out pointcutType))
        {
          var pointcut = pointcutType.CreateInstance<IPointcut> (argument.Value);
          pointcuts.Add (pointcut);
        }
      }

      return new AllPointcut (pointcuts);
    }

    private IPointcut Convert (ICustomAttributeProvider customAttributeProvider, PointcutAttributeBase pointcutAttribute)
    {
      var methodPointcutAttribute = pointcutAttribute as MethodPointcutAttribute;
      if (methodPointcutAttribute != null)
        return Convert (customAttributeProvider, methodPointcutAttribute.MethodName);

      return pointcutAttribute.Pointcut;
    }

    private IPointcut Convert (ICustomAttributeProvider customAttributeProvider, string methodName)
    {
      var type = customAttributeProvider as Type ?? ((MethodInfo) customAttributeProvider).DeclaringType;
      var method = type.GetMethod (methodName, BindingFlags.Public | BindingFlags.Static);

      if (method == null)
      {
        var message = string.Format ("Pointcut method '{0}' is missing or not declared as static.", methodName);
        throw new InvalidOperationException (message);
      }

      var parameter = method.GetParameters ().SingleOrDefault ();
      if (parameter == null || parameter.ParameterType != typeof (JoinPoint) || method.ReturnType != typeof (bool))
      {
        var message = string.Format ("Pointcut method '{0}' must have JoinPoint as argument and bool as return type.", methodName);
        throw new InvalidOperationException (message);
      }

      return new MethodPointcut (method);
    }
  }
}

