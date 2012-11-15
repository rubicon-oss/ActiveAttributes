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
using ActiveAttributes.Discovery;
using ActiveAttributes.Discovery.DeclarationProviders;
using ActiveAttributes.Interception;
using ActiveAttributes.Ordering;
using ActiveAttributes.Ordering.Providers;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.MutableReflection;

namespace ActiveAttributes.Assembly
{
  /// <summary>
  ///   Serves as a factory for objects with extended functionality through <see cref="AspectAttribute" />s.
  /// </summary>
  [ConcreteImplementation (typeof (ObjectFactory))]
  public interface IObjectFactory
  {
    T Create<T> ();
  }

  public class ObjectFactory : IObjectFactory
  {
    public readonly static IParticipant Assembler;

    public static T Create<T> ()
    {
      IObjectFactory objectFactory = new ObjectFactory ();
      return objectFactory.Create<T> ();
    }

    static ObjectFactory()
    {
      var adviceBuilderFactory = new AdviceBuilderFactory ();
      var customAttributeProviderTransform = new CustomAttributeProviderTransform (adviceBuilderFactory);
      var classDeclarationProvider = new ClassDeclarationProvider (customAttributeProviderTransform);
      var aspectTypesProvider = new AspectTypesProvider();
      var aspectClassDeclarationProvider = new AspectClassDeclarationProvider (aspectTypesProvider, classDeclarationProvider);
      var customAttributeDataTransform = new CustomAttributeDataTransform();
      var attributeDeclarationProvider = new AttributeDeclarationProvider (classDeclarationProvider, customAttributeDataTransform);
      var methodAttributeDeclarationProvider = new MethodAttributeDeclarationProvider (attributeDeclarationProvider);
      var assemblyAttributeDeclarationProvider = new AssemblyAttributeDeclarationProvider (aspectTypesProvider, attributeDeclarationProvider);
      var assemblyLevelAdviceDeclarationProviders = new IAssemblyLevelDeclarationProvider[] { aspectClassDeclarationProvider, assemblyAttributeDeclarationProvider };
      var typeLevelAdviceDeclarationProviders = new ITypeLevelDeclarationProvider[0];
      var methodLevelAdviceDeclarationProviders = new[] { methodAttributeDeclarationProvider };

      var compositeDeclarationProvider = new CompositeDeclarationProvider (assemblyLevelAdviceDeclarationProviders, typeLevelAdviceDeclarationProviders, methodLevelAdviceDeclarationProviders);


      var interceptionTypeProvider = new InterceptionTypeProvider ();
      var interceptionExpressionHelperFactory = new InterceptionExpressionHelperFactory (interceptionTypeProvider);
      var fieldService = new StorageService ();
      var methodCopyService = new MethodCopyService();
      var aspectInitializationExpressionHelper = new InitializationExpressionHelper (methodCopyService);
      var aspectStorageService = new InitializationService (fieldService, aspectInitializationExpressionHelper);
      var interceptionWeaver = new InterceptionWeaver (interceptionExpressionHelperFactory, aspectStorageService);
      var adviceDependencyProvider = new AdviceDependencyProvider (new IAdviceOrderingProvider[0]);
      var adviceSequencer = new AdviceSequencer (adviceDependencyProvider);
      var pointcutParser = new PointcutParser ();
      var pointcutVisitor = new PointcutEvaluator (pointcutParser);
      var adviceComposer = new AdviceComposer (adviceSequencer, pointcutVisitor);
      Assembler = new Assembler (compositeDeclarationProvider, adviceComposer, interceptionWeaver);
    }
    T IObjectFactory.Create<T> ()
    {
      var typeAssembler = new TypeAssembler (new[] { Assembler }, CreateReflectionEmitTypeModifier (typeof (T).FullName));
      var assembledType = typeAssembler.AssembleType (typeof (T));

      return (T) Activator.CreateInstance (assembledType);
    }

    private ITypeModifier CreateReflectionEmitTypeModifier (string testName)
    {
      var assemblyName = new AssemblyName (testName);
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (assemblyName, AssemblyBuilderAccess.RunAndSave, Environment.CurrentDirectory);
      var generatedFileName = assemblyName.Name + ".dll";

      var moduleBuilder = assemblyBuilder.DefineDynamicModule (generatedFileName, true);
      var moduleBuilderAdapter = new ModuleBuilderAdapter (moduleBuilder);
      var decoratedModuleBuilderAdapter = new UniqueNamingModuleBuilderDecorator (moduleBuilderAdapter);
      var debugInfoGenerator = DebugInfoGenerator.CreatePdbGenerator ();
      var handlerFactory = new SubclassProxyBuilderFactory (decoratedModuleBuilderAdapter, debugInfoGenerator);

      return new TypeModifier (handlerFactory);
    }
  }
}