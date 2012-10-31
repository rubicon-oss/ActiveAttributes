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
using ActiveAttributes.Core.Interception.Contexts;
using Remotion.Logging;

namespace ActiveAttributes.Core.Interception.Invocations
{
  public class OuterInvocation : Invocation
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (IInvocation));

    private readonly IInvocationContext _context;
    private readonly Action<IInvocation> _innerInterception;
    private readonly IInvocation _innerInvocation;

    public OuterInvocation (IInvocationContext context, Action<IInvocation> innerInterception, IInvocation innerInvocation)
    {
      _context = context;
      _innerInterception = innerInterception;
      _innerInvocation = innerInvocation;
    }

    public override IInvocationContext Context
    {
      get { return _context; }
    }

    public override void Proceed ()
    {
      s_log.DebugFormat ("Proceeding with '{0}'", _innerInterception);
      _innerInterception (_innerInvocation);
    }
  }
}