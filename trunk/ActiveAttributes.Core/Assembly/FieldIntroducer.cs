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

using System;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Extensions;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Core.Assembly
{
  public class FieldIntroducer : IFieldIntroducer
  {
    private int _counter = 1;
    private const string c_instancePrefix = "_m_";
    private const string c_staticPrefix = "_s_";

    public Data IntroduceTypeAspectFields (MutableType mutableType)
    {
      var instanceAspectsFieldName = c_instancePrefix + "TypeLevel_InstanceAspects";
      var instanceAspectsField = mutableType.AddField (typeof (AspectAttribute[]), instanceAspectsFieldName);

      var staticAspectsFieldName = c_staticPrefix + "TypeLevel_StaticAspects";
      var staticAspectsField = mutableType.AddField (
          typeof (AspectAttribute[]), staticAspectsFieldName, FieldAttributes.Static | FieldAttributes.Private);

      return new Data
             {
                 StaticAspectsField = staticAspectsField,
                 InstanceAspectsField = instanceAspectsField
             };
    }

    public Data IntroduceMethodAspectFields (MutableType mutableType, MethodInfo methodInfo)
    {
      var uniqueMethodName = methodInfo.Name + _counter++;

      var staticAspectsFieldName = c_staticPrefix + uniqueMethodName + "_StaticAspects";
      var staticAspectsField = mutableType.AddField (
          typeof (AspectAttribute[]), staticAspectsFieldName, FieldAttributes.Static | FieldAttributes.Private);

      var instanceAspectsFieldName = c_instancePrefix + uniqueMethodName + "_InstanceAspects";
      var instanceAspectsField = mutableType.AddField (typeof (AspectAttribute[]), instanceAspectsFieldName);

      return new Data
      {
        StaticAspectsField = staticAspectsField,
        InstanceAspectsField = instanceAspectsField
      };
    }

    public Data IntroduceMethodReflectionFields (MutableType mutableType, MethodInfo methodInfo)
    {
      var uniqueMethodName = methodInfo.Name + _counter++;
      var instanceBaseName = c_instancePrefix + uniqueMethodName;

      var propertyInfoFieldName = instanceBaseName + "_PropertyInfo";
      var propertyInfoField = mutableType.AddField (typeof (PropertyInfo), propertyInfoFieldName);

      var eventInfoFieldName = instanceBaseName + "_EventInfo";
      var eventInfoField = mutableType.AddField (typeof (EventInfo), eventInfoFieldName);

      var methodInfoFieldName = instanceBaseName + "_MethodInfo";
      var methodInfoField = mutableType.AddField (typeof (MethodInfo), methodInfoFieldName);

      var delegateType = methodInfo.GetDelegateType ();
      var delegateFieldName = instanceBaseName + "_Delegate";
      var delegateField = mutableType.AddField (delegateType, delegateFieldName);

      return new Data
      {
        PropertyInfoField = propertyInfoField,
        EventInfoField = eventInfoField,
        MethodInfoField = methodInfoField,
        DelegateField = delegateField,
      };
    }

    public struct Data
    {
      public FieldInfo PropertyInfoField;
      public FieldInfo EventInfoField;
      public FieldInfo MethodInfoField;
      public FieldInfo DelegateField;
      public FieldInfo StaticAspectsField;
      public FieldInfo InstanceAspectsField;
    }
  }
}