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
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Responsible for the assembly of types applied with aspects.
  /// </summary>
  public class Assembler : ITypeAssemblyParticipant
  {
    // private Dictionary<Assembly, Aspect[]> _assemblyLevelAspects = ...;


      // var instanceTypeLevelAspects = aspectsProvider.GetAspects (mutableType).Where (a => a.IsInstance);
      // var staticTypeLevelAspects = aspectsProvider.GetAspects (mutableType).Where (a => a.IsStatic);

      // var typeLevelFieldData = fieldIntroducer.Introduce (mutableType); // typeName_StaticAspects, typeName_InstanceAspects
      // var instanceTypeLevelAspectsForCodeGen = instanceTypeLevelAspects.Select ((cta, i) => cta.CreateAspectForCodeGeneration (cta, typeLevelFieldData, i));
      // var staticTypeLevelAspectsForCodeGen = staticTypeLevelAspects.Select ((cta, i) => cta.CreateAspectForCodeGeneration (cta, typeLevelFieldData, i));
      // var typeLevelAspectsForCodeGen = instanceTypeLevelAspectsForCodeGen.Concat (staticTypeLevelAspectsForCodeGen);
      // constructorPatcher.AddFieldInitialization (mutableMethod, typeLevelFieldData, typeLevelAspects);
            
    public void ModifyType (MutableType mutableType)
    {


      var fieldIntroducer = new FieldIntroducer();
      var constructorPatcher = new ConstructorPatcher();
      var methodPatcher = new MethodPatcher();
      var aspectsProvider = new AspectsProvider();
      var methodCopier = new MethodCopier();

      var typeLevelAspects = aspectsProvider.GetTypeLevelAspects (mutableType.UnderlyingSystemType);
      var typeFieldData = fieldIntroducer.IntroduceTypeLevelFields (mutableType);



      // TODO: Use GetMethods instead of AllMutableMethods. Use the MethodInfo (not MutableMethodInfo) to detect aspects on the methods. If there are aspects,
      // TODO: use GetMutableMethod to get the mutable method. If this throws an exception, wrap with a sensible ActiveAttributes configuration exception.
      // TODO: Then check MutableMethodInfo.CanSetBody, also throw a configuration exception if it is false.
      foreach (var mutableMethod in mutableType.AllMutableMethods.ToList())
      {
        var aspects = aspectsProvider.GetAspects (mutableMethod).ToList ();
        // var methodLevelAspects = aspectsProvider.GetAspects (mutableMethod); // TODO: Also needs to differentiate static/instance aspects and associate indexes with them, see above
        if (aspects.Count == 0)
          continue;

        var fieldData = fieldIntroducer.IntroduceMethodLevelFields (mutableMethod);
        // var methodLevelFieldData = fieldIntroducer.Introduce (mutableMethod);
        // constructorPatcher.AddFieldInitialization (mutableMethod, methodLevelFieldData);
        // var methodLevelAspectsForCodeGen = methodLevelAspects.Select (cta => cta.CreateAspectForCodeGeneration (cta, methodLevelFieldData));

        // var allAspectsForCodeGen = typeLevelAspectsForCodeGen.Concat (methodLevelAspectsForCodeGen).Where (aspect => aspect.CompileTimeAspect.Matches (mutableMethod).ToList();


        var copiedMethod = methodCopier.GetCopy (mutableMethod);

        // methodPatcher.Patch (mutableMethod, fieldData, typeLevelFieldData, allAspects);
        methodPatcher.Patch (mutableMethod, fieldData, aspects);
        constructorPatcher.Patch (mutableMethod, aspects, fieldData, copiedMethod);
      }
    }

    // private Aspect[] GetAssemblyLevelAspects (Assembly assembly)
  }
}

