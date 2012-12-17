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
using Microsoft.Scripting.Ast;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;
using System.Linq;
using Remotion.TypePipe.MutableReflection.BodyBuilding;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (EventMethodPreparer))]
  public interface IEventMethodPreparer
  {
    void Prepare (MutableType mutableType, EventInfo eventInfo);
  }

  public class EventMethodPreparer : IEventMethodPreparer
  {
    private static readonly MethodInfo s_combineMethod = typeof (Delegate).GetMethod ("Combine", new[] { typeof (Delegate), typeof (Delegate) });
    private static readonly MethodInfo s_removeMethod = typeof (Delegate).GetMethod ("Remove", new[] { typeof (Delegate), typeof (Delegate) });

    public void Prepare (MutableType mutableType, EventInfo eventInfo)
    {
      var eventType = eventInfo.EventHandlerType;
      var eventInvoke = eventType.GetMethod ("Invoke");

      var original = Expression.Field (
          new ThisExpression (mutableType),
          mutableType.GetField (eventInfo.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
      var broker = Expression.Field (
          new ThisExpression (mutableType),
          mutableType.AddField (eventInfo.Name + "Broker", eventType));

      var addMethod = mutableType.GetOrAddMutableMethod (eventInfo.GetAddMethod (true));
      var removeMethod = mutableType.GetOrAddMutableMethod (eventInfo.GetRemoveMethod (true));

      var parameters = eventInvoke.GetParameters().Select (ParameterDeclaration.CreateEquivalent);
      var invokeMethod = mutableType.AddMethod (
          "invoke_" + eventInfo.Name,
          MethodAttributes.Private,
          eventInvoke.ReturnType,
          parameters,
          ctx => Expression.Call (broker, eventInvoke, ctx.Parameters.Cast<Expression>()));

      addMethod.SetBody (ctx => CreateAddMethod (original, eventType, ctx, invokeMethod, broker, addMethod));
      removeMethod.SetBody (ctx => CreateRemoveBlock (broker, ctx, eventType, original));
    }

    private static BlockExpression CreateAddMethod (
        MemberExpression original,
        Type eventType,
        MethodBodyModificationContext ctx,
        MutableMethodInfo invokeMethod,
        MemberExpression broker,
        MutableMethodInfo addMethod)
    {
      var newDelegateExpression = new NewDelegateExpression (eventType, ctx.This, invokeMethod);
      var baseCall = Expression.Call (ctx.This, new NonVirtualCallMethodInfoAdapter (addMethod.UnderlyingSystemMethodInfo), newDelegateExpression);
      var condition = Expression.IfThen (
          Expression.Equal (original, Expression.Constant (null, eventType)),
          baseCall);
      var binaryExpression = Expression.Assign (
          broker,
          Expression.Convert (Expression.Call (null, s_combineMethod, new Expression[] { broker, ctx.Parameters.Single() }), eventType));
      return Expression.Block (
          baseCall,
          binaryExpression);
    }

    private static BlockExpression CreateRemoveBlock (MemberExpression broker, MethodBodyModificationContext ctx, Type eventType, MemberExpression original)
    {
      return Expression.Block (
          Expression.Assign (
              broker,
              Expression.Convert(Expression.Call (null, s_removeMethod, new Expression[] { broker, ctx.Parameters.Single() }), eventType)),
          Expression.IfThen (
              Expression.Equal (broker, Expression.Constant (null, eventType)),
              Expression.Assign (original, Expression.Constant (null, eventType))));
    }
  }
}