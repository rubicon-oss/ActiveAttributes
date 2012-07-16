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
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;
using Remotion.Globalization;
using Remotion.Logging;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Responsible for the assembly of types applied with aspects.
  /// </summary>
  public class Assembler : ITypeAssemblyParticipant
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (Assembler));

    private readonly FieldIntroducer _fieldIntroducer;
    private readonly ConstructorPatcher _constructorPatcher;
    private readonly MethodPatcher _methodPatcher;
    private readonly AspectsProvider _aspectProvider;
    private readonly MethodCopier _methodCopier;
    private readonly AspectGeneratorFactory _generatorFactory;

    public Assembler ()
    {
      LogManager.InitializeConsole (LogLevel.Debug);

      _generatorFactory = new AspectGeneratorFactory ();
      _methodCopier = new MethodCopier ();
      _aspectProvider = new AspectsProvider ();
      _methodPatcher = new MethodPatcher ();
      _constructorPatcher = new ConstructorPatcher ();
      _fieldIntroducer = new FieldIntroducer ();
    }

    public void ModifyType (MutableType mutableType)
    {
      s_log.InfoFormat ("Modifying type '{0}'", mutableType);

      var typeLevelAspectDescriptors = _aspectProvider.GetTypeLevelAspects (mutableType.UnderlyingSystemType).ToList();
      var typeFieldData = _fieldIntroducer.IntroduceTypeLevelFields (mutableType);

      var instanceTypeLevelField = typeFieldData.InstanceAspectsField;
      var staticTypeLevelField = typeFieldData.StaticAspectsField;

      var instanceTypeLevelAspectGenerators = _generatorFactory.GetAspectGenerators (
          typeLevelAspectDescriptors, AspectScope.Instance, instanceTypeLevelField).ToList();
      var staticTypeLevelAspectGenerators = _generatorFactory.GetAspectGenerators (
          typeLevelAspectDescriptors, AspectScope.Static, staticTypeLevelField).ToList();
      var allTypeLevelAspectGenerators = instanceTypeLevelAspectGenerators.Concat (staticTypeLevelAspectGenerators).ToList();

      _constructorPatcher.AddAspectInitialization (
          mutableType, staticTypeLevelField, instanceTypeLevelField, staticTypeLevelAspectGenerators, instanceTypeLevelAspectGenerators);

      // TODO: Use GetMethods instead of AllMutableMethods. Use the MethodInfo (not MutableMethodInfo) to detect aspects on the methods. If there are aspects,
      // TODO: use GetMutableMethod to get the mutable method. If this throws an exception, wrap with a sensible ActiveAttributes configuration exception.
      // TODO: Then check MutableMethodInfo.CanSetBody, also throw a configuration exception if it is false.
      foreach (var mutableMethod in mutableType.AllMutableMethods.ToList ())
      {
        var methodLevelAspectDescriptors = _aspectProvider.GetMethodLevelAspects (mutableMethod.UnderlyingSystemMethodInfo).ToList();

        if (mutableMethod.UnderlyingSystemMethodInfo.IsCompilerGenerated ())
        {
          var propertyName = mutableMethod.Name.Substring (4);
          var propertyInfo = mutableType.UnderlyingSystemType.GetProperty (propertyName, BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
          if (propertyInfo != null)
          {
            var propertyLevelAspects = _aspectProvider.GetPropertyLevelAspects (propertyInfo);
            methodLevelAspectDescriptors.AddRange (propertyLevelAspects);
          }
        }

        var methodLevelFieldData = _fieldIntroducer.IntroduceMethodLevelFields (mutableMethod);

        var methodInfoField = methodLevelFieldData.MethodInfoField;
        var delegateField = methodLevelFieldData.DelegateField;
        var instanceMethodLevelField = methodLevelFieldData.InstanceAspectsField;
        var staticMethodLevelField = methodLevelFieldData.StaticAspectsField;

        var instanceMethodLevelAspectGenerators = _generatorFactory.GetAspectGenerators (
            methodLevelAspectDescriptors, AspectScope.Instance, instanceMethodLevelField).ToList();
        var staticMethodLevelAspectGenerators = _generatorFactory.GetAspectGenerators (
            methodLevelAspectDescriptors, AspectScope.Static, staticMethodLevelField).ToList();
        var allMethodLevelAspectGenerators = instanceMethodLevelAspectGenerators.Concat (staticMethodLevelAspectGenerators).ToList();

        var allMatchingAspectGenerators = allTypeLevelAspectGenerators
            .Where (x => x.Descriptor.Matches (mutableMethod))
            .Concat (allMethodLevelAspectGenerators)
            .ToList();

        if (allMatchingAspectGenerators.Any())
        {
          var copiedMethod = _methodCopier.GetCopy (mutableMethod);

          _constructorPatcher.AddMethodInfoAndDelegateInitialization (mutableMethod, methodInfoField, delegateField, copiedMethod);
          _constructorPatcher.AddAspectInitialization (
              mutableType, staticMethodLevelField, instanceMethodLevelField, staticMethodLevelAspectGenerators, instanceMethodLevelAspectGenerators);

          _methodPatcher.AddMethodInterception (mutableMethod, methodInfoField, delegateField, allMatchingAspectGenerators);

          s_log.InfoFormat ("Mutated method '{0}' to be intercepted with:", mutableMethod);
          foreach (var aspect in allMatchingAspectGenerators)
            s_log.InfoFormat (" - {0}", aspect.Descriptor);
        }
        else
        {
          s_log.InfoFormat ("Skipped method '{0}' that was not marked with any aspects.", mutableMethod);
        }
      }
    }
  }
}