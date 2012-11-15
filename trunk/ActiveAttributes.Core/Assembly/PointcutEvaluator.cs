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
using ActiveAttributes.Advices;
using ActiveAttributes.Extensions;
using ActiveAttributes.Pointcuts;
using Remotion.ServiceLocation;
using System.Linq;
using Remotion.Utilities;

namespace ActiveAttributes.Assembly
{
  /// <summary>
  /// Serves as a evaluator of <see cref="Advice"/>s (and therefore <see cref="IPointcut"/>s) against a given <see cref="JoinPoint"/>.
  /// </summary>
  [ConcreteImplementation (typeof (PointcutEvaluator))]
  public interface IPointcutEvaluator
  {
    /// <summary>Evaluates if the advice matches a join-point. The evalutation will be dispatched first to the pointcut and then back to the evaluator.</summary>
    bool Matches (Advice advice, JoinPoint joinPoint);

    //bool VisitPointcut (IPointcut pointcut, JoinPoint joinPoint);

    bool MatchesExpression (IExpressionPointcut expressionPointcut, JoinPoint joinPoint);
    bool MatchesType (ITypePointcut pointcut, JoinPoint joinPoint);
    bool MatchesMemberName (IMemberNamePointcut pointcut, JoinPoint joinPoint);
    bool MatchesTypeName (ITypeNamePointcut pointcut, JoinPoint joinPoint);
    bool MatchesVisibility (IVisibilityPointcut pointcut, JoinPoint joinPoint);
    bool MatchesReturnType (IReturnTypePointcut pointcut, JoinPoint joinPoint);
    bool MatchesArgumentType (IArgumentTypePointcut pointcut, JoinPoint joinPoint);
    bool MatchesNamespace (INamespacePointcut pointcut, JoinPoint joinPoint);
    bool MatchesControlFlow (IControlFlowPointcut pointcut, JoinPoint joinPoint);
    bool MatchesMethod (IMethodPointcut pointcut, JoinPoint joinPoint);
    bool MatchesCustomAttribute (ICustomAttributePointcut pointcut, JoinPoint joinPoint);
  }

  public class PointcutEvaluator : IPointcutEvaluator
  {
    private readonly IPointcutParser _pointcutParser;

    public PointcutEvaluator (IPointcutParser pointcutParser)
    {
      ArgumentUtility.CheckNotNull ("pointcutParser", pointcutParser);

      _pointcutParser = pointcutParser;
    }

    public bool Matches (Advice advice, JoinPoint joinPoint)
    {
      return advice.Pointcuts.All (x => x.MatchVisit (this, joinPoint));
    }

    public bool MatchesExpression (IExpressionPointcut expressionPointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("expressionPointcut", expressionPointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      // TODO: special treatment for && and || ?
      var pointcuts = _pointcutParser.GetPointcuts (expressionPointcut.Expression);
      return pointcuts.All (x => x.MatchVisit (this, joinPoint));
    }

    public bool MatchesType (ITypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.Type.IsAssignableFrom (joinPoint.Type);
    }

    public bool MatchesMemberName (IMemberNamePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return joinPoint.Member.Name.IsMatchWildcard (pointcut.MemberName);
    }

    public bool MatchesTypeName (ITypeNamePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return joinPoint.Type.Name.IsMatchWildcard (pointcut.TypeName);
    }

    public bool MatchesVisibility (IVisibilityPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool MatchesReturnType (IReturnTypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.ReturnType.IsAssignableFrom (((MethodInfo) joinPoint.Member).ReturnType);
    }

    public bool MatchesArgumentType (IArgumentTypePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool MatchesNamespace (INamespacePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool MatchesControlFlow (IControlFlowPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotSupportedException();
    }

    public bool MatchesMethod (IMethodPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool MatchesCustomAttribute (ICustomAttributePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }
  }
}