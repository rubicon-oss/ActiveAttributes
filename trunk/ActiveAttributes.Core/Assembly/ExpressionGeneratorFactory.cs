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
using ActiveAttributes.Core.Assembly.Configuration;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class ExpressionGeneratorFactory : IExpressionGeneratorFactory
  {
    public IEnumerable<IExpressionGenerator> GetExpressionGenerators (
        IArrayAccessor instanceAccessor, IArrayAccessor staticAccessor, IEnumerable<IDescriptor> descriptors)
    {
      ArgumentUtility.CheckNotNull ("instanceAccessor", instanceAccessor);
      ArgumentUtility.CheckNotNull ("staticAccessor", staticAccessor);
      ArgumentUtility.CheckNotNull ("descriptors", descriptors);
      Assertion.IsTrue (!instanceAccessor.IsStatic);
      Assertion.IsTrue (staticAccessor.IsStatic);

      var aspectsAsCollection = descriptors.ConvertToCollection();
      var instanceGenerators = GetExpressionGenerators (instanceAccessor, aspectsAsCollection, Scope.Instance);
      var staticGenerators = GetExpressionGenerators (staticAccessor, aspectsAsCollection, Scope.Static);

      return instanceGenerators.Concat (staticGenerators);
    }

    private IEnumerable<IExpressionGenerator> GetExpressionGenerators (IArrayAccessor accessor, IEnumerable<IDescriptor> aspects, Scope scope)
    {
      return aspects.Where (x => x.Scope == scope).Select ((x, i) => new ExpressionGenerator (accessor, i, x)).Cast<IExpressionGenerator>();
    }
  }
}