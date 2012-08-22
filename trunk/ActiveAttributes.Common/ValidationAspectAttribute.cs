//Sample license text.

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
        var validators = infoValue.Info.GetCustomAttributes (typeof (IValidator), true).Cast<IValidator>();
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

      var validators = returnParameter.GetCustomAttributes (typeof (IValidator), true).Cast<IValidator>();
      var sortedValidators = Sort (validators);
      foreach (var validator in sortedValidators)
        validator.Validate ("return", context.ReturnValue);
    }

    private IEnumerable<IValidator> Sort (IEnumerable<IValidator> validators)
    {
      var list = validators.ToList();
      return list.OfType<NotNullAttribute> ().Cast<IValidator>().Concat(list.Where(x => !typeof(NotNullAttribute).IsAssignableFrom(x.GetType())));
    }
  }
}