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
using System.Reflection;
using ActiveAttributes.Aspects;
using ActiveAttributes.Weaving.Context;

namespace ActiveAttributes.Weaving.Invocation
{
  public class StaticInvocation : IInvocation
  {
    private readonly IContext _context;
    private readonly Action _innerInvocation;

    public StaticInvocation (IContext context, Action innerInvocation)
    {
      _context = context;
      _innerInvocation = innerInvocation;
    }

    public MemberInfo MemberInfo
    {
      get { return _context.MemberInfo; }
    }

    public object Instance
    {
      get { return _context.Instance; }
    }

    public IArgumentCollection Arguments
    {
      get { return _context.Arguments; }
    }

    public object ReturnValue
    {
      get { return _context.ReturnValue; }
      set { _context.ReturnValue = value; }
    }

    IReadOnlyArgumentCollection IReadOnlyContext.Arguments
    {
      get { return _context.Arguments; }
    }

    object IReadOnlyContext.ReturnValue
    {
      get { return _context.ReturnValue; }
    }

    public void Proceed ()
    {
      _innerInvocation();
    }
  }
  public class StaticInvocation<T> : IInvocation where T : IContext
  {
    private readonly T _context;
    private readonly Action<T> _innerInvocation;

    public StaticInvocation (T context, Action<T> innerInvocation)
    {
      _context = context;
      _innerInvocation = innerInvocation;
    }

    public MemberInfo MemberInfo
    {
      get { return _context.MemberInfo; }
    }

    public object Instance
    {
      get { return _context.Instance; }
    }

    public IArgumentCollection Arguments
    {
      get { return _context.Arguments; }
    }

    public object ReturnValue
    {
      get { return _context.ReturnValue; }
      set { _context.ReturnValue = value; }
    }

    IReadOnlyArgumentCollection IReadOnlyContext.Arguments
    {
      get { return _context.Arguments; }
    }

    object IReadOnlyContext.ReturnValue
    {
      get { return _context.ReturnValue; }
    }

    public void Proceed ()
    {
      _innerInvocation (_context);
    }
  }
}