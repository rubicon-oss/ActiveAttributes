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
using ActiveAttributes.Advices;
using ActiveAttributes.Assembly.Storages;
using ActiveAttributes.Declaration.Construction;
using ActiveAttributes.Extensions;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Assembly
{
  /// <summary>Serves as a service for initializations.</summary>
  [ConcreteImplementation (typeof (InitializationService))]
  public interface IInitializationService
  {
    /// <summary>Creates and initializes a <see cref="IStorage"/> for the <see cref="MemberInfo"/> of a given mutable method.</summary>
    IStorage AddMemberInfo (MutableMethodInfo mutableMethod);

    /// <summary>Creates and initializes a <see cref="IStorage"/> for the <see cref="Delegate"/> of a given mutable method.</summary>
    IStorage AddDelegate (MutableMethodInfo mutableMethod);

    /// <summary>Creates and initializes a <see cref="IStorage"/> for an <see cref="Advice"/>.</summary>
    IStorage GetOrAddAspect (Advice advice, MutableType mutableType);
  }

  public class InitializationService : IInitializationService
  {
    private readonly IStorageService _storageService;
    private readonly IInitializationExpressionHelper _initializationExpressionHelper;

    private readonly Dictionary<Tuple<MutableType, IConstruction, AdviceScope>, IStorage> _fieldWrappers =
        new Dictionary<Tuple<MutableType, IConstruction, AdviceScope>, IStorage>();

    public InitializationService (IStorageService storageService, IInitializationExpressionHelper initializationExpressionHelper)
    {
      ArgumentUtility.CheckNotNull ("storageService", storageService);
      ArgumentUtility.CheckNotNull ("initializationExpressionHelper", initializationExpressionHelper);

      _storageService = storageService;
      _initializationExpressionHelper = initializationExpressionHelper;
    }

    public IStorage AddMemberInfo (MutableMethodInfo mutableMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      Assertion.IsNotNull (mutableMethod.DeclaringType);

      var mutableType = (MutableType) mutableMethod.DeclaringType;

      IStorage storage;
      Func<Expression, Expression> initializationProvider;

      var property = mutableMethod.UnderlyingSystemMethodInfo.GetRelatedPropertyInfo();
      if (property != null)
      {
        storage = _storageService.AddStaticStorage (mutableType, typeof (PropertyInfo), mutableMethod.Name);
        initializationProvider = expr => _initializationExpressionHelper.CreatePropertyInfoInitExpression (property);
      }
      else
      {
        storage = _storageService.AddStaticStorage (mutableType, typeof (MethodInfo), mutableMethod.Name);
        initializationProvider = expr => _initializationExpressionHelper.CreateMethodInfoInitExpression (mutableMethod);
      }

      AddStaticInitialization (mutableType, storage, initializationProvider);

      return storage;
    }

    public IStorage AddDelegate (MutableMethodInfo mutableMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      Assertion.IsNotNull (mutableMethod.DeclaringType);

      var mutableType = (MutableType) mutableMethod.DeclaringType;
      var delegateType = mutableMethod.GetDelegateType();
      var storage = _storageService.AddStaticStorage (mutableType, delegateType, mutableMethod.Name);
      foreach (var constructor in mutableType.AllMutableConstructors)
      {
        constructor.SetBody (
            ctx =>
            {
              var initialization = _initializationExpressionHelper.CreateDelegateInitExpression (mutableMethod, ctx.This);
              var assignExpression = GetAssignExpression (storage, ctx.This, initialization);
              return Expression.Block (typeof (void), ctx.PreviousBody, assignExpression);
            });
      }
      return storage;
    }

    public IStorage GetOrAddAspect (Advice advice, MutableType mutableType)
    {
      ArgumentUtility.CheckNotNull ("advice", advice);
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);

      // TODO use mutableType in tuple
      IStorage field;
      var tuple = Tuple.Create (mutableType, advice.Construction, advice.Scope);
      if (!_fieldWrappers.TryGetValue (tuple, out field))
      {
        field = AddAspect (advice, mutableType);
        _fieldWrappers.Add (tuple, field);
      }

      return field;
    }

    private IStorage AddAspect (Advice advice, MutableType mutableType)
    {
      Func<Expression, Expression> initializationProvider = expr => _initializationExpressionHelper.CreateAspectInitExpression (advice.Construction);
      switch (advice.Scope)
      {
        case AdviceScope.Static:
          var staticStorage = _storageService.AddStaticStorage (mutableType, advice.DeclaringType, "aspect");
          AddStaticInitialization (mutableType, staticStorage, initializationProvider);
          return staticStorage;
        case AdviceScope.Instance:
          var instanceStorage = _storageService.AddInstanceStorage (mutableType, advice.DeclaringType, "aspect");
          AddInstanceInitialization (mutableType, instanceStorage, initializationProvider);
          return instanceStorage;
      }

      throw new NotImplementedException();
    }

    private void AddInstanceInitialization (MutableType mutableType, IStorage storage, Func<Expression, Expression> initializationProvider)
    {
      foreach (var constructor in mutableType.AllMutableConstructors)
      {
        constructor.SetBody (
            ctx =>
            {
              var initialization = initializationProvider (ctx.This);
              var assignExpression = GetAssignExpression (storage, ctx.This, initialization);
              return Expression.Block (typeof (void), ctx.PreviousBody, assignExpression);
            });
      }
    }

    private void AddStaticInitialization (MutableType mutableType, IStorage storage, Func<Expression, Expression> initializationProvider)
    {
      var initialization = initializationProvider (null);
      var assignExpression = GetAssignExpression (storage, null, initialization);
      mutableType.AddTypeInitialization (ctx => assignExpression);
    }

    private BinaryExpression GetAssignExpression (IStorage storage, Expression thisExpression, Expression initialization)
    {
      return Expression.Assign (storage.CreateStorageExpression (thisExpression), initialization);
    }
  }
}