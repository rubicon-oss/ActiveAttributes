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

namespace ActiveAttributes.Pointcuts
{
  public interface IMethodSignaturePointcut : IArgumentTypesPointcut, IReturnTypePointcut {}

  public class MethodSignaturePointcut : IMethodSignaturePointcut
  {
    private readonly Type[] _argumentTypes;
    private readonly Type _returnType;

    public MethodSignaturePointcut (Type[] argumentTypes, Type returnType)
    {
      _argumentTypes = argumentTypes;
      _returnType = returnType;
    }

    public Type[] ArgumentTypes
    {
      get { return _argumentTypes; }
    }

    public Type ReturnType
    {
      get { return _returnType; }
    }

    public bool Accept (IPointcutEvaluator evaluator, JoinPoint joinPoint)
    {
      return (_argumentTypes == null || evaluator.Visit ((IArgumentTypesPointcut) this, joinPoint)) &&
             (_returnType == null || evaluator.Visit ((IReturnTypePointcut) this, joinPoint));
    }
  }

  public class MethodSignaturePointcutAttribute : IPointcutAttribute
  {
    public Type[] ArgumentTypes { get; set; }
    public Type ReturnType { get; set; }

    public IPointcut Pointcut
    {
      get { return new MethodSignaturePointcut (ArgumentTypes, ReturnType); }
    }
  }
}