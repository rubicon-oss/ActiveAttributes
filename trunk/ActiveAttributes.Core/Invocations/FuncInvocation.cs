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
// 
using System;
using ActiveAttributes.Core.Contexts;

namespace ActiveAttributes.Core.Invocations
{
  public class FuncInvocation<TInstance, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TR> _context;
    private readonly Func<TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TR> context, Func<TR> func)
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
      _context.ReturnValue = _func ();
    }
  }

  public class FuncInvocation<TInstance, TA0, TR> : Invocation
  {
    private readonly FuncInvocationContext<TInstance, TA0, TR> _context;
    private readonly Func<TA0, TR> _func;

    public FuncInvocation (FuncInvocationContext<TInstance, TA0, TR> context, Func<TA0, TR> func)
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
      _context.ReturnValue = _func (_context.Arg0);
    }
  }
}