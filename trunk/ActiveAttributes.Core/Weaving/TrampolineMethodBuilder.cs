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
using System.Linq;
using System.Reflection;
using ActiveAttributes.Infrastructure;
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (TrampolineMethodBuilder))]
  public interface ITrampolineMethodBuilder
  {
    FieldInfo Create (JoinPoint joinPoint, ParameterExpression context, Expression body);
  }

  public class TrampolineMethodBuilder : ITrampolineMethodBuilder
  {
    private int _counter;

    public FieldInfo Create (JoinPoint joinPoint, ParameterExpression context, Expression body)
    {
      var mutableType = joinPoint.DeclaringType;
      var method = joinPoint.Method;
      var delegateType = typeof (Action<>).MakeGenericType (context.Type);

      var newMethod = joinPoint.DeclaringType.AddMethod (
          "<aa>" + method.Name + _counter++,
          MethodAttributes.Private,
          typeof (void),
          new[] { new ParameterDeclaration (context.Type, "context") },
          ctx => body.Replace (new Dictionary<Expression, Expression> { { context, ctx.Parameters.Single () } }));

      var field = mutableType.AddField (newMethod.Name, delegateType);
      mutableType.AddInstanceInitialization (
          ctx =>
          Expression.Assign (
              Expression.Field (ctx.This, field),
              new NewDelegateExpression (delegateType, ctx.This, newMethod)));

      return field;
    }
  }
}