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
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Construction;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (ConstructorInitializationService))]
  public interface IConstructorInitializationService
  {
    IFieldWrapper AddMemberInfoInitialization (MutableMethodInfo method);

    IFieldWrapper AddDelegateInitialization (MutableMethodInfo method);

    IFieldWrapper AddAspectInitialization (MutableType mutableType, IAspectConstructionInfo aspectConstructionInfo, Scope scope);
  }

  public class ConstructorInitializationService : IConstructorInitializationService
  {
    private readonly IFieldIntroducer2 _fieldIntroducer2;
    private readonly IConstructorExpressionsHelperFactory _expressionsHelperFactory;

    public ConstructorInitializationService (IFieldIntroducer2 fieldIntroducer2, IConstructorExpressionsHelperFactory expressionsHelperFactory)
    {
      ArgumentUtility.CheckNotNull ("fieldIntroducer2", fieldIntroducer2);
      ArgumentUtility.CheckNotNull ("expressionsHelperFactory", expressionsHelperFactory);

      _fieldIntroducer2 = fieldIntroducer2;
      _expressionsHelperFactory = expressionsHelperFactory;
    }

    public IFieldWrapper AddMemberInfoInitialization (MutableMethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      Assertion.IsNotNull (method.DeclaringType);

      var mutableType = (MutableType) method.DeclaringType;
      IFieldWrapper field;
      Func<IConstructorExpressionsHelper, Expression> expressionProvider;

      var property = method.UnderlyingSystemMethodInfo.GetRelatedPropertyInfo();
      if (property != null)
      {
        field = _fieldIntroducer2.AddField (mutableType, typeof (PropertyInfo), method.Name, FieldAttributes.Private | FieldAttributes.Static);
        expressionProvider = x => x.CreateMemberInfoAssignExpression (field, property);
      }
      else
      {
        field = _fieldIntroducer2.AddField (mutableType, typeof (MethodInfo), method.Name, FieldAttributes.Private | FieldAttributes.Static);
        expressionProvider = x => x.CreateMemberInfoAssignExpression (field, method);
      }

      AddInitialization (mutableType, expressionProvider);

      return field;
    }

    public IFieldWrapper AddDelegateInitialization (MutableMethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      Assertion.IsNotNull (method.DeclaringType);

      var mutableType = (MutableType) method.DeclaringType;
      var field = _fieldIntroducer2.AddField (mutableType, typeof (Action), "Delegate", FieldAttributes.Private | FieldAttributes.Static);

      Func<IConstructorExpressionsHelper, Expression> expressionProvider = x => x.CreateDelegateAssignExpression (field, method);
      AddInitialization (mutableType, expressionProvider);

      return field;
    }

    public IFieldWrapper AddAspectInitialization (MutableType mutableType, IAspectConstructionInfo aspectConstructionInfo, Scope scope)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      ArgumentUtility.CheckNotNull ("aspectConstructionInfo", aspectConstructionInfo);

      var attributes = FieldAttributes.Private | (scope == Scope.Static ? FieldAttributes.Static : 0);
      var field = _fieldIntroducer2.AddField (mutableType, typeof (IAspect), "TODO", attributes);

      Func<IConstructorExpressionsHelper, Expression> expressionProvider = x => x.CreateAspectAssignExpression (field, aspectConstructionInfo);
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