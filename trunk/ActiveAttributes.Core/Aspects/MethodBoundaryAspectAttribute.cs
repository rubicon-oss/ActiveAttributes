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
using ActiveAttributes.Core.Invocations;

namespace ActiveAttributes.Core.Aspects
{
  /// <summary>
  ///   Provides common pointcuts for the execution of methods. This aspect provides observing functionality only.
  /// </summary>
  [Serializable]
  [AttributeUsage (AttributeTargets.Method)]
  public class MethodBoundaryAspectAttribute : MethodInterceptionAspectAttribute
  {
    public override sealed void OnIntercept (IInvocation invocation)
    {
      var readOnlyInvocation = (IReadOnlyInvocation) invocation;
      WrapIfThrowing (() => OnEntry (readOnlyInvocation));

      try
      {
        invocation.Proceed();
        WrapIfThrowing (() => OnSuccess (readOnlyInvocation, invocation.Context.ReturnValue));
      }
      catch (Exception exception)
      {
        WrapIfThrowing (() => OnException (readOnlyInvocation, exception));
        throw;
      }
      finally
      {
        WrapIfThrowing (() => OnExit (readOnlyInvocation));
      }
    }

    protected virtual void OnEntry (IReadOnlyInvocation invocationInfo)
    {
    }

    protected virtual void OnSuccess (IReadOnlyInvocation invocationInfo, object returnValue)
    {
    }

    protected virtual void OnException (IReadOnlyInvocation invocationInfo, Exception exception)
    {
    }

    protected virtual void OnExit (IReadOnlyInvocation invocationInfo)
    {
    }

    private void WrapIfThrowing (Action action)
    {
      try
      {
        action();
      }
      catch (Exception exception)
      {
        throw new AspectInvocationException (exception);
      }
    }
  }
}