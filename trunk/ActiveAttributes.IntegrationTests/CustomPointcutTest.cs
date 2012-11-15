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
using ActiveAttributes.Assembly;
using ActiveAttributes.Pointcuts;

namespace ActiveAttributes.IntegrationTests
{
  public class CustomPointcutTest
  {
    public class RelaxedPointcutAttribute : PointcutAttributeBase
    {
      public RelaxedPointcutAttribute (Type type, Type returnType)
          : base (new RelaxedPointcut (type, returnType)) {}
    }

    public class RelaxedPointcut : IPointcut, ITypePointcut, IReturnTypePointcut
    {
      public RelaxedPointcut (Type type, Type returnType)
      {
        Type = type;
        ReturnType = returnType;
      }

      public bool MatchVisit (IPointcutEvaluator evaluator, JoinPoint joinPoint)
      {
        return evaluator.MatchesType (this, joinPoint) || evaluator.MatchesReturnType (this, joinPoint);
      }

      public Type Type { get; private set; }
      public Type ReturnType { get; private set; }
    }
  }
}