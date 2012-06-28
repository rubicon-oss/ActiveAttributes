using System;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Extensions;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Introduces fields for <see cref="MethodInfo"/>, <see cref="Delegate"/>, and static/instance <see cref="AspectAttribute"/>
  /// into a <see cref="MutableType"/>. 
  /// </summary>
  public class FieldIntroducer
  {
    private const string c_instancePrefix = "_m_";
    private const string c_staticPrefix = "_s_";

    public Data Introduce (MutableMethodInfo mutableMethod)
    {
      var mutableType = (MutableType) mutableMethod.DeclaringType;
      var methodToken = GetMethodToken (mutableMethod);

      var methodInfoFieldName = c_instancePrefix + methodToken + "_MethodInfo";
      var methodInfoField = mutableType.AddField (typeof (MethodInfo), methodInfoFieldName);

      var delegateType = mutableMethod.GetDelegateType (mutableMethod.DeclaringType.UnderlyingSystemType);
      var delegateFieldName = c_instancePrefix + methodToken + "_Delegate";
      var delegateField = mutableType.AddField (delegateType, delegateFieldName);


      var staticAspectsFieldName = c_staticPrefix + methodToken + "_StaticAspects";
      var staticAspectsField = mutableType.AddField (typeof (AspectAttribute[]), staticAspectsFieldName, FieldAttributes.Static | FieldAttributes.Private);


      var instanceAspectsFieldName = c_instancePrefix + methodToken + "_InstanceAspects";
      var instanceAspectsField = mutableType.AddField (typeof (AspectAttribute[]), instanceAspectsFieldName);

      return new Data
             {
                 MethodInfoField = methodInfoField,
                 DelegateField = delegateField,
                 StaticAspectsField = staticAspectsField,
                 InstanceAspectsField = instanceAspectsField
             };
    }

    private string GetMethodToken (MutableMethodInfo mutableMethod)
    {
      var mutableType = (MutableType) mutableMethod.DeclaringType;

      var baseName = mutableMethod.Name;
      var i = 0;
      do
      {
        i++;
      } while (mutableType.AddedFields.Any (x => c_instancePrefix + baseName + i + "_MethodInfo" == x.Name));

      return baseName + i;
    }

    public struct Data
    {
      public FieldInfo MethodInfoField;
      public FieldInfo DelegateField;
      public FieldInfo StaticAspectsField;
      public FieldInfo InstanceAspectsField;
    }
  }
}