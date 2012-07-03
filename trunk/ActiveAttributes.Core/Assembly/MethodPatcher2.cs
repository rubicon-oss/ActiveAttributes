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
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace ActiveAttributes.Core.Assembly
{
  public class MethodPatcher2
  {
    private TypeProvider _typeProvider;

    private readonly MethodInfo _onInterceptMethodInfo;
    private readonly MethodInfo _onInterceptGetMethodInfo;
    private readonly MethodInfo _onInterceptSetMethodInfo;

    public MethodPatcher2 ()
    {
      _onInterceptMethodInfo = typeof (MethodInterceptionAspectAttribute).GetMethod ("OnIntercept");
      _onInterceptGetMethodInfo = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptGet");
      _onInterceptSetMethodInfo = typeof (PropertyInterceptionAspectAttribute).GetMethod ("OnInterceptSet");
    }

    public MutableMethodInfo Patch (MutableMethodInfo mutableMethod, FieldIntroducer.Data fieldData, IEnumerable<CompileTimeAspectBase> aspects)
    {
      _typeProvider = new TypeProvider (mutableMethod);

      var mutableType = (MutableType) mutableMethod.DeclaringType;
      var copiedMethod = GetCopiedMethod (mutableType, mutableMethod);

      mutableMethod.SetBody (ctx => GetPatchedBody (ctx, fieldData, aspects));

      return copiedMethod;
    }

    private Expression GetPatchedBody (MethodBodyModificationContext ctx, FieldIntroducer.Data fieldData, IEnumerable<CompileTimeAspectBase> aspects)
    {
      var aspectsAsCollection = aspects.ToList();

      var methodInfoFieldExpression = Expression.Field (ctx.This, fieldData.MethodInfoField);
      var delegateFieldExpression = Expression.Field (ctx.This, fieldData.DelegateField);
      var aspectsFieldExpression = Expression.Field (ctx.This, fieldData.InstanceAspectsField);

      var invocationContextType = _typeProvider.GetInvocationContextType();
      var invocationContextVariableExpression = Expression.Variable (invocationContextType, "ctx");
      var invocationContextConstructor = invocationContextType.GetConstructors().Single();
      var invocationContextArgumentExpressions = new[] { methodInfoFieldExpression, ctx.This }.Concat (ctx.Parameters.Cast<Expression>());
      var invocationContextNewExpression = Expression.New (invocationContextConstructor, invocationContextArgumentExpressions);
      var invocationContextAssignExpression = Expression.Assign (invocationContextVariableExpression, invocationContextNewExpression);

      var invocationType = _typeProvider.GetInvocationType();
      var invocationConstructor = invocationType.GetConstructors().Single();
      var invocationCreateExpression = Expression.New (
          invocationConstructor,
          invocationContextVariableExpression,
          delegateFieldExpression);

      var aspectAccessExpression = Expression.ArrayAccess (aspectsFieldExpression, Expression.Constant (0));
      var aspectConvertExpression = Expression.Convert (aspectAccessExpression, typeof (MethodInterceptionAspectAttribute));
      var aspectCallMethodInfo = typeof (MethodInterceptionAspectAttribute).GetMethod ("OnIntercept");
      var aspectCallExpression = Expression.Call (aspectConvertExpression, aspectCallMethodInfo, invocationCreateExpression);

      return Expression.Block (
          new[] { invocationContextVariableExpression },
          invocationContextAssignExpression,
          aspectCallExpression);
    }


    private MutableMethodInfo GetCopiedMethod (MutableType mutableType, MutableMethodInfo mutableMethod)
    {
      return mutableType.AddMethod (
          "_m_" + mutableMethod.Name + "_Copy",
          MethodAttributes.Private,
          mutableMethod.ReturnType,
          ParameterDeclaration.CreateForEquivalentSignature (mutableMethod),
          ctx => ctx.GetCopiedMethodBody (mutableMethod, ctx.Parameters.Cast<Expression>()));
    }
  }
}