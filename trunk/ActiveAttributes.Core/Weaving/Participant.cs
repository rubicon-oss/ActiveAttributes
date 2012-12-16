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
using System.Linq;
using ActiveAttributes.Discovery;
using ActiveAttributes.Infrastructure;
using ActiveAttributes.Weaving.Expressions;
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Castle.Core.Internal;

namespace ActiveAttributes.Weaving
{
  [ConcreteImplementation (typeof (IntertypeWeaver))]
  public interface IIntertypeWeaver
  {
    void Import (Aspect aspect, JoinPoint joinPoint);
  }

  public class IntertypeWeaver : IIntertypeWeaver
  {
    private readonly IAspectStorageCache _aspectStorageCache;

    public IntertypeWeaver (IAspectStorageCache aspectStorageCache)
    {
      _aspectStorageCache = aspectStorageCache;
    }

    public void Import (Aspect aspect, JoinPoint joinPoint)
    {
      var mutableType = joinPoint.DeclaringType;
      var storage = _aspectStorageCache.GetOrAddStorage (aspect, joinPoint);
      foreach (var import in aspect.MemberImports)
      {
        var member = mutableType.GetMethod (import.Name);
        if (member == null)
        {
          if (import.IsRequired)
            throw new Exception (string.Format ("Member '{0}' not found.", import.Name));
          continue;
        }


        var import1 = import;
        var field = Expression.Field (storage.CreateStorageExpression (joinPoint.This), import.Field);
        mutableType.AddInstanceInitialization (
            ctx =>
            Expression.Assign (field, new NewDelegateExpression (import1.Type, joinPoint.This, member)));
      }
      //aspect.MemberImports.Single(x => x.)
    }
  }

  public class Participant : IParticipant
  {
    private readonly IDeclarationProvider _declarationProvider;
    private readonly IAdviceComposer _adviceComposer;
    private readonly IAdviceWeaver _adviceWeaver;
    private readonly IIntertypeWeaver _intertypeWeaver;

    public Participant (IDeclarationProvider declarationProvider, IAdviceComposer adviceComposer, IAdviceWeaver adviceWeaver, IIntertypeWeaver intertypeWeaver)
    {
      _declarationProvider = declarationProvider;
      _adviceComposer = adviceComposer;
      _adviceWeaver = adviceWeaver;
      _intertypeWeaver = intertypeWeaver;
    }

    public void ModifyType (MutableType mutableType)
    {
      var typeAspects = _declarationProvider.GetDeclarations (mutableType).ConvertToCollection();

      typeAspects.ForEach (x => _intertypeWeaver.Import (x, new JoinPoint (mutableType)));


      foreach (var method in mutableType.AllMutableMethods)
      {
        var methodAspects = _declarationProvider.GetDeclarations (method).ConvertToCollection();
        var allAspects = typeAspects.Concat (methodAspects).ConvertToCollection();

        var joinPoint = new JoinPoint (method, new MethodExecutionExpression(method, method.Body));
        var allAdvices = _adviceComposer.Compose (allAspects, joinPoint).ToList();

        if (allAdvices.Any())
          _adviceWeaver.Weave (joinPoint, allAdvices);
      }
    }

    public ICacheKeyProvider PartialCacheKeyProvider
    {
      get { return null; }
    }
  }
}