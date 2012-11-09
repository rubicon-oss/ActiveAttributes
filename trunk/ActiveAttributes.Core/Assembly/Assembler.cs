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
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Discovery.DeclarationProviders;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Core.Assembly
{
  public class Assembler : ITypeAssemblyParticipant
  {
    private readonly IDeclarationProvider _declarationProvider;
    private readonly IAdviceComposer _adviceComposer;
    private readonly IWeaver _weaver;

    private readonly IEnumerable<IAdviceBuilder> _globalAdvices;

    public Assembler (
        IDeclarationProvider declarationProvider, IAdviceComposer adviceComposer, IWeaver weaver)
    {
      ArgumentUtility.CheckNotNull ("declarationProvider", declarationProvider);
      ArgumentUtility.CheckNotNull ("adviceComposer", adviceComposer);
      ArgumentUtility.CheckNotNull ("weaver", weaver);

      _declarationProvider = declarationProvider;
      _adviceComposer = adviceComposer;
      _weaver = weaver;

      _globalAdvices = declarationProvider.GetDeclarations().ConvertToCollection();
    }

    public void ModifyType (MutableType mutableType)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);

      var typeAdvices = _declarationProvider.GetDeclarations (mutableType).ConvertToCollection();

      foreach (var method in mutableType.GetMethods())
      {
        var method1 = method;
        var methodAdvices = _declarationProvider.GetDeclarations (method1);
        var joinPoint = new JoinPoint (method);
        var allAdvices = ConcatAdvices (_globalAdvices, typeAdvices, methodAdvices);
        var sortedAdvices = _adviceComposer.Compose (allAdvices, joinPoint).ConvertToCollection();
        _weaver.Weave (method, sortedAdvices);
      }
    }

    private IEnumerable<IAdviceBuilder> ConcatAdvices (params IEnumerable<IAdviceBuilder>[] advices)
    {
      return advices.SelectMany (x => x);
    }
  }
}