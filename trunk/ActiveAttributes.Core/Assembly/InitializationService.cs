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
using ActiveAttributes.Core.AdviceInfo;
using ActiveAttributes.Core.Assembly.Storages;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.Construction;
using ActiveAttributes.Core.Extensions;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public interface IInitializationService
  {
    IStorage GetOrAddAspect (Advice advice, MutableType mutableType);

    IStorage AddMemberInfo (MutableMethodInfo mutableMethod);

    IStorage AddDelegate (MutableMethodInfo mutableMethod);
  }

  public class InitializationService : IInitializationService
  {
    private readonly IFieldService _fieldService;
    private readonly IInitializationExpressionHelper _initializationExpressionHelper;

    private readonly Dictionary<Tuple<IConstruction, AdviceScope>, IStorage> _fieldWrappers =
        new Dictionary<Tuple<IConstruction, AdviceScope>, IStorage>();

    public InitializationService (IFieldService fieldService, IInitializationExpressionHelper initializationExpressionHelper)
    {
      ArgumentUtility.CheckNotNull ("fieldService", fieldService);
      ArgumentUtility.CheckNotNull ("initializationExpressionHelper", initializationExpressionHelper);

      _fieldService = fieldService;
      _initializationExpressionHelper = initializationExpressionHelper;
    }

    public IStorage GetOrAddAspect (Advice advice, MutableType mutableType)
    {
      ArgumentUtility.CheckNotNull ("advice", advice);
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);

      IStorage field;
      var tuple = Tuple.Create (advice.Construction, advice.Scope);
      if (!_fieldWrappers.TryGetValue (tuple, out field))
      {
        field = AddStorage (advice, mutableType);
        _fieldWrappers.Add (tuple, field);
      }

      return field;
    }

    public IStorage AddMemberInfo (MutableMethodInfo mutableMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      Assertion.IsNotNull (mutableMethod.DeclaringType);

      var mutableType = (MutableType) mutableMethod.DeclaringType;

      IStorage storage;
      Expression initialization;

      var property = mutableMethod.UnderlyingSystemMethodInfo.GetRelatedPropertyInfo();
      if (property != null)
      {
        storage = _fieldService.AddStaticStorage (mutableType, typeof (PropertyInfo), mutableMethod.Name);
        initialization = _initializationExpressionHelper.CreatePropertyInfoInitExpression (property);
      }
      else
      {
        storage = _fieldService.AddStaticStorage (mutableType, typeof (MethodInfo), mutableMethod.Name);
        initialization = _initializationExpressionHelper.CreateMethodInfoInitExpression (mutableMethod);
      }

      AddStaticInitialization (mutableType, storage, initialization);

      return storage;
    }

    public IStorage AddDelegate (MutableMethodInfo mutableMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      Assertion.IsNotNull (mutableMethod.DeclaringType);

      var mutableType = (MutableType) mutableMethod.DeclaringType;
      var delegateType = mutableMethod.GetDelegateType();
      var storage = _fieldService.AddStaticStorage (mutableType, delegateType, mutableMethod.Name);
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

    private IStorage AddStorage (Advice advice, MutableType mutableType)
    {
      var initialization = _initializationExpressionHelper.CreateAspectInitExpression (advice.Construction);
      switch (advice.Scope)
      {
        case AdviceScope.Static:
          return AddStaticStorageAndInitialization (advice, mutableType, initialization);
        case AdviceScope.Instance:
          return AddInstanceStorageAndInitialization (advice, mutableType, initialization);
      }

      throw new NotImplementedException();
    }

    private IStorage AddInstanceStorageAndInitialization (Advice advice, MutableType mutableType, Expression initialization)
    {
      var storage = _fieldService.AddInstanceStorage (mutableType, advice.DeclaringType, "aspect");
      AddInstanceInitialization (mutableType, storage, initialization);
      return storage;
    }

    private IStorage AddStaticStorageAndInitialization (Advice advice, MutableType mutableType, Expression initialization)
    {
      var storage = _fieldService.AddStaticStorage (mutableType, advice.DeclaringType, "aspect");
      AddStaticInitialization (mutableType, storage, initialization);
      return storage;
    }



    private void AddInstanceInitialization (MutableType mutableType, IStorage storage, Expression initialization)
    {
      foreach (var constructor in mutableType.AllMutableConstructors)
      {
        constructor.SetBody (
            ctx =>
            {
              var assignExpression = GetAssignExpression (storage, ctx.This, initialization);
              return Expression.Block (typeof (void), ctx.PreviousBody, assignExpression);
            });
      }
    }

    private void AddStaticInitialization (MutableType mutableType, IStorage storage, Expression initialization)
    {
      var assignExpression = GetAssignExpression (storage, null, initialization);
      mutableType.AddTypeInitialization (assignExpression);
    }



    private BinaryExpression GetAssignExpression (IStorage storage, Expression thisExpression, Expression initialization)
    {
      return Expression.Assign (storage.GetStorageExpression (thisExpression), initialization);
    }
  }
}