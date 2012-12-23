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
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Weaving.Expressions2
{
  public class MethodExecutionExpression : PrimitiveExpressionBase
  {
    public static MethodExecutionExpression Adapt (MutableMethodInfo method)
    {
      var instance = new ThisExpression (method.DeclaringType);
      var baseMethod = NonVirtualCallMethodInfoAdapter.Adapt (method.UnderlyingSystemMethodInfo);
      var parameters = method.ParameterExpressions.Cast<Expression>();
      var body = Call (instance, baseMethod, parameters);

      return new MethodExecutionExpression (method, body);
    }

    private readonly MutableMethodInfo _method;
    private readonly Expression _body;

    private MethodExecutionExpression (MutableMethodInfo method, Expression body)
        : base (method.ReturnType)
    {
      _method = method;
      _body = body;
    }

    public MutableMethodInfo Method
    {
      get { return _method; }
    }

    public Expression Body
    {
      get { return _body; }
    }

    public override Expression Accept (IPrimitiveExpressionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      return visitor.VisitExecution (this);
    }

    protected override Expression VisitChildren (ExpressionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      var newBody = visitor.Visit (Body);
      if (newBody != Body)
        return new MethodExecutionExpression (Method, newBody);

      return this;
    }
  }
}