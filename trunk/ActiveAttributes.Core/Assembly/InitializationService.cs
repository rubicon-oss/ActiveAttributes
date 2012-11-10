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
using ActiveAttributes.Core.Assembly.FieldWrapper;
using ActiveAttributes.Core.Extensions;
using JetBrains.Annotations;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (InitializationService))]
  public interface IInitializationService
  {
    IFieldWrapper AddMemberInfoInitialization (MethodInfo method);

    IFieldWrapper AddDelegateInitialization (MethodInfo method);
  }

  public class InitializationService : IInitializationService
  {
    private readonly IFieldService _fieldService;
    private readonly IConstructorExpressionsHelperFactory _expressionsHelperFactory;
    private readonly IMethodCopyService _methodCopyService;


    // TODO use TypeInitialization Expressions
    // TODO MethodCopyService?
    public InitializationService (IFieldService fieldService, IConstructorExpressionsHelperFactory expressionsHelperFactory, IMethodCopyService methodCopyService)
    {
      ArgumentUtility.CheckNotNull ("fieldService", fieldService);
      ArgumentUtility.CheckNotNull ("expressionsHelperFactory", expressionsHelperFactory);
      ArgumentUtility.CheckNotNull ("methodCopyService", methodCopyService);

      _fieldService = fieldService;
      _expressionsHelperFactory = expressionsHelperFactory;
      _methodCopyService = methodCopyService;
    }

    public IFieldWrapper AddMemberInfoInitialization (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      Assertion.IsTrue (method.DeclaringType is MutableType);

      var mutableType = (MutableType) method.DeclaringType;
      var mutableMethod = (MutableMethodInfo) method;
      IFieldWrapper field;
      Func<IConstructorExpressionsHelper, Expression> expressionProvider;

      // TODO UnderlyingSystemMethodInfo
      var property = mutableMethod.UnderlyingSystemMethodInfo.GetRelatedPropertyInfo ();
      if (property != null)
      {
        field = _fieldService.AddField (mutableType, typeof (PropertyInfo), method.Name, FieldAttributes.Private | FieldAttributes.Static);
        expressionProvider = x => x.CreateMemberInfoAssignExpression (field, property);
      }
      else
      {
        field = _fieldService.AddField (mutableType, typeof (MethodInfo), method.Name, FieldAttributes.Private | FieldAttributes.Static);
        expressionProvider = x => x.CreateMemberInfoAssignExpression (field, method);
      }

      AddInitialization (mutableType, expressionProvider);

      return field;
    }

    public IFieldWrapper AddDelegateInitialization (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      Assertion.IsTrue (method.DeclaringType is MutableType);

      var mutableType = (MutableType) method.DeclaringType;
      var field = _fieldService.AddField (mutableType, method.GetDelegateType(), "Delegate", FieldAttributes.Private | FieldAttributes.Static);

      var copy = _methodCopyService.GetCopy ((MutableMethodInfo) method);

      Func<IConstructorExpressionsHelper, Expression> expressionProvider = x => x.CreateDelegateAssignExpression (field, copy);
      AddInitialization (mutableType, expressionProvider);

      return field;
    }

    private void AddInitialization (MutableType mutableType, Func<IConstructorExpressionsHelper, Expression> expressionProvider)
    {
      var constructor = mutableType.AllMutableConstructors.Single();
      constructor.SetBody (
          ctx =>
          {
            var expressionHelper = _expressionsHelperFactory.CreateConstructorExpressionHelper (ctx);
            return Expression.Block (ctx.PreviousBody, expressionProvider (expressionHelper));
          });
    }
  }
}