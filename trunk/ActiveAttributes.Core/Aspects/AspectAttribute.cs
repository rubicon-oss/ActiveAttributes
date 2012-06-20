using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using ActiveAttributes.Core.Configuration;

namespace ActiveAttributes.Core.Aspects
{
  public abstract class AspectAttribute : Attribute, ISerializable
  {
    protected AspectAttribute () {}

    #region ISerializable members

    protected AspectAttribute (SerializationInfo info, StreamingContext context)
    {
      Scope = (AspectScope) info.GetInt32 ("scope");
      Priority = info.GetInt32 ("priority");
    }

    [SecurityPermission (SecurityAction.Demand, SerializationFormatter = true)]
    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("scope", (int) Scope);
      info.AddValue ("priority", Priority);
    }

    #endregion

    #region IClonable members

    public object Clone ()
    {
      return MemberwiseClone();
    }

    #endregion

    public AspectScope Scope { get; set; }

    public int Priority { get; set; }

    public virtual bool Validate (MethodInfo method)
    {
      return true;
    }
  }
}