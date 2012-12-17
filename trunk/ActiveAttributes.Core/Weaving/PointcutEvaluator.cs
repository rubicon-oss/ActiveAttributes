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
using System.Linq;
using ActiveAttributes.Model;
using ActiveAttributes.Model.Pointcuts;
using ActiveAttributes.Weaving.Expressions;
using Microsoft.Scripting.Ast;
using Remotion.Utilities;
using ActiveAttributes.Extensions;

namespace ActiveAttributes.Weaving
{
  public class PointcutEvaluator
  {
    public virtual bool Visit (IPointcut pointcut, JoinPoint joinPoint)
    {
      return pointcut.Accept (this, joinPoint);
    }

    public virtual bool VisitAll (AllPointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.Pointcuts.All (x => x.Accept (this, joinPoint));
    }

    public virtual bool VisitAny (AnyPointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.Pointcuts.Any (x => x.Accept (this, joinPoint));
    }

    public virtual bool VisitNot (NotPointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return !pointcut.Pointcut.Accept (this, joinPoint);
    }

    public bool VisitMemberName (MemberNamePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return joinPoint.Method == null || joinPoint.Method.Name.IsMatchWildcard (pointcut.MethodName);
    }

    public bool VisitType (TypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return pointcut.Type.IsAssignableFrom (joinPoint.DeclaringType);
    }

    public bool VisitReturnType (ReturnTypePointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      var method = joinPoint.Method as MethodInfo;
      return method != null && pointcut.ReturnType.IsAssignableFrom (method.ReturnType);
    }

    public bool VisitMethod (MethodPointcut pointcut, JoinPoint joinPoint)
    {
      ArgumentUtility.CheckNotNull ("pointcut", pointcut);
      ArgumentUtility.CheckNotNull ("joinPoint", joinPoint);

      return (bool) pointcut.Method.Invoke (null, new object[0]);
    }

    public bool VisitArgumentIndex (ArgumentIndexPointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException ();
    }

    public bool VisitArgumentName (ArgumentNamePointcut pointcut, JoinPoint joinPoint)
    {
      throw new NotImplementedException ();
    }

    public bool VisitArgument (ArgumentPointcut pointcut, JoinPoint joinPoint)
    {
      return joinPoint.Method.GetParameters().Any (x => pointcut.ArgumentType.IsAssignableFrom (x.ParameterType));
    }

    public bool VisitMethodExecution (MethodExecutionPointcut pointcut, JoinPoint joinPoint)
    {
      return joinPoint.Expression is MethodExecutionExpression;
    }

    public bool VisitMethodCall (MethodCallPointcut pointcut, JoinPoint joinPoint)
    {
      return joinPoint.Expression is MethodCallExpression;
    }
  }
}