using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ActiveAttributes.Core
{
  [Serializable]
  [AttributeUsage (AttributeTargets.Method, AllowMultiple = true)]
  public class Aspect : Attribute, ISerializable
  {
    public Aspect ()
    {
    }

    protected Aspect (SerializationInfo info, StreamingContext context)
    {
      Scope = (AspectScope) info.GetInt32 ("scope");
      Priority = info.GetInt32 ("priority");
    }

    [SecurityPermissionAttribute (SecurityAction.Demand, SerializationFormatter = true)]
    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("scope", (int) Scope);
      info.AddValue ("priority", Priority);
    }

    public AspectScope Scope { get; set; }

    public int Priority { get; set; }

    public object Clone ()
    {
      return MemberwiseClone ();
    }

    public virtual void OnInvoke (Invocation invocation)
    {
      invocation.Proceed();
    }
  }
}