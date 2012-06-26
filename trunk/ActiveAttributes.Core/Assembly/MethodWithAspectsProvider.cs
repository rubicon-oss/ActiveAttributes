using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ActiveAttributes.Core.Aspects;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  public class MethodWithAspectsProvider
  {
    public IEnumerable<Tuple<MutableMethodInfo, IEnumerable<AspectAttribute>>> GetMethodsWithAspects (MutableType mutableType)
    {
      foreach (var mutableMethod in mutableType.AllMutableMethods.ToArray())
      {
        var attributes = mutableMethod.GetCustomAttributes (typeof (MethodInterceptionAspectAttribute), true);
        var aspects = new List<AspectAttribute> (attributes.Cast<AspectAttribute>());

        if (mutableMethod.IsCompilerGenerated())
        {
          var propertyName = mutableMethod.Name.Substring (4);
          var propertyInfo = mutableType.UnderlyingSystemType.GetProperty (propertyName);
          // TODO
          //var attributes = CustomAttributeData.GetCustomAttributes (propertyInfo)
          //    .Where (cad => typeof (PropertyInterceptionAspectAttribute).IsAssignableFrom (cad.Constructor.DeclaringType));
          attributes = propertyInfo.GetCustomAttributes (typeof (PropertyInterceptionAspectAttribute), true);
          aspects.AddRange (attributes.Cast<AspectAttribute>());
        }

        if (aspects.Count > 0)
          yield return new Tuple<MutableMethodInfo, IEnumerable<AspectAttribute>> (mutableMethod, aspects);
      }
    }
  }

  public static class MethodInfoExtensions
  {
    public static bool IsCompilerGenerated (this MethodBase methodInfo)
    {
      var customAttributes = methodInfo.GetCustomAttributes (typeof (CompilerGeneratedAttribute), true);
      return customAttributes.Length == 1;
    }
  }
}