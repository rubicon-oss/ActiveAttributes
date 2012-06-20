using System;
using System.Runtime.Serialization;
using ActiveAttributes.Core.Invocation;

namespace ActiveAttributes.Core.Aspects
{
  [Serializable]
  [AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
  public abstract class MethodInterceptionAspectAttribute : AspectAttribute
  {
    protected MethodInterceptionAspectAttribute () {}

    protected MethodInterceptionAspectAttribute (SerializationInfo info, StreamingContext context)
        : base (info, context) {}

    public abstract void OnIntercept (IInvocation invocation);
  }
}