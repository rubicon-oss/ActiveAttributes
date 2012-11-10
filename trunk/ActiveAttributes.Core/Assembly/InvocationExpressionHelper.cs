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
using ActiveAttributes.Core.Assembly.Storage;
using ActiveAttributes.Core.Interception.Invocations;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (InvocationExpressionHelper))]
  public interface IInvocationExpressionHelper
  {
    NewExpression CreateInnermostInvocation (
        Expression thisExpression, Type innerInvocationType, Expression invocationContext, IStorage delegateField);

    NewExpression CreateOuterInvocation (
        Expression previousAspect,
        Expression previousInvocation,
        MethodInfo previousAdvice,
        Expression invocationContext);
  }

  public class InvocationExpressionHelper : IInvocationExpressionHelper
  {
    public NewExpression CreateInnermostInvocation (
        Expression thisExpression, Type innerInvocationType, Expression invocationContext, IStorage delegateField)
    {
      var constructor = innerInvocationType.GetConstructors().Single();
      var arguments = new[] { invocationContext, delegateField.GetStorageExpression (thisExpression) };

      return Expression.New (constructor, arguments);
    }

    public NewExpression CreateOuterInvocation (
        Expression previousAspect, Expression previousInvocation, MethodInfo previousAdvice, Expression invocationContext)
    {
      ArgumentUtility.CheckNotNull ("previousAspect", previousAspect);
      ArgumentUtility.CheckNotNull ("previousInvocation", previousInvocation);
      ArgumentUtility.CheckNotNull ("previousAdvice", previousAdvice);
      ArgumentUtility.CheckNotNull ("invocationContext", invocationContext);

      var constructor = typeof (OuterInvocation).GetConstructors().Single();
      var arguments = new[]
                      {
                          invocationContext,
                          GetAdviceDelegate (previousAspect, previousAdvice),
                          previousInvocation
                      };

      return Expression.New (constructor, arguments);
    }

    private UnaryExpression GetAdviceDelegate (Expression previousAspect, MethodInfo previousAdvice)
    {
      var createDelegate = typeof (Delegate).GetMethod ("CreateDelegate", new[] { typeof (Type), typeof (object), typeof (MethodInfo) });
      var delegateType = typeof (Action<IInvocation>);
      var delegate_ = Expression.Call (null, createDelegate, Expression.Constant (delegateType), previousAspect, Expression.Constant (previousAdvice));
      var typeDelegate = Expression.Convert (delegate_, delegateType);
      return typeDelegate;
    }
  }
}