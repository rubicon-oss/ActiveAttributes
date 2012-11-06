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
using ActiveAttributes.Core.Discovery.AdviceDeclarationProviders;
using ActiveAttributes.Core.Infrastructure;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  public class Assembler : ITypeAssemblyParticipant
  {
    private readonly IAdviceDeclarationProvider _adviceDeclarationProvider;
    private readonly IAdviceComposer _adviceComposer;
    private readonly IWeaver _weaver;

    private readonly IEnumerable<IAdviceBuilder> _globalAdvices;

    public Assembler (
        IAdviceDeclarationProvider adviceDeclarationProvider, IAdviceComposer adviceComposer, IWeaver weaver)
    {
      ArgumentUtility.CheckNotNull ("adviceDeclarationProvider", adviceDeclarationProvider);
      ArgumentUtility.CheckNotNull ("adviceComposer", adviceComposer);
      ArgumentUtility.CheckNotNull ("weaver", weaver);

      _adviceDeclarationProvider = adviceDeclarationProvider;
      _adviceComposer = adviceComposer;
      _weaver = weaver;

      _globalAdvices = adviceDeclarationProvider.GetDeclarations().ToList();
    }

    public void ModifyType (MutableType mutableType)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);

      var typeAdvices = _adviceDeclarationProvider.GetDeclarations (mutableType).ToList();

      foreach (var method in mutableType.GetMethods())
      {
        var method1 = method;
        var methodAdvices = _adviceDeclarationProvider.GetDeclarations (method1);
        var joinPoint = new JoinPoint (method);
        var allAdvices = ConcatAdvices (_globalAdvices, typeAdvices, methodAdvices);
        var sortedAdvices = _adviceComposer.Compose (allAdvices, joinPoint).ToList ();
        _weaver.Weave (method, sortedAdvices);
      }
    }

    private IEnumerable<IAdviceBuilder> ConcatAdvices (params IEnumerable<IAdviceBuilder>[] advices)
    {
      return advices.SelectMany (x => x);
    }
  }
}