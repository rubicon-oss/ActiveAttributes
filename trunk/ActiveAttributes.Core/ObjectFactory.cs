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
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using TypeAssembler = Remotion.TypePipe.TypeAssembly.TypeAssembler;

namespace ActiveAttributes.Core
{
  public static class ObjectFactory
  {
    public static T Create<T> ()
    {
      var typeAssembler = new TypeAssembler (new[] { Assembly.TypeAssembler.Singleton }, CreateReflectionEmitTypeModifier ("AA.generated"));
      var assembledType = typeAssembler.AssembleType (typeof (T));

      return (T) Activator.CreateInstance (assembledType);
    }


    private static ITypeModifier CreateReflectionEmitTypeModifier (string testName)
    {
      var assemblyName = new AssemblyName (testName);
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (assemblyName, AssemblyBuilderAccess.RunAndSave, Environment.CurrentDirectory);
      var generatedFileName = assemblyName.Name + ".dll";

      var moduleBuilder = assemblyBuilder.DefineDynamicModule (generatedFileName, true);
      var moduleBuilderAdapter = new ModuleBuilderAdapter (moduleBuilder);
      var decoratedModuleBuilderAdapter = new UniqueNamingModuleBuilderDecorator (moduleBuilderAdapter);
      var expressionPreparer = new ExpandingExpressionPreparer ();
      var debugInfoGenerator = DebugInfoGenerator.CreatePdbGenerator ();
      var handlerFactory = new SubclassProxyBuilderFactory (decoratedModuleBuilderAdapter, expressionPreparer, debugInfoGenerator);

      return new TypeModifier (handlerFactory);
    }
  }
}