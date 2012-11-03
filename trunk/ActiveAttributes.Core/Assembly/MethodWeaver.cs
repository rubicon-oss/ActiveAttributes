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
using System.Reflection;
using ActiveAttributes.Core.Assembly.FieldWrapper;
using ActiveAttributes.Core.Assembly.Old;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.AttributedAspectDeclarationProviders;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Construction;
using ActiveAttributes.Core.Ordering;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using System.Linq;

namespace ActiveAttributes.Core.Assembly
{
  public interface IAdviceStorageProvider
  {
    IFieldWrapper GetStorageExpression (IAspectConstructionInfo constructionInfo, AdviceScope scope, MutableType mutableType);
  }

  public interface IGlobalInitializationService
  {
    IFieldWrapper AddAspectInitialization (IAspectConstructionInfo constructionInfo);
  } 

  internal class AdviceStorageProvider : IAdviceStorageProvider
  {
    private readonly IConstructorInitializationService _constructorInitializationService;
    private readonly IDictionary<Tuple<IAspectConstructionInfo, AdviceScope, MutableType>, IFieldWrapper> _initializedAspects; 

    public AdviceStorageProvider (IConstructorInitializationService constructorInitializationService)
    {
      _constructorInitializationService = constructorInitializationService;
    }

    public IFieldWrapper GetStorageExpression (IAspectConstructionInfo constructionInfo, AdviceScope scope, MutableType mutableType)
    {
      var tuple = Tuple.Create (constructionInfo, scope, mutableType);
      IFieldWrapper field = null;
      if (_initializedAspects.TryGetValue (tuple, out field))
        return field;

      field = _constructorInitializationService.AddAspectInitialization (mutableType, constructionInfo, scope);
      return field;
    }
  }


  public class Weaver : IActiveAttributesTypeWeaver
  {
    private readonly IEnumerable<IAspectDeclarationProvider> _aspectDeclarationProviders;
    private IDictionary<Advice, IFieldWrapper> _adviceStorage;

    public Weaver (IEnumerable<IAspectDeclarationProvider> aspectDeclarationProviders)
    {
      _aspectDeclarationProviders = aspectDeclarationProviders;
      _adviceStorage = new Dictionary<Advice, IFieldWrapper> ();

      var standalones = aspectDeclarationProviders.OfType<IStandaloneAspectDeclarationProvider> ().SelectMany (x => x.GetDeclarations ())
          .Concat (aspectDeclarationProviders.OfType<IAssemblyLevelAspectDeclarationProvider> ().SelectMany (x => x.GetDeclarations (null)));


    }

    public void ModifyType (MutableType mutableType)
    {
      //var aspdecls = new AspectDeclaration[0];
      //var joinPoint = new JoinPoint (mutableType);

      //var y = from declaration in aspdecls
      //        from advice in declaration.Advices
      //        where advice.Pointcuts.All (x => _pointcutVisitor.VisitPointcut (x, joinPoint))
      //        select new { ConstructionInfo = declaration.AspectConstructionInfo, Advices = advice };
      //_constructorInitializationService.
    }
  }

  public class MethodWeaver
  {
    private readonly IAdviceSequencer _adviceSequencer;
    private readonly IConstructorInitializationService _constructorInitializationService;
    private readonly IMethodInterceptionService _methodInterceptionService;
    private readonly IEnumerable<IMethodLevelAspectDeclarationProvider> _aspectDeclarationProviders;

    public MethodWeaver (
        IAdviceSequencer adviceSequencer,
        IConstructorInitializationService constructorInitializationService,
        IMethodInterceptionService methodInterceptionService,
        IEnumerable<IMethodLevelAspectDeclarationProvider> aspectDeclarationProviders)
    {
      _adviceSequencer = adviceSequencer;
      _constructorInitializationService = constructorInitializationService;
      _methodInterceptionService = methodInterceptionService;
      _aspectDeclarationProviders = aspectDeclarationProviders;
    }

    public void Weave (MutableMethodInfo method, IEnumerable<Advice> advices)
    {
      //_constructorInitializationService.
    }
  }
}