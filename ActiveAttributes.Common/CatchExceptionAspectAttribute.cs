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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Interception.Invocations;

namespace ActiveAttributes.Common
{
  /// <summary>
  /// Catches exceptions of a certain type; namely <see cref="Exception"/> unless otherwise stated in the constructor.
  /// </summary>
  public class CatchExceptionAspectAttribute : MethodInterceptionAspectAttribute
  {
    private readonly Type _exceptionType;

    public CatchExceptionAspectAttribute (Type exceptionType = null)
    {
      _exceptionType = exceptionType ?? typeof (Exception);
    }

    public override void OnIntercept (IInvocation invocation)
    {
      try
      {
        invocation.Proceed();
      }
      catch (Exception exception)
      {
        if (!_exceptionType.IsInstanceOfType(exception))
          throw;
      }
    }
  }
}