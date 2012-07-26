// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Extensions;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Introduces fields for storing <see cref="MethodInfo"/>, <see cref="Delegate"/>, static/instance <see cref="AspectAttribute"/>'s for a given <see cref="MutableType"/>, or <see cref="MutableMethodInfo"/>.
  /// </summary>
  public class FieldIntroducer
  {
    private const string c_instancePrefix = "_m_";
    private const string c_staticPrefix = "_s_";

    public Data IntroduceAssemblyLevelFields (MutableType mutableType)
    {
      var instanceAspectFieldName = c_instancePrefix + "AssemblyLevel_InstanceAspects";
      var instanceAspectsField = mutableType.AddField (typeof (AspectAttribute[]), instanceAspectFieldName);
      
      return new Data { InstanceAspectsField = instanceAspectsField };
    }

    public Data IntroduceTypeLevelFields (MutableType mutableType)
    {
      var instanceAspectsFieldName = c_instancePrefix + "TypeLevel_InstanceAspects";
      var instanceAspectsField = mutableType.AddField (typeof (AspectAttribute[]), instanceAspectsFieldName);

      var staticAspectsFieldName = c_staticPrefix + "TypeLevel_StaticAspects";
      var staticAspectsField = mutableType.AddField (typeof (AspectAttribute[]), staticAspectsFieldName, FieldAttributes.Static | FieldAttributes.Private);

      return new Data
             {
               StaticAspectsField = staticAspectsField,
               InstanceAspectsField = instanceAspectsField
             };
    }

    public Data IntroduceMethodLevelFields (MutableMethodInfo mutableMethod)
    {
      var mutableType = (MutableType) mutableMethod.DeclaringType;
      var uniqueMethodName = mutableMethod.Name + Guid.NewGuid ();

      var methodInfoFieldName = c_instancePrefix + uniqueMethodName + "_MethodInfo";
      var methodInfoField = mutableType.AddField (typeof (MethodInfo), methodInfoFieldName);

      var delegateType = mutableMethod.GetDelegateType ();
      var delegateFieldName = c_instancePrefix + uniqueMethodName + "_Delegate";
      var delegateField = mutableType.AddField (delegateType, delegateFieldName);

      var staticAspectsFieldName = c_staticPrefix + uniqueMethodName + "_StaticAspects";
      var staticAspectsField = mutableType.AddField (typeof (AspectAttribute[]), staticAspectsFieldName, FieldAttributes.Static | FieldAttributes.Private);

      var instanceAspectsFieldName = c_instancePrefix + uniqueMethodName + "_InstanceAspects";
      var instanceAspectsField = mutableType.AddField (typeof (AspectAttribute[]), instanceAspectsFieldName);

      return new Data
      {
        MethodInfoField = methodInfoField,
        DelegateField = delegateField,
        StaticAspectsField = staticAspectsField,
        InstanceAspectsField = instanceAspectsField
      };
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