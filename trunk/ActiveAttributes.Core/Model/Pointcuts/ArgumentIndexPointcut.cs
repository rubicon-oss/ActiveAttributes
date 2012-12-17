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
using ActiveAttributes.Weaving;

namespace ActiveAttributes.Model.Pointcuts
{
  public class ArgumentIndexPointcut : IPointcut
  {
    private readonly Type _argumentType;
    private readonly int _index;

    public ArgumentIndexPointcut (Type argumentType, int index)
    {
      _argumentType = argumentType;
      _index = index;
    }

    public Type ArgumentType
    {
      get { return _argumentType; }
    }

    public int Index
    {
      get { return _index; }
    }

    public bool Accept (PointcutEvaluator evalutor, JoinPoint joinPoint)
    {
      return evalutor.VisitArgumentIndex (this, joinPoint);
    }
  }
}