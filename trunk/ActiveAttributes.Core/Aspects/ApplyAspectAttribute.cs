using System;

namespace ActiveAttributes.Core.Aspects
{
  [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
  public class ApplyAspectAttribute : Attribute
  {
    public ApplyAspectAttribute (Type aspectType, params object[] arguments)
    {
      AspectType = aspectType;
      Arguments = arguments;
    }

    public Type AspectType { get; private set; }
    public object[] Arguments { get; private set; }
  }
}