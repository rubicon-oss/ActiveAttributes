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
using ActiveAttributes.Ordering;
using ActiveAttributes.Weaving;
using Remotion.Utilities;

namespace ActiveAttributes.Infrastructure.Ordering
{
  public class AdviceNameOrdering : OrderingBase
  {
    private readonly string _beforeAdvice;
    private readonly string _afterAdvice;

    public AdviceNameOrdering (string beforeAdvice, string afterAdvice, string source)
        : base (ArgumentUtility.CheckNotNullOrEmpty ("source", source))
    {
      ArgumentUtility.CheckNotNullOrEmpty ("beforeAdvice", beforeAdvice);
      ArgumentUtility.CheckNotNullOrEmpty ("afterAdvice", afterAdvice);

      _beforeAdvice = beforeAdvice;
      _afterAdvice = afterAdvice;
    }

    public string BeforeAdvice
    {
      get { return _beforeAdvice; }
    }

    public string AfterAdvice
    {
      get { return _afterAdvice; }
    }

    public override bool Accept (CrosscuttingDependencyProvider dependencyProvider, ICrosscutting crosscutting1, ICrosscutting crosscutting2)
    {
      return dependencyProvider.VisitAdviceName (this, crosscutting1, crosscutting2);
    }
  }
}