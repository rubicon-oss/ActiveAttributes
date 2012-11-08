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
    bool Matches (Advice advice, JoinPoint joinPoint);

    //bool VisitPointcut (IPointcut pointcut, JoinPoint joinPoint);

    bool VisitExpression (IExpressionPointcut expressionPointcut, JoinPoint joinPoint);
    bool VisitType (ITypePointcut pointcut, JoinPoint joinPoint);
    bool VisitMemberName (IMemberNamePointcut pointcut, JoinPoint joinPoint);
    bool VisitTypeName (ITypeNamePointcut pointcut, JoinPoint joinPoint);
    bool VisitVisibility (IVisibilityPointcut pointcut, JoinPoint joinPoint);
    bool VisitReturnType (IReturnTypePointcut pointcut, JoinPoint joinPoint);
    bool VisitArgumentType (IArgumentTypePointcut pointcut, JoinPoint joinPoint);
    bool VisitNamespace (INamespacePointcut pointcut, JoinPoint joinPoint);
    bool VisitControlFlow (IControlFlowPointcut pointcut, JoinPoint joinPoint);
    bool VisitMethod (IMethodPointcut pointcut, JoinPoint joinPoint);
    bool VisitCustomAttribute (ICustomAttributePointcut pointcut, JoinPoint joinPoint);
  }

  public class PointcutVisitor : IPointcutVisitor
  {
    private readonly IPointcutParser _pointcutParser;

    public PointcutVisitor (IPointcutParser pointcutParser)
    {
      ArgumentUtility.CheckNotNull ("pointcutParser", pointcutParser);

      _pointcutParser = pointcutParser;
    }

    public bool Matches (Advice advice, JoinPoint joinPoint)
    {
      return advice.Pointcuts.All (x => x.MatchVisit (this, joinPoint));
    }

    //public bool VisitPointcut (IPointcut pointcut, JoinPoint joinPoint)
    //{
    //  ArgumentUtility.CheckNotNull ("pointcut", pointcut);
    //  ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

    //  if (pointcut is IExpressionPointcut && !VisitExpression ((IExpressionPointcut) pointcut, joinPoint))
    //    return false;
    //  if (pointcut is ITypePointcut && !VisitType ((ITypePointcut) pointcut, joinPoint))
    //    return false;
    //  if (pointcut is IMemberNamePointcut && !VisitMemberName ((IMemberNamePointcut) pointcut, joinPoint))
    //    return false;
    //  if (pointcut is ITypeNamePointcut && !VisitTypeName ((ITypeNamePointcut) pointcut, joinPoint))
    //    return false;
    //  if (pointcut is IReturnTypePointcut && !VisitReturnType ((IReturnTypePointcut) pointcut, joinPoint))
    //    return false;
    //  if (pointcut is IArgumentTypesPointcut && !VisitArgumentTypes ((IArgumentTypesPointcut) pointcut, joinPoint))
    //    return false;
    //  if (pointcut is INamespacePointcut && !VisitNamespace ((INamespacePointcut) pointcut, joinPoint))
    //    return false;
    //  if (pointcut is IControlFlowPointcut && !VisitControlFlow ((IControlFlowPointcut) pointcut, joinPoint))
    //    return false;

    //  return true;
    //}

    public bool VisitExpression (IExpressionPointcut expressionPointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("expressionPointcut", expressionPointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      // TODO: special treatment for && and ||
      var pointcuts = _pointcutParser.GetPointcuts (expressionPointcut.Expression);
      return pointcuts.All (x => x.MatchVisit (this, joinPoint));
    }

    public bool VisitType (ITypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.Type.IsAssignableFrom (joinPoint.Type);
    }

    public bool VisitMemberName (IMemberNamePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return joinPoint.Member.Name.IsMatchWildcard (pointcut.MemberName);
    }

    public bool VisitTypeName (ITypeNamePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return joinPoint.Type.Name.IsMatchWildcard (pointcut.TypeName);
    }

    public bool VisitVisibility (IVisibilityPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool VisitReturnType (IReturnTypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.ReturnType.IsAssignableFrom (((MethodInfo) joinPoint.Member).ReturnType);
    }

    public bool VisitArgumentType (IArgumentTypePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool VisitNamespace (INamespacePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool VisitControlFlow (IControlFlowPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotSupportedException();
    }

    public bool VisitMethod (IMethodPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool VisitCustomAttribute (ICustomAttributePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }
  }
}