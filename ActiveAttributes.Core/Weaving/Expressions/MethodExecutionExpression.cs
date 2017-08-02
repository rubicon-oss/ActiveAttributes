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

using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.Expressions.ReflectionAdapters;
using Remotion.TypePipe.MutableReflection;
using System.Linq;

namespace ActiveAttributes.Weaving.Expressions
{
  public class MethodExecutionExpression : Expression
  {
    private readonly MutableMethodInfo _method;
    private readonly Expression _body;

    public MethodExecutionExpression (MutableMethodInfo method)
    {
      _method = method;
      _body = Call (
          new ThisExpression (method.DeclaringType),
          NonVirtualCallMethodInfoAdapter.Adapt (method.UnderlyingSystemMethodInfo),
          method.ParameterExpressions.Cast<Expression>());
    }

    public MethodInfo Method
    {
      get { return _method; }
    }

    public Expression Body
    {
      get { return _body; }
    }

    public override bool CanReduce
    {
      get { return true; }
    }

    public override Expression Reduce ()
    {
      return _body;
    }

    public override ExpressionType NodeType
    {
      get { return ExpressionType.Extension; }
    }

    protected override Expression VisitChildren (ExpressionVisitor visitor)
    {
      return base.VisitChildren (visitor);
    }

    protected override Expression Accept (ExpressionVisitor visitor)
    {
      return base.Accept (visitor);
    }

    public override string ToString ()
    {
      return "muh";
    }

    public override System.Type Type
    {
      get { return _method.ReturnType; }
    }
  }
}