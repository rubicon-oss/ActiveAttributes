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
using System.Reflection;
using ActiveAttributes.Core.Assembly.Configuration;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly
{
  public class MethodAssembler : IMethodAssembler
  {
    private readonly IEnumerable<IMethodLevelDescriptorProvider> _aspectProviders;
    private readonly IFieldIntroducer _fieldIntroducer;
    private readonly IGiveMeSomeName _giveMeSomeName;
    private readonly IScheduler _scheduler;
    private readonly IMethodCopier _methodCopier;
    private readonly IConstructorPatcher _constructorPatcher;
    private readonly IFactory _factory;

    public MethodAssembler (
        IConfiguration configuration,
        IFieldIntroducer fieldIntroducer,
        IGiveMeSomeName giveMeSomeName,
        IScheduler scheduler,
        IMethodCopier methodCopier,
        IConstructorPatcher constructorPatcher,
        IFactory factory)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("fieldIntroducer", fieldIntroducer);
      ArgumentUtility.CheckNotNull ("giveMeSomeName", giveMeSomeName);
      ArgumentUtility.CheckNotNull ("scheduler", scheduler);
      ArgumentUtility.CheckNotNull ("methodCopier", methodCopier);
      ArgumentUtility.CheckNotNull ("constructorPatcher", constructorPatcher);
      ArgumentUtility.CheckNotNull ("factory", factory);

      _aspectProviders = configuration.DescriptorProviders.OfType<IMethodLevelDescriptorProvider>().ToList();
      _fieldIntroducer = fieldIntroducer;
      _giveMeSomeName = giveMeSomeName;
      _scheduler = scheduler;
      _methodCopier = methodCopier;
      _constructorPatcher = constructorPatcher;
      _factory = factory;
    }

    public void ModifyMethod (MutableType mutableType, MethodInfo method, IEnumerable<IExpressionGenerator> typeGenerators)
    {
      var descriptors = _aspectProviders.SelectMany (x => x.GetDescriptors (method)).ToArray();
      var fields = _fieldIntroducer.IntroduceMethodFields (mutableType, method);
      var generators = _giveMeSomeName.IntroduceExpressionGenerators (mutableType, descriptors, fields).ToArray();

      var allGenerators = generators.Concat (typeGenerators.Where (x => x.Descriptor.Matches (method))).ConvertToCollection ();
      if (!allGenerators.Any ())
        return;

      var allAsTuples = allGenerators.Select (x => Tuple.Create (x.Descriptor, x));
      var sortedGenerators = _scheduler.GetOrdered (allAsTuples);

      var mutableMethod = mutableType.GetOrAddMutableMethod (method);
      var copiedMethod = _methodCopier.GetCopy (mutableMethod);

      _constructorPatcher.AddReflectionAndDelegateInitialization (mutableMethod, fields, copiedMethod);

      var typeProvider = _factory.GetTypeProvider (method);
      var methodPatcher = _factory.GetMethodPatcher (mutableMethod, fields, sortedGenerators, typeProvider);
      methodPatcher.AddMethodInterception();
    }
  }
}