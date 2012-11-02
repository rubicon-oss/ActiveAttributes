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
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Configuration2.Configurators;
using Microsoft.Practices.ServiceLocation;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Castle.Components.DictionaryAdapter.Xml;
using System.Linq;

namespace ActiveAttributes.Core
{
  public class ObjectFactory : IObjectFactory
  {
    private static IObjectFactory _factory;

    public static T Create<T> ()
    {

      _factory = _factory ?? ServiceLocator.Current.GetInstance<IObjectFactory>();
      return _factory.Create<T> ();
    }

    private void Muh ()
    {

      var configurationProvider = ServiceLocator.Current.GetInstance<IActiveAttributesConfigurationProvider> ();
      var configuration = configurationProvider.GetConfiguration();

      var fieldIntroducer2 = new FieldIntroducer2();
      var aspectInitExpressionHelper = new AspectInitExpressionHelper();
      var constructorExpressionHelperFactory = new ConstructorExpressionsHelperFactory(aspectInitExpressionHelper);
      var constructorInitializationService = new ConstructorInitializationService (fieldIntroducer2, constructorExpressionHelperFactory);

      var invocationTypeProvider2 = new InvocationTypeProvider2();
      var invocationExpressionHelper = new InvocationExpressionHelper();
      var methodExpressionHelperFactory = new MethodExpressionHelperFactory(invocationExpressionHelper);
      var methodInterceptionService = new MethodInterceptionService (invocationTypeProvider2, methodExpressionHelperFactory);



    }

    T IObjectFactory.Create<T> ()
    {
      IActiveAttributesConfigurationProvider configurationProvider = ServiceLocator.Current.GetInstance<IActiveAttributesConfigurationProvider>();
      var configuration = configurationProvider.GetConfiguration();


      var constructorPatcher = new ConstructorPatcher();
      var factory = new Factory();
      var giveMeSomeName = new GiveItSomeName (factory, new ExpressionGeneratorFactory(), constructorPatcher);
      var fieldIntroducer = new FieldIntroducer();
      var methodAssembler = new MethodAssembler (
          configuration.AspectDescriptorProviders.OfType<IMethodLevelAspectDescriptorProvider>(),
          fieldIntroducer,
          giveMeSomeName,
          new AspectSorter (configuration),
          new MethodCopier(),
          constructorPatcher,
          factory);
      var typeAssembler_ = new TypeAssembler (
          configuration.AspectDescriptorProviders.OfType<ITypeLevelAspectDescriptorProvider>(), fieldIntroducer, giveMeSomeName, methodAssembler);

      var typeAssembler = new Remotion.TypePipe.TypeAssembly.TypeAssembler (
          new[] { typeAssembler_ }, CreateReflectionEmitTypeModifier (typeof (T).FullName));
      var assembledType = typeAssembler.AssembleType (typeof (T));

      return (T) Activator.CreateInstance (assembledType);
    }

    private ITypeModifier CreateReflectionEmitTypeModifier (string testName)
    {
      var assemblyName = new AssemblyName (testName);
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (
          assemblyName, AssemblyBuilderAccess.RunAndSave, Environment.CurrentDirectory);
      var generatedFileName = assemblyName.Name + ".dll";

      var moduleBuilder = assemblyBuilder.DefineDynamicModule (generatedFileName, true);
      var moduleBuilderAdapter = new ModuleBuilderAdapter (moduleBuilder);
      var decoratedModuleBuilderAdapter = new UniqueNamingModuleBuilderDecorator (moduleBuilderAdapter);
      var expressionPreparer = new ExpandingExpressionPreparer();
      var debugInfoGenerator = DebugInfoGenerator.CreatePdbGenerator();
      var handlerFactory = new SubclassProxyBuilderFactory (decoratedModuleBuilderAdapter, expressionPreparer, debugInfoGenerator);

      return new TypeModifier (handlerFactory);
    }
  }
}