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

namespace ActiveAttributes.Weaving.Expressions
{
  public class MethodExecutionExpression : Expression
  {
    private readonly MethodInfo _methodInfo;
    private readonly Expression _body;

    public MethodExecutionExpression (MethodInfo methodInfo, Expression body)
    {
      _methodInfo = methodInfo;
      _body = body;
    }

    public MethodInfo MethodInfo
    {
      get { return _methodInfo; }
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

    protected override Expression Accept (ExpressionVisitor visitor)
    {
      // TODO visitor
      return _body;
    }
  }
}