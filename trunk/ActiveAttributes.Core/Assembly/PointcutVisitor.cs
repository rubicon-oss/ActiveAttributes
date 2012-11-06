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
using System.Reflection;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using Remotion.ServiceLocation;
using System.Linq;
using ActiveAttributes.Core.Extensions;
using Remotion.Utilities;

namespace ActiveAttributes.Core.Assembly
{
  [ConcreteImplementation (typeof (PointcutVisitor))]
  public interface IPointcutVisitor
  {
    bool VisitPointcut (IPointcut pointcut, JoinPoint joinPoint);

    bool VisitPointcut (ITextPointcut pointcut, JoinPoint joinPoint);
    bool VisitPointcut (ITypePointcut pointcut, JoinPoint joinPoint);
    bool VisitPointcut (IMemberNamePointcut pointcut, JoinPoint joinPoint);
    bool VisitPointcut (ITypeNamePointcut pointcut, JoinPoint joinPoint);
    bool VisitPointcut (IVisibilityPointcut pointcut, JoinPoint joinPoint);
    bool VisitPointcut (IReturnTypePointcut pointcut, JoinPoint joinPoint);
    bool VisitPointcut (IArgumentsPointcut pointcut, JoinPoint joinPoint);
    bool VisitPointcut (INamespacePointcut pointcut, JoinPoint joinPoint);
    bool VisitPointcut (IControlFlowPointcut pointcut, JoinPoint joinPoint);
    bool VisitPointcut (IMethodPointcut pointcut, JoinPoint joinPoint);
  }

  public class PointcutVisitor : IPointcutVisitor
  {
    private readonly IPointcutParser _pointcutParser;

    public PointcutVisitor (IPointcutParser pointcutParser)
    {
      ArgumentUtility.CheckNotNull ("pointcutParser", pointcutParser);

      _pointcutParser = pointcutParser;
    }

    public bool VisitPointcut (IPointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      if (pointcut is ITextPointcut && !VisitPointcut ((ITextPointcut) pointcut, joinPoint))
        return false;
      if (pointcut is ITypePointcut && !VisitPointcut ((ITypePointcut) pointcut, joinPoint))
        return false;
      if (pointcut is IMemberNamePointcut && !VisitPointcut ((IMemberNamePointcut) pointcut, joinPoint))
        return false;
      if (pointcut is ITypeNamePointcut && !VisitPointcut ((ITypeNamePointcut) pointcut, joinPoint))
        return false;
      if (pointcut is IReturnTypePointcut && !VisitPointcut ((IReturnTypePointcut) pointcut, joinPoint))
        return false;
      if (pointcut is IArgumentsPointcut && !VisitPointcut ((IArgumentsPointcut) pointcut, joinPoint))
        return false;
      if (pointcut is INamespacePointcut && !VisitPointcut ((INamespacePointcut) pointcut, joinPoint))
        return false;
      if (pointcut is IControlFlowPointcut && !VisitPointcut ((IControlFlowPointcut) pointcut, joinPoint))
        return false;

      return true;
    }

    public bool VisitPointcut (ITextPointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      // TODO: special treatment for && and ||
      var pointcuts = _pointcutParser.GetPointcuts (pointcut.Text);
      return pointcuts.All (x => x.MatchVisit (this, joinPoint));
    }

    public bool VisitPointcut (ITypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.Type.IsAssignableFrom (joinPoint.Type);
    }

    public bool VisitPointcut (IMemberNamePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return joinPoint.Member.Name.IsMatchWildcard (pointcut.MemberName);
    }

    public bool VisitPointcut (ITypeNamePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return joinPoint.Type.Name.IsMatchWildcard (pointcut.TypeName);
    }

    public bool VisitPointcut (IVisibilityPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool VisitPointcut (IReturnTypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.ReturnType.IsAssignableFrom (((MethodInfo) joinPoint.Member).ReturnType);
    }

    public bool VisitPointcut (IArgumentsPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool VisitPointcut (INamespacePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool VisitPointcut (IControlFlowPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotSupportedException();
    }

    public bool VisitPointcut (IMethodPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }
  }
}