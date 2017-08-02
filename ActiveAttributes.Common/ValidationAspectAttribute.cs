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
using XValidation;

namespace ActiveAttributes.Common
{
  /// <summary>
  /// Validates arguments and return value of a method call based on <see cref="IValidationAttribute"/>s.
  /// </summary>
  public class ValidationAspectAttribute : MethodInterceptionAspectAttribute
  {
    private static readonly ValidationInfoProvider s_infoProvider = new ValidationInfoProvider();

    private InterceptionValidator _validator;

    public override void OnIntercept (IInvocation invocation)
    {
      if (_validator == null)
        _validator = new InterceptionValidator (s_infoProvider, invocation.Context.MethodInfo);

      var context = new ValidationContext (invocation.Context);

      _validator.ValidateArguments (context);
      invocation.Proceed();
      _validator.ValidateReturnValue (context);
    }
  }
}