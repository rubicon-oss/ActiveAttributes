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
using ActiveAttributes.Core.Contexts;
// ReSharper disable RedundantUsingDirective
using Remotion;
// ReSharper restore RedundantUsingDirective

namespace ActiveAttributes.Core.Invocations
{
  // @begin-template first=1 template=1 generate=0..8 suppressTemplate=true
  // @replace ", TA<n>"
  // @replace "TA<n>, "
  // @replace "_context.Arg<n>" ", "
  public class FuncInvocation<TInstance, TA1, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA1, TR> _context;
    private readonly Func<TA1, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA1, TR> context, Func<TA1, TR> func)
    {
      _context = context;
      _func = func;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      _context.ReturnValue = _func (_context.Arg1);
    }
  }
  // @end-template
}