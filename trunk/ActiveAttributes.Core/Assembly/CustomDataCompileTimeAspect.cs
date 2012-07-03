// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;

namespace ActiveAttributes.Core.Assembly
{
  public enum CompileTimeAspectType
  {
    CustomDataCompileTimeAspect,
    TypeArgsCompileTimeAspect
  }

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

  public class CustomDataCompileTimeAspect : CompileTimeAspectBase
  {
    private readonly CustomAttributeData _customData;

    public CustomDataCompileTimeAspect (CustomAttributeData customData)
    {
      if (!typeof (AspectAttribute).IsAssignableFrom (customData.Constructor.DeclaringType))
        throw new ArgumentException ("CustomAttributeData must be from an AspectAttribute");

      _customData = customData;
    }

    public override CompileTimeAspectType CompileTimeType
    {
      get { return CompileTimeAspectType.CustomDataCompileTimeAspect; }
    }

    public override int Priority
    {
      get
      {
        // TODO: review
        var priorityArgument = _customData.NamedArguments.SingleOrDefault (x => x.MemberInfo.Name == "Priority");
        return priorityArgument.MemberInfo != null
                   ? (int) priorityArgument.TypedValue.Value
                   : 0;
      }
    }

    public override AspectScope Scope
    {
      get
      {
        var priorityArgument = _customData.NamedArguments.SingleOrDefault (x => x.MemberInfo.Name == "Scope");
        return priorityArgument.MemberInfo != null
                   ? (AspectScope) priorityArgument.TypedValue.Value
                   : 0;
      }
    }

    public override Type AspectType
    {
      get { return _customData.Constructor.DeclaringType; }
    }

    public override ConstructorInfo ConstructorInfo
    {
      get { return _customData.Constructor; }
    }
    public override IList<CustomAttributeTypedArgument> ConstructorArguments
    {
      get { return _customData.ConstructorArguments; }
    }
    public override IList<CustomAttributeNamedArgument> NamedArguments
    {
      get { return _customData.NamedArguments; }
    }

    public override object[] Arguments
    {
      get { throw new NotSupportedException(); }
    }
  }
}