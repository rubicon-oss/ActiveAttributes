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
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace ActiveAttributes.Assembly
{
  /// <summary>Serves as a evaluator of <see cref="Advice"/>s (and therefore <see cref="IPointcut"/>s) against a given <see cref="JoinPoint"/>.</summary>
  [ConcreteImplementation (typeof (PointcutEvaluator))]
  public interface IPointcutEvaluator
  {
    /// <summary>Evaluates if the advice matches a join-point. The evalutation will be dispatched first to the pointcut and then back to the evaluator.</summary>
    bool Matches (Advice advice, JoinPoint joinPoint);

    bool Visit (IPointcut pointcut, JoinPoint joinPoint);

    bool Visit (IExpressionPointcut expressionPointcut, JoinPoint joinPoint);
    bool Visit (ITypePointcut pointcut, JoinPoint joinPoint);
    bool Visit (IMemberNamePointcut pointcut, JoinPoint joinPoint);
    bool Visit (ITypeNamePointcut pointcut, JoinPoint joinPoint);
    bool Visit (IVisibilityPointcut pointcut, JoinPoint joinPoint);
    bool Visit (IReturnTypePointcut pointcut, JoinPoint joinPoint);
    bool Visit (IArgumentTypesPointcut pointcut, JoinPoint joinPoint);
    bool Visit (INamespacePointcut pointcut, JoinPoint joinPoint);
    bool Visit (IControlFlowPointcut pointcut, JoinPoint joinPoint);
    bool Visit (IMethodPointcut pointcut, JoinPoint joinPoint);
    bool Visit (ICustomAttributePointcut pointcut, JoinPoint joinPoint);
    bool Visit (IMemberTypePointcut pointcut, JoinPoint joinPoint);
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
      return advice.Pointcuts.All (x => x.Accept (this, joinPoint));
    }

    public bool Visit (IPointcut pointcut, JoinPoint joinPoint)
    {
      if (pointcut is IExpressionPointcut && !Visit ((IExpressionPointcut) pointcut, joinPoint))
        return false;



      if (pointcut is ITypePointcut && !Visit ((TypePointcut) pointcut, joinPoint))
        return false;

      if (pointcut is ITypeNamePointcut && !Visit ((ITypeNamePointcut) pointcut, joinPoint))
        return false;



      if (pointcut is IMemberNamePointcut && !Visit ((IMemberNamePointcut) pointcut, joinPoint))
        return false;

      if (pointcut is IVisibilityPointcut && !Visit ((IVisibilityPointcut) pointcut, joinPoint))
        return false;

      if (pointcut is IReturnTypePointcut && !Visit ((IReturnTypePointcut) pointcut, joinPoint))
        return false;

      if (pointcut is IArgumentTypesPointcut && !Visit ((IArgumentTypesPointcut) pointcut, joinPoint))
        return false;



      if (pointcut is IMethodPointcut && !Visit ((IMethodPointcut) pointcut, joinPoint))
        return false;


      if (pointcut is PropertySetPointcut || pointcut is PropertyGetPointcut)
      {
        var method = joinPoint.Member as MethodInfo;
        if (method == null)
          return false;

        if (method is MutableMethodInfo)
          method = ((MutableMethodInfo) method).UnderlyingSystemMethodInfo;

        var property = method.GetRelatedPropertyInfo();
        if (property == null)
          return false;

        if ((pointcut is PropertySetPointcut && method != property.GetSetMethod()) ||
            (pointcut is PropertyGetPointcut && method != property.GetGetMethod()))
          return false;
      }

      return true;
    }

    public bool Visit (IExpressionPointcut expressionPointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("expressionPointcut", expressionPointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      // TODO: special treatment for && and || ?
      var pointcuts = _pointcutParser.GetPointcuts (expressionPointcut.Expression);
      return pointcuts.All (x => x.Accept (this, joinPoint));
    }

    public bool Visit (ITypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.Type.IsAssignableFrom (joinPoint.Type);
    }

    public bool Visit (IMemberNamePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return joinPoint.Member.Name.IsMatchWildcard (pointcut.MemberName);
    }

    public bool Visit (ITypeNamePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return joinPoint.Type.Name.IsMatchWildcard (pointcut.TypeName);
    }

    public bool Visit (IVisibilityPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool Visit (IReturnTypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.ReturnType.IsAssignableFrom (((MethodInfo) joinPoint.Member).ReturnType);
    }

    public bool Visit (IArgumentTypesPointcut pointcut, JoinPoint joinPoint)
    {
      var method = (MethodInfo) joinPoint.Member;
      var zip = pointcut.ArgumentTypes.Zip (method.GetParameters().Select (x => x.ParameterType));
      return zip.All (x => x.Item1.IsAssignableFrom (x.Item2));
    }

    public bool Visit (INamespacePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool Visit (IControlFlowPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotSupportedException();
    }

    public bool Visit (IMethodPointcut pointcut, JoinPoint joinPoint)
    {
      return (bool) pointcut.Method.Invoke (null, new object[0]);
    }

    public bool Visit (ICustomAttributePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    public bool Visit (IMemberTypePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException();
    }

    private bool Visit (PropertySetPointcut pointcut, JoinPoint joinPoint)
    {
      var method = joinPoint.Member as MethodInfo;
      var property = method.GetRelatedPropertyInfo();
      return property != null && method == property.GetSetMethod();
    }

    private bool Visit (PropertyGetPointcut pointcut, JoinPoint joinPoint)
    {
      var method = joinPoint.Member as MethodInfo;
      var property = method.GetRelatedPropertyInfo ();
      return property != null && method == property.GetGetMethod ();
    }
  }
}