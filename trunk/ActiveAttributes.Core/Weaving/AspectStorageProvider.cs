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
using ActiveAttributes.Infrastructure;
using ActiveAttributes.Weaving.Storage;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (AspectStorageProvider))]
  public interface IAspectStorageBuilder
  {
    IStorage CreateStorage (Aspect aspect, JoinPoint joinPoint);
  }

  [ConcreteImplementation (typeof (AspectStorageProvider))]
  public interface IAspectCacheKeyProvider
  {
    object CreateCacheKey (Aspect aspect, JoinPoint joinPoint);
  }

  public class AspectStorageProvider : IAspectStorageBuilder, IAspectCacheKeyProvider
  {
    private readonly IAspectExpressionBuilder _aspectExpressionBuilder;
    private readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

    private int _counter;

    public AspectStorageProvider (IAspectExpressionBuilder aspectExpressionBuilder)
    {
      _aspectExpressionBuilder = aspectExpressionBuilder;
    }

    public virtual IStorage CreateStorage (Aspect aspect, JoinPoint joinPoint)
    {
      var mutableType = (MutableType) joinPoint.DeclaringType;
      var initExpression = _aspectExpressionBuilder.CreateAspectInitExpressions (aspect.Construction);
      var name = "<aa>_Aspect" + _counter++ + "_" + aspect.Type;

      switch (aspect.Scope)
      {
        case AspectScope.PerType:
          var staticField = mutableType.AddField (name, aspect.Type, FieldAttributes.Private | FieldAttributes.Static);
          mutableType.AddTypeInitialization (ctx => Expression.Assign (Expression.Field (null, staticField), initExpression));
          return new StaticStorage (staticField);
        case AspectScope.PerObject:
          var instanceField = mutableType.AddField (name, aspect.Type);
          mutableType.AddInstanceInitialization (ctx => Expression.Assign (Expression.Field (ctx.This, instanceField), initExpression));
          return new InstanceStorage (instanceField);
        case AspectScope.Singleton:
        case AspectScope.PerDeclaration:
          return new SingletonStorage (name, aspect.Type, initExpression);
        case AspectScope.Transient:
          return new TransientStorage (initExpression);
      }
      throw new NotImplementedException();
    }

    public virtual object CreateCacheKey (Aspect aspect, JoinPoint joinPoint)
    {
      switch (aspect.Scope)
      {
        case AspectScope.Singleton:
          return aspect.Type;
        case AspectScope.PerDeclaration:
          return aspect;
        case AspectScope.PerObject:
        case AspectScope.PerType:
          return joinPoint.DeclaringType;
        case AspectScope.Transient:
          return null;
        default:
          throw new Exception (string.Format ("Storage not implemented for {0}", aspect.Scope));
      }
    }
  }

}