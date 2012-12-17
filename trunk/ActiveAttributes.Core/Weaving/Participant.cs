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
using ActiveAttributes.Model;
using ActiveAttributes.Weaving.Expressions;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.MutableReflection;
using Castle.Core.Internal;

namespace ActiveAttributes.Weaving
{
  public class Participant : IParticipant
  {
    private readonly IDeclarationProvider _declarationProvider;
    private readonly IAdviceComposer _adviceComposer;
    private readonly IAdviceWeaver _adviceWeaver;
    private readonly IIntertypeWeaver _intertypeWeaver;
    private readonly IEventMethodPreparer _eventMethodPreparer;

    public Participant (
        IDeclarationProvider declarationProvider,
        IAdviceComposer adviceComposer,
        IAdviceWeaver adviceWeaver,
        IIntertypeWeaver intertypeWeaver,
        IEventMethodPreparer eventMethodPreparer)
    {
      _declarationProvider = declarationProvider;
      _adviceComposer = adviceComposer;
      _adviceWeaver = adviceWeaver;
      _intertypeWeaver = intertypeWeaver;
      _eventMethodPreparer = eventMethodPreparer;
    }

    public void ModifyType (MutableType mutableType)
    {
      var typeAspects = _declarationProvider.GetDeclarations (mutableType).ConvertToCollection();

      typeAspects.ForEach (x => _intertypeWeaver.Import (x, new JoinPoint (mutableType)));

      foreach (var event_ in mutableType.UnderlyingSystemType.GetEvents().Where(x => x.GetAddMethod().IsVirtual))
        _eventMethodPreparer.Prepare (mutableType, event_);

      foreach (var method in mutableType.AllMutableMethods.ToArray())
      {
        // TODO: remove UnderlyingSystemMethodInfo
        var methodAspects = _declarationProvider.GetDeclarations (method.UnderlyingSystemMethodInfo).ConvertToCollection();
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