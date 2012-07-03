using System;
using System.Collections.Generic;
using System.Reflection;
using ActiveAttributes.Core.Configuration;

namespace ActiveAttributes.Core.Assembly.CompileTimeAspects
{
  public class TypeArgsCompileTimeAspect : CompileTimeAspectBase
  {
    private readonly Type _aspectType;
    private readonly object[] _arguments;

    public TypeArgsCompileTimeAspect (Type aspectType, object[] arguments)
    {
      _aspectType = aspectType;
      _arguments = arguments;
    }

    public override CompileTimeAspectType CompileTimeType
    {
      get { return CompileTimeAspectType.TypeArgsCompileTimeAspect; }
    }

    public override int Priority
    {
      get { throw new NotSupportedException(); }
    }

    public override AspectScope Scope
    {
      get { throw new NotImplementedException(); }
    }

    public override Type AspectType
    {
      get { return _aspectType; }
    }

    public override ConstructorInfo ConstructorInfo
    {
      get { throw new NotSupportedException (); }
    }

    public override IList<CustomAttributeTypedArgument> ConstructorArguments
    {
      get { throw new NotSupportedException (); }
    }

    public override IList<CustomAttributeNamedArgument> NamedArguments
    {
      get { throw new NotSupportedException (); }
    }

    public override object[] Arguments
    {
      get { return _arguments; }
    }
  }
}