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
using System.Reflection;
using ActiveAttributes.Aspects;
using ActiveAttributes.Model.Ordering;
using ActiveAttributes.Model.Pointcuts;
using Remotion.Utilities;

namespace ActiveAttributes.Model
{
  public class Advice : AspectElementBase, ICrosscutting
  {
    private readonly MethodInfo _method;
    private readonly AdviceExecution _execution;
    private readonly ICrosscutting _crosscutting;

    public Advice (MethodInfo method, AdviceExecution execution, Aspect aspect, ICrosscutting crosscutting)
      : base (aspect)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      ArgumentUtility.CheckNotNull ("aspect", aspect);
      ArgumentUtility.CheckNotNull ("crosscutting", crosscutting);

      _method = method;
      _execution = execution;
      _crosscutting = crosscutting;
    }

    public string Name
    {
      get { return _method.Name; }
    }

    public MethodInfo Method
    {
      get { return _method; }
    }

    public AdviceExecution Execution
    {
      get { return _execution; }
    }

    public IPointcut Pointcut
    {
      get { return _crosscutting.Pointcut; }
    }

    public IEnumerable<IOrdering> Orderings
    {
      get { return _crosscutting.Orderings; }
    }

    Type ICrosscutting.Type
    {
      get { throw new NotImplementedException(); }
    }

    string ICrosscutting.Role
    {
      get { throw new NotImplementedException(); }
    }

    int ICrosscutting.Priority
    {
      get { throw new NotImplementedException(); }
    }
  }
}