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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Aspects;
using ActiveAttributes.Aspects.Ordering;
using ActiveAttributes.Model;
using ActiveAttributes.Model.Ordering;
using ActiveAttributes.Model.Pointcuts;
using ActiveAttributes.Weaving.Construction;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests
{
  public static partial class ObjectMother
  {
    public static Advice GetAdvice (MethodInfo method = null, AdviceExecution execution = (AdviceExecution) 0, Aspect aspect = null, ICrosscutting crosscutting = null)
    {
      method = method ?? GetMethodInfo (returnType: typeof (void), parameterTypes: Type.EmptyTypes);
      crosscutting = crosscutting ?? GetCrosscutting();
      aspect = aspect ?? GetAspect();

      return new Advice (aspect, method, execution, crosscutting);
    }

    public static ICrosscutting GetCrosscutting (
        IEnumerable<IOrdering> orderings = null,
        IPointcut pointcut = null,
        string name = "Aspect",
        string role = StandardRoles.Unspecified,
        int priority = 0)
    {
      orderings = orderings ?? new IOrdering[0];
      pointcut = pointcut ?? GetPointcut();

      var crosscutting = MockRepository.GenerateStub<ICrosscutting>();

      crosscutting.Stub (x => x.Orderings).Return (orderings);
      crosscutting.Stub (x => x.Pointcut).Return (pointcut);
      crosscutting.Stub (x => x.Name).Return (name);
      crosscutting.Stub (x => x.Role).Return (role);
      crosscutting.Stub (x => x.Priority).Return (priority);

      return crosscutting;
    }

    public static IOrdering GetOrdering()
    {
      var ordering = MockRepository.GenerateStub<IOrdering>();

      return ordering;
    }

    public static IPointcut GetPointcut ()
    {
      var pointcut = MockRepository.GenerateStub<IPointcut>();

      return pointcut;
    }

    public static Aspect GetAspect (
        Type type = null,
        AspectScope scope = (AspectScope) 0,
        AspectActivation activation = (AspectActivation) 0,
        IAspectConstruction construction = null,
        ICrosscutting crosscutting = null,
        IEnumerable<Advice> advices = null,
        IEnumerable<MemberImport> imports = null,
        IEnumerable<MemberIntroduction> introductions = null)
    {
      type = type ?? typeof (UnspecifiedAspect);
      scope = GetAspectScope (scope);
      activation = GetAspectActivation (activation);
      construction = construction ?? GetConstruction();
      crosscutting = crosscutting ?? GetCrosscutting();
      advices = advices ?? GetMultiple (() => GetAdvice());
      imports = imports ?? new MemberImport[0];
      introductions = introductions ?? new MemberIntroduction[0];

      return new Aspect (type, scope, activation, construction, crosscutting, advices, imports, introductions);
    }

    public static IAspectConstruction GetConstruction (
        ConstructorInfo constructor = null,
        ReadOnlyCollection<object> constructorArguments = null,
        ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> namedArguments = null)
    {
      constructor = constructor ?? GetConstructorInfo();
      constructorArguments = constructorArguments ?? new object[0].ToList().AsReadOnly();
      namedArguments = namedArguments ?? new ReadOnlyCollectionDecorator<ICustomAttributeNamedArgument> (new ICustomAttributeNamedArgument[0]);

      var construction = MockRepository.GenerateStub<IAspectConstruction>();

      construction.Stub (x => x.ConstructorInfo).Return (constructor);
      construction.Stub (x => x.ConstructorArguments).Return (constructorArguments);
      construction.Stub (x => x.NamedArguments).Return (namedArguments);

      return construction;
    }

    public static AspectScope GetAspectScope (AspectScope scope = (AspectScope) 0)
    {
      if (scope != 0)
        return scope;

      return AspectScope.Factory;
    }


    public static AspectActivation GetAspectActivation (AspectActivation activation = (AspectActivation) 0)
    {
      if (activation != 0)
        return activation;

      return AspectActivation.Auto;
    }

    public class UnspecifiedAspect {}
  }
}