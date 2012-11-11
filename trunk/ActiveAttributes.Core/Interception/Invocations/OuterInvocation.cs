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

namespace ActiveAttributes.Interception.Invocations
{
  public class OuterInvocation : IInvocation
  {
    private readonly IInvocationContext _context;
    private readonly Action _innerMethod;

    public OuterInvocation (IInvocationContext context, Action innerMethod)
    {
      _context = context;
      _innerMethod = innerMethod;
    }

    public MemberInfo MemberInfo
    {
      get { return _context.MemberInfo; }
    }

    public object Instance
    {
      get { return _context.MemberInfo; }
    }

    public IArgumentCollection Arguments
    {
      get { return _context.Arguments; }
    }

    object IInvocationContext.ReturnValue
    {
      get { return _context.ReturnValue; }
      set { _context.ReturnValue = value; }
    }

    IReadOnlyArgumentCollection IReadOnlyInvocationContext.Arguments
    {
      get { return _context.Arguments; }
    }

    object IReadOnlyInvocationContext.ReturnValue
    {
      get { return _context.ReturnValue; }
    }

    public void Proceed ()
    {
      _innerMethod();
    }
  }
}