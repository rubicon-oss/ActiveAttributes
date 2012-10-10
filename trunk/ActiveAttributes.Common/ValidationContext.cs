using System;
using System.Reflection;
using ActiveAttributes.Core.Contexts;
using XValidation;
using System.Linq;

namespace ActiveAttributes.Common
{
  /// <summary>
  /// Wraps an <see cref="IInvocationContext"/> into an <see cref="IValidationContext"/>.
  /// </summary>
  public class ValidationContext : IValidationContext
  {
    private readonly IInvocationContext _invocationContext;

    public ValidationContext (IInvocationContext invocationContext)
    {
      _invocationContext = invocationContext;
    }

    public MethodBase Method
    {
      get { return _invocationContext.MethodInfo; }
    }

    public object Instance
    {
      get { return _invocationContext.Instance; }
    }

    public object[] Arguments
    {
      get { return _invocationContext.Arguments.Select(x => x).ToArray(); }
    }

    public object ReturnValue
    {
      get { return _invocationContext.ReturnValue; }
    }
  }
}