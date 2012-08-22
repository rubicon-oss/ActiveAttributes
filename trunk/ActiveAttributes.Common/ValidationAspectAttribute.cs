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
using System.Collections.Generic;
using System.Linq;
using ActiveAttributes.Common.Validation;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Common
{
  public class ValidationAspectAttribute : MethodInterceptionAspectAttribute
  {
    public ValidationAspectAttribute ()
    {
    }

    public override void OnIntercept (IInvocation invocation)
    {
      ValidateArguments(invocation.Context);

      invocation.Proceed();

      ValidateReturnValue(invocation.Context);
    }

    private void ValidateArguments (IInvocationContext context)
    {
      var parameterInfos = context.MethodInfo.GetParameters ();
      var infoAndValues = parameterInfos.Zip (context.Arguments, (x, y) => new { Info = x, Value = y });
      foreach (var infoValue in infoAndValues)
      {
        var validators = infoValue.Info.GetCustomAttributes (typeof (ValidatorBase), true).Cast<ValidatorBase> ();
        var sortedValidators = Sort (validators);
        foreach (var validator in sortedValidators)
          validator.Validate (infoValue.Info.Name, infoValue.Value);
      }
    }

    private void ValidateReturnValue (IInvocationContext context)
    {
      var returnParameter = context.MethodInfo.ReturnParameter;

      if (returnParameter == null || returnParameter.ParameterType == typeof (void))
        return;

      var validators = returnParameter.GetCustomAttributes (typeof (ValidatorBase), true).Cast<ValidatorBase> ();
      var sortedValidators = Sort (validators);
      foreach (var validator in sortedValidators)
        validator.Validate ("return", context.ReturnValue);
    }

    private IEnumerable<ValidatorBase> Sort (IEnumerable<ValidatorBase> validators)
    {
      var list = validators.ToList();
      return list.OfType<NotNullAttribute>()
          .Cast<ValidatorBase>()
          .Concat (list.Where (x => !typeof (NotNullAttribute).IsAssignableFrom (x.GetType())));
    }
  }
}