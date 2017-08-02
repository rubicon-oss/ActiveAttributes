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
using System.Reflection;
using ActiveAttributes.Aspects;
using ActiveAttributes.Discovery;
using ActiveAttributes.Model;
using ActiveAttributes.Model.Ordering;
using ActiveAttributes.Model.Pointcuts;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting.Reflection;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class AdviceBuilderTest
  {
    private AdviceBuilder _builder;

    private IPointcutBuilder _pointcutBuilderMock;
    private IOrderingBuilder _orderingBuilderMock;
    private IContextMappingBuilder _mappingBuilderMock;

    [SetUp]
    public void SetUp ()
    {
      _pointcutBuilderMock = MockRepository.GenerateStrictMock<IPointcutBuilder>();
      _orderingBuilderMock = MockRepository.GenerateStrictMock<IOrderingBuilder>();
      _mappingBuilderMock = MockRepository.GenerateStrictMock<IContextMappingBuilder>();
      _builder = new AdviceBuilder(_pointcutBuilderMock, _orderingBuilderMock, _mappingBuilderMock);
    }

    [Test]
    public void AddAdvice ()
    {
      var aspect = ObjectMother.GetAspect (typeof (DomainAspect));
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainAspect obj) => obj.Advice());
      var fakePointcut = ObjectMother.GetPointcut();
      var fakeOrdering = ObjectMother.GetOrdering();
      var fakeMappingPointcut = ObjectMother.GetPointcut();
      var fakeMappings = new Predicate<FieldInfo>[0].AsOneTime();
      var fakeTuple = Tuple.Create<IEnumerable<Predicate<FieldInfo>>, IPointcut> (fakeMappings, fakeMappingPointcut);
      _pointcutBuilderMock.Expect (x => x.Build (method)).Return (fakePointcut);
      _orderingBuilderMock.Expect (x => x.BuildOrderings (Arg<ICrosscutting>.Is.Anything, Arg.Is (method))).Return (new[] { fakeOrdering });
      _mappingBuilderMock.Expect (x => x.GetMappingsAndPointcut (method)).Return (fakeTuple);

      var result = _builder.GetAdvices (aspect).Single();

      Assert.That (result.Aspect, Is.SameAs (aspect));
      Assert.That (result.Method, Is.EqualTo (method));
      Assert.That (result.Execution, Is.EqualTo (AdviceExecution.Around));
      Assert.That (result.Name, Is.EqualTo (method.Name));
      Assert.That (result.Orderings, Is.EqualTo(new[] { fakeOrdering }));
      Assert.That (result.Pointcut, Is.TypeOf<AllPointcut>());
      var allPointcut = (AllPointcut) result.Pointcut;
      Assert.That (allPointcut.Pointcuts, Is.EqualTo (new[] { fakePointcut, fakeMappingPointcut }));
    }

    private class DomainAspect
    {
      [Advice (AdviceExecution.Around)]
      public void Advice () {}

      public void NonAdvice () {}
    }
  }
}