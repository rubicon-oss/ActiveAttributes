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
using System.Diagnostics;
using System.Reflection;
using ActiveAttributes.Discovery.Construction;
using ActiveAttributes.Pointcuts;

namespace ActiveAttributes.Advices
{
  [DebuggerDisplay("Aspect: {DeclaringType,nq}, Advice = {Method,nq}")]
  public class Advice
  {
    private readonly IConstruction _construction;
    private readonly AdviceExecution _execution;
    private readonly AdviceScope _scope;
    private readonly int _priority;
    private readonly string _role;
    private readonly string _name;
    private readonly MethodInfo _method;
    private readonly IEnumerable<IPointcut> _pointcuts;

    public Advice (
        IConstruction construction,
        MethodInfo method,
        string name,
        string role,
        AdviceExecution execution,
        AdviceScope scope,
        int priority,
        IEnumerable<IPointcut> pointcuts)
    {
      _construction = construction;
      _execution = execution;
      _scope = scope;
      _priority = priority;
      _method = method;
      _pointcuts = pointcuts;
      _role = role;
      _name = name;
    }

    public IConstruction Construction
    {
      get { return _construction; }
    }

    public Type DeclaringType
    {
      get { return _method.DeclaringType; }
    }

    public MethodInfo Method
    {
      get { return _method; }
    }

    public string Name
    {
      get { return _name; }
    }

    public string Role
    {
      get { return _role; }
    }

    public AdviceExecution Execution
    {
      get { return _execution; }
    }

    public AdviceScope Scope
    {
      get { return _scope; }
    }

    public int Priority
    {
      get { return _priority; }
    }

    public IEnumerable<IPointcut> Pointcuts
    {
      get { return _pointcuts; }
    }

    public override string ToString ()
    {
      return _name ?? base.ToString();
    }
  }
}