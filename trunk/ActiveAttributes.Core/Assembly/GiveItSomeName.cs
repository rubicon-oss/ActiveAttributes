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
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class GiveItSomeName : IGiveMeSomeName
  {
    private readonly IFactory _factory;
    private readonly IExpressionGeneratorFactory _expressionGeneratorFactory;
    private readonly IConstructorPatcher _constructorPatcher;

    public GiveItSomeName (
        IFactory factory,
        IExpressionGeneratorFactory expressionGeneratorFactory,
        IConstructorPatcher constructorPatcher)
    {
      ArgumentUtility.CheckNotNull ("factory", factory);
      ArgumentUtility.CheckNotNull ("expressionGeneratorFactory", expressionGeneratorFactory);
      ArgumentUtility.CheckNotNull ("constructorPatcher", constructorPatcher);

      _factory = factory;
      _expressionGeneratorFactory = expressionGeneratorFactory;
      _constructorPatcher = constructorPatcher;
    }

    public IEnumerable<IExpressionGenerator> IntroduceExpressionGenerators (
        MutableType mutableType, IEnumerable<IDescriptor> aspectDescriptors, FieldInfoContainer fieldInfoContainer)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      ArgumentUtility.CheckNotNull ("aspectDescriptors", aspectDescriptors);

      var instanceAspectsAccessor = _factory.GetAccessor (fieldInfoContainer.InstanceAspectsField);
      var staticAspectsAccessor = _factory.GetAccessor (fieldInfoContainer.StaticAspectsField);
      var aspectGenerators = _expressionGeneratorFactory.GetExpressionGenerators (instanceAspectsAccessor, staticAspectsAccessor, aspectDescriptors).ToArray();
      _constructorPatcher.AddAspectInitialization (mutableType, staticAspectsAccessor, instanceAspectsAccessor, aspectGenerators);
      return aspectGenerators;
    }
  }
}