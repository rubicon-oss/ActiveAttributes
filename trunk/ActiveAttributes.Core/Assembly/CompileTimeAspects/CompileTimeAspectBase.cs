using System;
using System.Collections.Generic;
using System.Reflection;
using ActiveAttributes.Core.Configuration;

namespace ActiveAttributes.Core.Assembly.CompileTimeAspects
{
  public abstract class CompileTimeAspectBase
  {
    public abstract CompileTimeAspectType CompileTimeType { get; }
    public abstract int Priority { get; }
    public abstract AspectScope Scope { get; }
    public abstract Type AspectType { get; }
    public abstract ConstructorInfo ConstructorInfo { get; }
    public abstract IList<CustomAttributeTypedArgument> ConstructorArguments { get; }
    public abstract IList<CustomAttributeNamedArgument> NamedArguments { get; }
    public abstract object[] Arguments { get; }
  }
}