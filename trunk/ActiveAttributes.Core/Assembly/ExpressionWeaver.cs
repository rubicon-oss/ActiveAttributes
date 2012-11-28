﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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

using System.Collections.Generic;
using ActiveAttributes.Advices;
using ActiveAttributes.Pointcuts;
using Microsoft.Scripting.Ast;

namespace ActiveAttributes.Assembly
{
  public class ExpressionAdvice
  {
    private readonly Expression _method;
    private readonly IEnumerable<IPointcut> _pointcuts;

    public ExpressionAdvice (Expression method, IEnumerable<IPointcut> pointcuts)
    {
      _method = method;
      _pointcuts = pointcuts;
    }

    public Expression Method
    {
      get { return _method; }
    }

    public IEnumerable<IPointcut> Pointcuts
    {
      get { return _pointcuts; }
    }
  }
  public class ExpressionWeaver : ExpressionVisitor
  {
    private readonly IEnumerable<ExpressionAdvice> _advices;

    public ExpressionWeaver (IEnumerable<ExpressionAdvice> advices)
    {
      _advices = advices;
    }


    protected override CatchBlock VisitCatchBlock (CatchBlock node)
    {


      return base.VisitCatchBlock (node);
    }
  }
}