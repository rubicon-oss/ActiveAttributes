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
using ActiveAttributes.Core.Configuration;
using Remotion.FunctionalProgramming;
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
      var constructorPatcher = new ConstructorPatcher();
      var fieldIntroducer = new FieldIntroducer();
      var factory = new Factory();
      Singleton = new Assembler (aspectProvider, fieldIntroducer, constructorPatcher, methodCopier, factory);
    }

    public static Assembler Singleton { get; private set; }

    private readonly IFieldIntroducer _fieldIntroducer;
    private readonly IConstructorPatcher _constructorPatcher;
    private readonly IAspectsProvider _aspectProvider;
    private readonly IMethodCopier _methodCopier;
    private readonly IFactory _factory;

    internal Assembler (
        IAspectsProvider aspectProvider,
        IFieldIntroducer fieldIntroducer,
        IConstructorPatcher constructorPatcher,
        IMethodCopier methodCopier,
        IFactory factory)
    {
      _fieldIntroducer = fieldIntroducer;
      _constructorPatcher = constructorPatcher;
      _aspectProvider = aspectProvider;
      _methodCopier = methodCopier;
      _factory = factory;
    }

    public void ModifyType (MutableType mutableType)
    {
      s_log.InfoFormat ("Modifying type '{0}'", mutableType);

      var typeLevelAspectGenerators = HandleTypeLevelGenerators (mutableType).ToList();

      // TODO: Use GetMethods instead of AllMutableMethods. Use the MethodInfo (not MutableMethodInfo) to detect aspects on the methods. If there are aspects,
      // TODO: use GetMutableMethod to get the mutable method. If this throws an exception, wrap with a sensible ActiveAttributes configuration exception.
      // TODO: Then check MutableMethodInfo.CanSetBody, also throw a configuration exception if it is false.

      //foreach (var method in mutableType.GetMethods ())
      //{
      //  var methodLevelAspectGenerators = HandleMethodLevelAspects (mutableType, method);
      //}

      var mutableMethodInfos = mutableType.AllMutableMethods.ToList();
      foreach (var mutableMethod in mutableMethodInfos)
      {
        var methodLevelAspectGenerators = HandleMethodLevelAspects(mutableType, mutableMethod);

        // method level aspects + matching type level aspects
        var allMatchingAspectGenerators =
            typeLevelAspectGenerators.Where (x => x.Descriptor.Matches (mutableMethod.UnderlyingSystemMethodInfo))
                .Concat (methodLevelAspectGenerators)
                .ToList();

        if (allMatchingAspectGenerators.Any())
          AddAspectsToMethod (allMatchingAspectGenerators, mutableMethod);
      }
    }

    private void AddAspectsToMethod (List<IAspectGenerator> allMatchingAspectGenerators, MutableMethodInfo mutableMethod)
    {
      var mutableType = (MutableType) mutableMethod.DeclaringType;
      var copiedMethod = _methodCopier.GetCopy (mutableMethod);

      // reflection information
      var data = _fieldIntroducer.IntroduceMethodReflectionFields (mutableType, mutableMethod);
      var propertyInfoField = data.PropertyInfoField;
      var eventInfoField = data.EventInfoField;
      var methodInfoField = data.MethodInfoField;
      var delegateField = data.DelegateField;
      _constructorPatcher.AddReflectionAndDelegateInitialization (
          mutableMethod, propertyInfoField, eventInfoField, methodInfoField, delegateField, copiedMethod);

      // interception
      var typeProvider = _factory.GetTypeProvider (mutableMethod.UnderlyingSystemMethodInfo);
      var methodPatcher = _factory.GetMethodPatcher (
          mutableMethod,
          propertyInfoField,
          eventInfoField,
          methodInfoField,
          delegateField,
          allMatchingAspectGenerators,
          typeProvider);
      methodPatcher.AddMethodInterception();

      s_log.InfoFormat ("Intercepting method '{0}' with:", mutableMethod);
      foreach (var aspect in allMatchingAspectGenerators)
        s_log.InfoFormat (" - {0}", aspect.Descriptor);
    }

    private IEnumerable<IAspectGenerator> HandleTypeLevelGenerators (MutableType mutableType)
    {
      var aspectDescriptors = _aspectProvider.GetTypeLevelAspects (mutableType.UnderlyingSystemType).ToList();
      var fieldData = _fieldIntroducer.IntroduceTypeAspectFields (mutableType);

      return HandleAspects (mutableType, aspectDescriptors, fieldData);
    }

    private IEnumerable<IAspectGenerator> HandleMethodLevelAspects (MutableType mutableType, MutableMethodInfo mutableMethod)
    {
      var aspectDescriptors = GetMethodLevelAspectDescriptors (mutableMethod);
      var fieldData = _fieldIntroducer.IntroduceMethodAspectFields (mutableType, mutableMethod);

      return HandleAspects (mutableType, aspectDescriptors, fieldData);
    }

    private IEnumerable<IAspectGenerator> HandleAspects (
        MutableType mutableType, IEnumerable<IAspectDescriptor> aspectDescriptors, FieldIntroducer.Data fieldData)
    {
      var descriptorsAsCollection = aspectDescriptors.ConvertToCollection();

      var instanceArrayAccessors = _factory.GetInstanceAccessor (fieldData.InstanceAspectsField);
      var staticArrayAccessors = _factory.GetStaticAccessor (fieldData.StaticAspectsField);

      var instanceAspectGenerators = GetGenerators (instanceArrayAccessors, descriptorsAsCollection, AspectScope.Instance).ToList();
      var staticAspectGenerators = GetGenerators (staticArrayAccessors, descriptorsAsCollection, AspectScope.Static).ToList();

      _constructorPatcher.AddAspectInitialization (
          mutableType,
          staticArrayAccessors,
          instanceArrayAccessors,
          staticAspectGenerators,
          instanceAspectGenerators);

      return instanceAspectGenerators.Concat (staticAspectGenerators);
    }

    private IEnumerable<IAspectDescriptor> GetMethodLevelAspectDescriptors (MutableMethodInfo mutableMethod)
    {
      var methodLevelAspects = _aspectProvider.GetMethodLevelAspects (mutableMethod.UnderlyingSystemMethodInfo);
      var interfaceLevelAspects = _aspectProvider.GetInterfaceLevelAspects (mutableMethod.UnderlyingSystemMethodInfo);
      var propertyLevelAspects = _aspectProvider.GetPropertyLevelAspects (mutableMethod.UnderlyingSystemMethodInfo);
      var parameterLevelAspects = _aspectProvider.GetParameterLevelAspects (mutableMethod.UnderlyingSystemMethodInfo);
      var relatedMethodAspects = methodLevelAspects
          .Concat (interfaceLevelAspects)
          .Concat (propertyLevelAspects)
          .Concat (parameterLevelAspects)
          .ConvertToCollection();
      return relatedMethodAspects;
    }

    private IEnumerable<IAspectGenerator> GetGenerators (
        IArrayAccessor arrayAccessor, IEnumerable<IAspectDescriptor> descriptors, AspectScope scope)
    {
      return descriptors
          .Where (x => x.Scope == scope)
          .Select ((x, i) => _factory.GetGenerator (arrayAccessor, i, x))
          .ToList();
    }
  }
}