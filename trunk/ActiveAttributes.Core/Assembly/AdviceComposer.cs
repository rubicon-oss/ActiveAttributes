﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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

using System.Collections.Generic;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Ordering;
using Remotion.Utilities;
using System.Linq;

namespace ActiveAttributes.Core.Assembly
{
  public interface IAdviceComposer
  {
    IEnumerable<Advice> Compose (IEnumerable<IAdviceBuilder> adviceBuilders, JoinPoint joinPoint);
  }

  public class AdviceComposer : IAdviceComposer
  {
    private readonly IAdviceSequencer _adviceSequencer;
    private readonly IPointcutEvaluator _pointcutEvaluator;

    public AdviceComposer (IAdviceSequencer adviceSequencer, IPointcutEvaluator pointcutEvaluator)
    {
      ArgumentUtility.CheckNotNull ("adviceSequencer", adviceSequencer);
      ArgumentUtility.CheckNotNull ("pointcutEvaluator", pointcutEvaluator);

      _adviceSequencer = adviceSequencer;
      _pointcutEvaluator = pointcutEvaluator;
    }

    public IEnumerable<Advice> Compose (IEnumerable<IAdviceBuilder> adviceBuilders, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("adviceBuilders", adviceBuilders);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      var advices = adviceBuilders.Select (x => x.Build());
      var matchingAdvices = advices.Where (x => _pointcutEvaluator.Matches (x, joinPoint));
      var sortedAdvices = _adviceSequencer.Sort (matchingAdvices);
      return sortedAdvices;
    }
  }
}