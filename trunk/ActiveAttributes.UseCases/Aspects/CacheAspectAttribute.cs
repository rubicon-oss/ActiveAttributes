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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.UseCases.Aspects
{

  public class CacheAspectAttribute : MethodInterceptionAspectAttribute
  {
    private DateTime _lastTime;

    public CacheAspectAttribute ()
    {
    }

    public override void OnIntercept (IInvocation invocation)
    {
      var diff = DateTime.Now - _lastTime;
      if (diff >= TimeSpan.FromSeconds (5))
        _lastTime = DateTime.Now;

      invocation.Context.ReturnValue = _lastTime;
    }
  }
}