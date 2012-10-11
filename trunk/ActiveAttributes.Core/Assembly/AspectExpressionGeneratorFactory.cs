using System;
using System.Collections.Generic;
using System.Linq;
using ActiveAttributes.Core.Assembly.Configuration;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class AspectExpressionGeneratorFactory
  {
    public IEnumerable<IAspectGenerator> GetGenerators (
        IArrayAccessor instanceAccessor, IArrayAccessor staticAccessor, IEnumerable<IAspectDescriptor> aspects)
    {
      ArgumentUtility.CheckNotNull ("instanceAccessor", instanceAccessor);
      ArgumentUtility.CheckNotNull ("staticAccessor", staticAccessor);
      ArgumentUtility.CheckNotNull ("aspects", aspects);
      Assertion.IsTrue (!instanceAccessor.IsStatic);
      Assertion.IsTrue (staticAccessor.IsStatic);

      var aspectsAsCollection = aspects.ConvertToCollection();
      var instanceGenerators = GetGenerators (instanceAccessor, aspectsAsCollection, AspectScope.Instance);
      var staticGenerators = GetGenerators (staticAccessor, aspectsAsCollection, AspectScope.Static);

      return instanceGenerators.Concat (staticGenerators);
    }

    private IEnumerable<IAspectGenerator> GetGenerators (IArrayAccessor accessor, IEnumerable<IAspectDescriptor> aspects, AspectScope aspectScope)
    {
      return aspects.Where (x => x.Scope == aspectScope).Select ((x, i) => new AspectGenerator (accessor, i, x)).Cast<IAspectGenerator>();
    }
  }
}