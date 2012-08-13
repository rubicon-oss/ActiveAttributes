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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;
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

    static Assembler ()
    {
      var methodCopier = new MethodCopier();
      var aspectProvider = new AspectsProvider();
      var methodPatcherFactory = new MethodPatcherFactory();
      var constructorPatcher = new ConstructorPatcher();
      var fieldIntroducer = new FieldIntroducer();
      Singleton = new Assembler (aspectProvider, fieldIntroducer, constructorPatcher, methodPatcherFactory, methodCopier);
    }

    public static Assembler Singleton { get; private set; }

    private readonly FieldIntroducer _fieldIntroducer;
    private readonly ConstructorPatcher _constructorPatcher;
    private readonly MethodPatcherFactory _methodPatcherFactory;
    private readonly IAspectsProvider _aspectProvider;
    private readonly MethodCopier _methodCopier;

    private Assembler (
        IAspectsProvider aspectProvider,
        FieldIntroducer fieldIntroducer,
        ConstructorPatcher constructorPatcher,
        MethodPatcherFactory methodPatcherFactory,
        MethodCopier methodCopier)
    {
      _fieldIntroducer = fieldIntroducer;
      _constructorPatcher = constructorPatcher;
      _methodPatcherFactory = methodPatcherFactory;
      _aspectProvider = aspectProvider;
      _methodCopier = methodCopier;
    }

    private static IEnumerable<IAspectGenerator> GetGenerators (IArrayAccessor arrayAccessor, IEnumerable<IAspectDescriptor> descriptors, AspectScope scope)
    {
      return descriptors
          .Where (x => x.Scope == scope)
          .Select ((x, i) => new AspectGenerator (arrayAccessor, i, x))
          .Cast<IAspectGenerator>()
          .ToList();
    }

    public void ModifyType (MutableType mutableType)
    {
      s_log.InfoFormat ("Modifying type '{0}'", mutableType);

      // TYPE LEVEL ASPECTS

      // get aspects and field data
      var typeLevelAspectDescriptors = _aspectProvider.GetTypeLevelAspects (mutableType).ToList();
      var typeFieldData = _fieldIntroducer.IntroduceTypeLevelFields (mutableType);

      var instanceTypeLevelArrayAccessor = new InstanceArrayAccessor (typeFieldData.InstanceAspectsField);
      var staticTypeLevelArrayAccessor = new StaticArrayAccessor (typeFieldData.StaticAspectsField);

      // create generators
      var instanceTypeLevelAspectGenerators = GetGenerators (instanceTypeLevelArrayAccessor, typeLevelAspectDescriptors, AspectScope.Instance).ToList();
      var staticTypeLevelAspectGenerators = GetGenerators (staticTypeLevelArrayAccessor, typeLevelAspectDescriptors, AspectScope.Static).ToList();

      // add initialization
      _constructorPatcher.AddAspectInitialization (
          mutableType, staticTypeLevelArrayAccessor, instanceTypeLevelArrayAccessor, staticTypeLevelAspectGenerators, instanceTypeLevelAspectGenerators);

      // TODO: Use GetMethods instead of AllMutableMethods. Use the MethodInfo (not MutableMethodInfo) to detect aspects on the methods. If there are aspects,
      // TODO: use GetMutableMethod to get the mutable method. If this throws an exception, wrap with a sensible ActiveAttributes configuration exception.
      // TODO: Then check MutableMethodInfo.CanSetBody, also throw a configuration exception if it is false.
      var mutableMethodInfos = mutableType.AllMutableMethods.ToList ();
      foreach (var mutableMethod in mutableMethodInfos)
      {
        // get aspects
        // ... by method
        var methodLevelAspectDescriptors = _aspectProvider.GetMethodLevelAspects (mutableMethod).ToList();
        // ... by interface
        var interfaceLevelAspects = _aspectProvider.GetInterfaceLevelAspects (mutableMethod);
        methodLevelAspectDescriptors.AddRange (interfaceLevelAspects);
        // ... by property
        var propertyLevelAspects = _aspectProvider.GetPropertyLevelAspects (mutableMethod);
        methodLevelAspectDescriptors.AddRange (propertyLevelAspects);
        // ... by event

        // get field data
        var methodLevelFieldData = _fieldIntroducer.IntroduceMethodLevelFields (mutableMethod);

        var propertyInfoField = methodLevelFieldData.PropertyInfoField;
        var eventInfoField = methodLevelFieldData.EventInfoField;
        var methodInfoField = methodLevelFieldData.MethodInfoField;
        var delegateField = methodLevelFieldData.DelegateField;

        var instanceMethodLevelArrayAccessor = new InstanceArrayAccessor (methodLevelFieldData.InstanceAspectsField);
        var staticMethodLevelArrayAccessor = new StaticArrayAccessor (methodLevelFieldData.StaticAspectsField);

        // create generators
        var instanceMethodLevelAspectGenerators = GetGenerators (instanceMethodLevelArrayAccessor, methodLevelAspectDescriptors, AspectScope.Instance).ToList ();
        var staticMethodLevelAspectGenerators = GetGenerators (staticMethodLevelArrayAccessor, methodLevelAspectDescriptors, AspectScope.Static).ToList ();

        // get all matching
        var allMatchingAspectGenerators = Enumerable.Empty<IAspectGenerator>()
            .Concat (instanceTypeLevelAspectGenerators)
            .Concat (staticTypeLevelAspectGenerators)
            .Where (x => x.Descriptor.Matches (mutableMethod.UnderlyingSystemMethodInfo))
            .Concat (instanceMethodLevelAspectGenerators)
            .Concat (staticMethodLevelAspectGenerators)
            .ToList();

        // if any
        if (allMatchingAspectGenerators.Any())
        {
          var copiedMethod = _methodCopier.GetCopy (mutableMethod);

          // add initialization
          _constructorPatcher.AddReflectionAndDelegateInitialization (
              mutableMethod, propertyInfoField, eventInfoField, methodInfoField, delegateField, copiedMethod);
          _constructorPatcher.AddAspectInitialization (
              mutableType,
              staticMethodLevelArrayAccessor,
              instanceMethodLevelArrayAccessor,
              staticMethodLevelAspectGenerators,
              instanceMethodLevelAspectGenerators);

          // add interception
          var typeProvider = new TypeProvider (mutableMethod.UnderlyingSystemMethodInfo);
          var methodPatcher = _methodPatcherFactory.GetMethodPatcher (mutableMethod, propertyInfoField, eventInfoField, methodInfoField, delegateField, allMatchingAspectGenerators, typeProvider);
          methodPatcher.AddMethodInterception ();

          s_log.InfoFormat ("Intercepting method '{0}' with:", mutableMethod);
          foreach (var aspect in allMatchingAspectGenerators)
            s_log.InfoFormat (" - {0}", aspect.Descriptor);
        }
        else
        {
          s_log.DebugFormat ("Skipping unmarked method '{0}'.", mutableMethod);
        }
      }
    }
  }
}