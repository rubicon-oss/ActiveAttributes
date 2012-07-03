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
  public class ActionInvocation<TInstance> : Invocation
  {
    private readonly ActionInvocationContext<TInstance> _context;
    private readonly Action _action;

    public ActionInvocation (ActionInvocationContext<TInstance> context, Action action)
    {
      _context = context;
      _action = action;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      _action();
    }
  }

  public class ActionInvocation<TInstance, TA0> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA0> _context;
    private readonly Action<TA0> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA0> context, Action<TA0> action)
    {
      _context = context;
      _action = action;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      var context = (ActionInvocationContext<TInstance, TA0>) Context;
      _action (context.Arg0);
    }
  }

  public class ActionInvocation<TInstance, TA0, TA1> : Invocation
  {
    private readonly ActionInvocationContext<TInstance, TA0, TA1> _context;
    private readonly Action<TA0, TA1> _action;

    public ActionInvocation (ActionInvocationContext<TInstance, TA0, TA1> context, Action<TA0, TA1> action)
    {
      _context = context;
      _action = action;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      var context = (ActionInvocationContext<TInstance, TA0, TA1>) Context;
      _action (context.Arg0, context.Arg1);
    }
  }
}