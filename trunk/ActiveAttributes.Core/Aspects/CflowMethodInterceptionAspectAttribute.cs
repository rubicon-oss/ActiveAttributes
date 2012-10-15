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
using System.Diagnostics;
using ActiveAttributes.Core.Invocations;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Aspects
{
  public abstract class CflowMethodInterceptionAspectAttribute : MethodInterceptionAspectAttribute
  {
    public string ExecutionOf { get; set; }

    public override sealed void OnIntercept (IInvocation invocation)
    {
      var stackTrace = new StackTrace();
      var stackFrames = stackTrace.GetFrames();
      Assertion.IsNotNull (stackFrames);

      foreach (var stackFrame in stackFrames)
      {
        var method = stackFrame.GetMethod();
        Assertion.IsNotNull (method.DeclaringType);

        var fullMethodName = string.Format ("{0}.{1}", method.DeclaringType.Name, method.Name);
        if (fullMethodName == ExecutionOf)
        {
          OnCflowIntercept (invocation);
          return;
        }
      }

      invocation.Proceed();
    }

    protected abstract void OnCflowIntercept (IInvocation invocation);
  }

  class MethodInterceptionCflowDecorator : MethodInterceptionAspectAttribute
  {
    private readonly MethodInterceptionAspectAttribute _methodInterceptionAspect;

    public MethodInterceptionCflowDecorator (MethodInterceptionAspectAttribute methodInterceptionAspect)
    {
      _methodInterceptionAspect = methodInterceptionAspect;
    }

    public override void OnIntercept (IInvocation invocation)
    {
      throw new NotImplementedException();
    }
  }
}