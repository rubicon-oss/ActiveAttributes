using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests
{
  [SetUpFixture]
  public class SetupFixture
  {
    private static string s_generatedFileDirectory;

    private HashSet<string> _copiedFileNames;

    public static string GeneratedFileDirectory
    {
      get
      {
        Assertion.IsNotNull (s_generatedFileDirectory, "GeneratedFileDirectory can only be called after SetUp has run.");
        return s_generatedFileDirectory;
      }
    }

    [SetUp]
    public void SetUp ()
    {
      s_generatedFileDirectory = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "GeneratedAssemblies");
      _copiedFileNames = new HashSet<string>();

      PrepareOutputDirectory();
    }

    [TearDown]
    public void TearDown ()
    {
      CleanupOutputDirectory();
    }

    private void PrepareOutputDirectory ()
    {
      Directory.CreateDirectory (s_generatedFileDirectory);

      CopyModuleToOutputDirectory (GetType().Assembly.ManifestModule);
      CopyModuleToOutputDirectory (typeof (MutableType).Assembly.ManifestModule);
      CopyModuleToOutputDirectory (typeof (MethodInterceptionAspectAttribute).Assembly.ManifestModule);
    }

    private void CopyModuleToOutputDirectory (Module copiedModule)
    {
      var sourcePath = copiedModule.FullyQualifiedName;
      var destPath = Path.Combine (s_generatedFileDirectory, copiedModule.Name);
      File.Copy (sourcePath, destPath, true);
      _copiedFileNames.Add (copiedModule.Name);
    }

    private void CleanupOutputDirectory ()
    {
      // Only delete directory if no generated files left
      var fileNamesInGeeratedDirectory = Directory.GetFiles (s_generatedFileDirectory).Select (Path.GetFileName);
      if (fileNamesInGeeratedDirectory.Any (f => !_copiedFileNames.Contains (f)))
        return;

      Directory.Delete (s_generatedFileDirectory, true);
    }
  }
}