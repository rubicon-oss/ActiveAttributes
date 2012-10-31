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
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using Remotion.ServiceLocation;
using System.Linq;

namespace ActiveAttributes.Core.Infrastructure
{
  [ConcreteImplementation (typeof (PointcutVisitor))]
  public interface IPointcutVisitor
  {
    bool VisitPointcut (IPointcut pointcut, IJoinPoint joinPoint);

    bool VisitPointcut (ITextPointcut pointcut, IJoinPoint joinPoint);
    bool VisitPointcut (ITypePointcut pointcut, IJoinPoint joinPoint);
    bool VisitPointcut (IMemberNamePointcut pointcut, IJoinPoint joinPoint);
    bool VisitPointcut (IControlFlowPointcut pointcut, IJoinPoint joinPoint);
  }

  public class PointcutVisitor : IPointcutVisitor
  {
    private readonly IPointcutParser _pointcutParser;

    public PointcutVisitor (IPointcutParser pointcutParser)
    {
      _pointcutParser = pointcutParser;
    }

    public bool VisitPointcut (IPointcut pointcut, IJoinPoint joinPoint)
    {
      if (pointcut is ITextPointcut)
        VisitPointcut ((ITextPointcut) pointcut, joinPoint);
      else if (pointcut is ITypePointcut)
        VisitPointcut ((ITypePointcut) pointcut, joinPoint);
      else if (pointcut is IMemberNamePointcut)
        VisitPointcut ((IMemberNamePointcut) pointcut, joinPoint);
      else if (pointcut is IControlFlowPointcut)
        VisitPointcut ((IControlFlowPointcut) pointcut, joinPoint);

      throw new NotSupportedException();
    }

    public bool VisitPointcut (ITextPointcut pointcut, IJoinPoint joinPoint)
    {
      // TODO: special treatment for && and ||
      return _pointcutParser.GetPointcuts (pointcut.Text).All (x => x.MatchVisit (this, joinPoint));
    }

    public bool VisitPointcut (ITypePointcut pointcut, IJoinPoint joinPoint)
    {
      return pointcut.Type == joinPoint.Type;
    }

    public bool VisitPointcut (IMemberNamePointcut pointcut, IJoinPoint joinPoint)
    {
      return pointcut.MemberName == joinPoint.Member.Name;
    }

    public bool VisitPointcut (IControlFlowPointcut pointcut, IJoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }
  }
}