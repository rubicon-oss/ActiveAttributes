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
using System.Collections.Generic;
using System.Linq;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Assembly.Providers;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Logging;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  /// <summary>
  /// Implementation of an <see cref="ITypeAssemblyParticipant"/> that modifies types according to
  /// applied <see cref="AspectAttribute"/>.
  /// </summary>
  public class Assembler : ITypeAssemblyParticipant
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (Assembler));

    static Assembler ()
    {
      var methodCopier = new MethodCopier();
      var aspectProviders = new IAspectProvider[]
                            {
                                new TypeLevelAspectProvider(),
                                new MethodLevelAspectProvider(),
                                new InterfaceMethodAspectProvider(),
                                new MethodParameterAspectProvider(),
                                new PropertyAspectProvider()
                            };
      var constructorPatcher = new ConstructorPatcher();
      var fieldIntroducer = new FieldIntroducer();
      var factory = new Factory();
      var scheduler = new AspectScheduler (AspectConfiguration.Singleton);
      Singleton = new Assembler (aspectProviders, fieldIntroducer, constructorPatcher, methodCopier, factory, scheduler);
    }

    public static Assembler Singleton { get; private set; }

    private readonly AspectExpressionGeneratorFactory _generatorFactory = new AspectExpressionGeneratorFactory();
    private readonly IFieldIntroducer _fieldIntroducer;
    private readonly IConstructorPatcher _constructorPatcher;
    private readonly IAspectProvider[] _aspectProviders;
    private readonly IMethodCopier _methodCopier;
    private readonly IFactory _factory;
    private readonly IAspectScheduler _scheduler;

    // TODO: Maybe add an AspectProviderContainer with .MethodLevelAspectProviders and .TypeLevelAspectProviders property (and more in the future), then you don't need IAspectProvider any more
    internal Assembler (
        IAspectProvider[] aspectProviders,
        IFieldIntroducer fieldIntroducer,
        IConstructorPatcher constructorPatcher,
        IMethodCopier methodCopier,
        IFactory factory,
        IAspectScheduler scheduler)
    {
      _fieldIntroducer = fieldIntroducer;
      _constructorPatcher = constructorPatcher;
      _aspectProviders = aspectProviders;
      _methodCopier = methodCopier;
      _factory = factory;
      _scheduler = scheduler;
    }

    public void ModifyType (MutableType mutableType)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);

      // var typeLevelDescriptors = ...;
      // Dictionary<IAttributeDescriptor, IAttributeGenerator> typeLevelGenerators = CreateTypeLevelGenerators (typeLevelDescriptors);
      var typeLevelAspectGenerators = HandleTypeLevelGenerators (mutableType).ToList();

      //foreach (var method in mutableType.GetMethods ())
      //{
      //  var method_ = method;

      //  var matchingTypeLevelAspects = typeLevelAspectGenerators.Where (x => x.Descriptor.Matches (method_));
      //  var methodLevelAspects = _aspectProviders.OfType<IMethodLevelAspectProvider> ().SelectMany (x => x.GetAspects (method_));

      //  if (!matchingTypeLevelAspects.Any () && !methodLevelAspects.Any ())
      //    continue;

      //  var mutableMethod = mutableType.GetOrAddMutableMethod (method);
      //  if (!mutableMethod.CanSetBody)
      //    throw new Exception ("TODO");
      //}


      var mutableMethodInfos = mutableType.AllMutableMethods.ToList ();
      foreach (var mutableMethod in mutableMethodInfos)
      {
        // var methodLevelDescriptors = ...;
        // var allDescriptors = typeLevelDescriptors.Where (d => d.Matches (...)).Concat (methodLevelDescriptors);
        // var sortedDescriptors = _scheduler.Sort (allDescriptors);
        // Dictionary<IAttributeDescriptor, IAttributeGenerator> methodLevelGenerators = CreateMethodLevelGenerators (methodLevelDescriptors);
        // var generators = sortedDescriptors.Select (d => typeLevelGenerators.GetValueOrDefault (d) ?? methodLevelGenerators.GetValueOrDefault (d));
        var methodLevelAspectGenerators = HandleMethodLevelAspects (mutableMethod);

        // matching type level aspects + method level aspects
        var method = mutableMethod;
        var allMatchingAspectGenerators = typeLevelAspectGenerators
            .Where (x => x.Descriptor.Matches (method.UnderlyingSystemMethodInfo))
            .Concat (methodLevelAspectGenerators)
            .ToList ();

        if (allMatchingAspectGenerators.Any ())
        {
          var tuples = allMatchingAspectGenerators.Select (x => Tuple.Create (x.Descriptor, x));
          var sortedAspects = _scheduler.GetOrdered (tuples).ToList ();
          AddAspectsToMethod (sortedAspects, mutableMethod);
        }
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
      var aspectDescriptors = _aspectProviders.OfType<ITypeLevelAspectProvider>().SelectMany (x => x.GetAspects (mutableType.UnderlyingSystemType));
      var fieldData = _fieldIntroducer.IntroduceTypeAspectFields (mutableType);

      return HandleAspects (mutableType, aspectDescriptors, fieldData);
    }

    private IEnumerable<IAspectGenerator> HandleMethodLevelAspects (MutableMethodInfo mutableMethod)
    {
      var mutableType = (MutableType) mutableMethod.DeclaringType;
      var aspectDescriptors = _aspectProviders.OfType<IMethodLevelAspectProvider>().SelectMany (x => x.GetAspects (mutableMethod.UnderlyingSystemMethodInfo));
      var fieldData = _fieldIntroducer.IntroduceMethodAspectFields (mutableType, mutableMethod);

      return HandleAspects (mutableType, aspectDescriptors, fieldData);
    }

    private IEnumerable<IAspectGenerator> HandleAspects (
        MutableType mutableType, IEnumerable<IAspectDescriptor> aspectDescriptors, FieldIntroducer.Data fieldData)
    {
      var instanceArrayAccessor = _factory.GetAccessor (fieldData.InstanceAspectsField);
      var staticArrayAccessor = _factory.GetAccessor (fieldData.StaticAspectsField);
      var aspectGenerators = _generatorFactory.GetGenerators (instanceArrayAccessor, staticArrayAccessor, aspectDescriptors).ConvertToCollection();
      _constructorPatcher.AddAspectInitialization (mutableType, staticArrayAccessor, instanceArrayAccessor, aspectGenerators);

      return aspectGenerators;
    }
  }
}