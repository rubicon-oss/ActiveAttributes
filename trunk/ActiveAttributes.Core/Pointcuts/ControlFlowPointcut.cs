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
using ActiveAttributes.Advices;
using ActiveAttributes.Assembly;
using Remotion.Utilities;

namespace ActiveAttributes.Pointcuts
{
  public interface IControlFlowPointcut : IPointcut
  {
    string ControlFlow { get; }
  }

  public class ControlFlowPointcut : IControlFlowPointcut
  {
    private readonly string _controlFlow;

    public ControlFlowPointcut (string controlFlow)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("controlFlow", controlFlow);

      _controlFlow = controlFlow;
    }

    public string ControlFlow
    {
      get { return _controlFlow; }
    }

    public bool Accept (IPointcutEvaluator evaluator, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("evaluator", evaluator);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return evaluator.Visit (this, joinPoint);
    }
  }

  public class ControlFlowPointcutAttribute : PointcutAttributeBase
  {
    public ControlFlowPointcutAttribute (string controlFlow)
        : base (new ControlFlowPointcut (controlFlow)) {}
  }
}