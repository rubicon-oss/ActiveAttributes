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
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Model
{
  public class JoinPoint
  {
    private readonly MutableType _declaringType;
    private readonly MutableMethodInfo _method;
    private readonly Expression _expression;

    public JoinPoint (MutableType declaringType)
    {
      _declaringType = declaringType;
    }

    public JoinPoint (MutableMethodInfo method, Expression expression)
      : this ((MutableType) method.DeclaringType)
    {
      _method = method;
      _expression = expression;
    }

    public MutableType DeclaringType
    {
      get { return _declaringType; }
    }

    public MutableMethodInfo Method
    {
      get { return _method; }
    }

    public Expression Expression
    {
      get { return _expression; }
    }

    public ThisExpression This
    {
      get { return new ThisExpression (_declaringType); }
    }

    public IEnumerable<ParameterExpression> Parameters
    {
      get { return _method.ParameterExpressions; }
    }
  }
}