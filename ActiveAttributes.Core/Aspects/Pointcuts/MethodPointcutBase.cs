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
using ActiveAttributes.Model.Pointcuts;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Aspects.Pointcuts
{
  public abstract class MethodPointcutBase : PointcutAttributeBase
  {
    protected MethodPointcutBase (IPointcut pointcut)
        : base (pointcut) {}

    public Type ReturnType { get; set; }

    public Type[] ArgumentTypes { get; set; }

    public string Name { get; set; }

    public override IPointcut Pointcut
    {
      get
      {
        var pointcuts = new List<IPointcut>();

        if (ReturnType != null)
          pointcuts.Add (new ReturnTypePointcut (ReturnType));

        if (Name != null)
          pointcuts.Add (new MemberNamePointcut (Name));

        return new AllPointcut (pointcuts.Concat (base.Pointcut));
      }
    }
  }
}