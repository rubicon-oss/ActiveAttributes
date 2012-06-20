using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using ActiveAttributes.Core.Assembly;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.TypeAssembly;

namespace ActiveAttributes.Core
{
  public static class ObjectFactory
  {
    public static T Create<T> ()
    {
      var typeAssembler = new TypeAssembler (new[] { new Assembler() }, CreateReflectionEmitTypeModifier ("AA.generated.dll"));
      var assembledType = typeAssembler.AssembleType (typeof (T));

      return (T) Activator.CreateInstance (assembledType);
    }

    private static ITypeModifier CreateReflectionEmitTypeModifier (string testName)
    {
      var assemblyName = new AssemblyName (testName);
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (
          assemblyName, AssemblyBuilderAccess.RunAndSave, Environment.CurrentDirectory);
      var generatedFileName = assemblyName.Name + ".dll";

      var moduleBuilder = assemblyBuilder.DefineDynamicModule (generatedFileName, true);
      var moduleBuilderAdapter = new ModuleBuilderAdapter (moduleBuilder);
      var guidBasedSubclassProxyNameProvider = new GuidBasedSubclassProxyNameProvider();
      var expressionPreparer = new ExpandingExpressionPreparer();
      var debugInfoGenerator = DebugInfoGenerator.CreatePdbGenerator();
      var handlerFactory = new SubclassProxyBuilderFactory (
          moduleBuilderAdapter, guidBasedSubclassProxyNameProvider, expressionPreparer, debugInfoGenerator);

      return new TypeModifier (handlerFactory);
    }
  }
}