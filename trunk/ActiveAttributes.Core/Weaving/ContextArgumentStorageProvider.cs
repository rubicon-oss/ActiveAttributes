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
using System.Reflection;
using ActiveAttributes.Extensions;
using ActiveAttributes.Weaving.Storage;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (ContextArgumentStorageProvider))]
  public interface IContextArgumentStorageProvider
  {
    /// <summary>Creates and initializes a <see cref="IStorage"/> for the <see cref="MemberInfo"/> of a given mutable method.</summary>
    IStorage AddMemberInfo (MutableMethodInfo mutableMethod);
  }

  public class ContextArgumentStorageProvider : IContextArgumentStorageProvider
  {
    private int _counter;

    private readonly IContextArgumentExpressionBuilder _contextArgumentExpressionBuilder;

    public ContextArgumentStorageProvider (IContextArgumentExpressionBuilder contextArgumentExpressionBuilder)
    {
      _contextArgumentExpressionBuilder = contextArgumentExpressionBuilder;
    }

    public IStorage AddMemberInfo (MutableMethodInfo mutableMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableMethod", mutableMethod);
      Assertion.IsNotNull (mutableMethod.DeclaringType);

      var mutableType = (MutableType) mutableMethod.DeclaringType;

      IStorage storage;
      Func<ThisExpression, Expression> initializationProvider;

      var property = mutableMethod.UnderlyingSystemMethodInfo.GetRelatedPropertyInfo ();
      if (property != null)
      {
        storage = AddStaticStorage (mutableType, typeof (PropertyInfo), mutableMethod.Name);
        initializationProvider = thisExpression => _contextArgumentExpressionBuilder.CreatePropertyInfoInitExpression (property);
      }
      else
      {
        storage = AddStaticStorage (mutableType, typeof (MethodInfo), mutableMethod.Name);
        initializationProvider = expr => _contextArgumentExpressionBuilder.CreateMethodInfoInitExpression (mutableMethod);
      }

      mutableType.AddTypeInitialization (ctx => CreateAssignExpression (storage, initializationProvider, ctx));

      return storage;
    }

    private IStorage AddStaticStorage (MutableType mutableType, Type fieldType, string fieldName)
    {
      var field = mutableType.AddField (fieldName + _counter++, fieldType, FieldAttributes.Private | FieldAttributes.Static);
      return new StaticStorage (field);
    }

    private Expression CreateAssignExpression (IStorage storage, Func<ThisExpression, Expression> expressionProvider, BodyContextBase ctx)
    {
      return Expression.Assign (storage.CreateStorageExpression (null), expressionProvider (null));
    }
  }
}