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
using ActiveAttributes.Assembly;
using ActiveAttributes.Pointcuts;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class PointcutEvaluatorTest
  {
    private PointcutEvaluator _pointcutEvaluator;

    private IPointcutParser _pointcutParserMock;

    [SetUp]
    public void SetUp ()
    {
      _pointcutParserMock = MockRepository.GenerateStrictMock<IPointcutParser>();

      _pointcutEvaluator = new PointcutEvaluator (_pointcutParserMock);
    }

    [Test]
    public void Matches_ReturnsTrue ()
    {
      var joinPoint = ObjectMother.GetJoinPoint();
      var pointcut = MockRepository.GenerateStrictMock<IPointcut>();
      pointcut.Expect (x => x.MatchVisit (_pointcutEvaluator, joinPoint)).Return (true);
      var advice = ObjectMother.GetAdvice (pointcuts: new[] { pointcut });

      var result = _pointcutEvaluator.Matches (advice, joinPoint);

      pointcut.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Matches_ReturnsTrue_ForEmptyPointcuts ()
    {
      var joinPoint = ObjectMother.GetJoinPoint();
      var advice = ObjectMother.GetAdvice ();

      var result = _pointcutEvaluator.Matches (advice, joinPoint);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Matches_ReturnsFalse ()
    {
      var joinPoint = ObjectMother.GetJoinPoint();
      var pointcut1 = MockRepository.GenerateStrictMock<IPointcut>();
      var pointcut2 = MockRepository.GenerateStrictMock<IPointcut>();
      pointcut1.Expect (x => x.MatchVisit (_pointcutEvaluator, joinPoint)).Return (true);
      pointcut2.Expect (x => x.MatchVisit (_pointcutEvaluator, joinPoint)).Return (false);
      var advice = ObjectMother.GetAdvice (pointcuts: new[] { pointcut1, pointcut2 });

      var result = _pointcutEvaluator.Matches (advice, joinPoint);

      pointcut1.VerifyAllExpectations();
      pointcut2.VerifyAllExpectations();
      Assert.That (result, Is.False);
    }

    [Test]
    public void TypePointcut ()
    {
      var joinPoint = ObjectMother.GetJoinPoint (typeof (DomainType));

      CheckMatches (_pointcutEvaluator.MatchesType, new TypePointcut (joinPoint.Type), joinPoint);
      CheckMatchesNot (_pointcutEvaluator.MatchesType, new TypePointcut (typeof (int)), joinPoint);
      CheckMatches (_pointcutEvaluator.MatchesType, new TypePointcut (typeof (DomainTypeBase)), joinPoint);
      CheckMatches (_pointcutEvaluator.MatchesType, new TypePointcut (typeof (IDomainInterface)), joinPoint);
    }

    [Test]
    public void MemberNamePointcut ()
    {
      var method = ObjectMother.GetMethodInfo ("Method");
      var joinPoint = new JoinPoint (method);

      CheckMatches (_pointcutEvaluator.MatchesMemberName, new MemberNamePointcut ("Method"), joinPoint);
      CheckMatchesNot (_pointcutEvaluator.MatchesMemberName, new MemberNamePointcut ("Field"), joinPoint);
      CheckMatches (_pointcutEvaluator.MatchesMemberName, new MemberNamePointcut ("Meth*"), joinPoint);
    }

    [Test]
    public void ExpressionPointcut ()
    {
      var joinPoint = new JoinPoint (typeof (int));
      var pointcut = new ExpressionPointcut ("test");

      SetupPointcutParser (joinPoint, true, true);
      CheckMatches (_pointcutEvaluator.MatchesExpression, pointcut, joinPoint);

      SetupPointcutParser (joinPoint, true, false);
      CheckMatchesNot (_pointcutEvaluator.MatchesExpression, pointcut, joinPoint);

      SetupPointcutParser (joinPoint, false, false);
      CheckMatchesNot (_pointcutEvaluator.MatchesExpression, pointcut, joinPoint);
    }

    [Test]
    public void TypeNamePointcut ()
    {
      var joinPoint = new JoinPoint (typeof (string));

      CheckMatches (_pointcutEvaluator.MatchesTypeName, new TypeNamePointcut ("String"), joinPoint);
      CheckMatchesNot (_pointcutEvaluator.MatchesTypeName, new TypeNamePointcut ("int"), joinPoint);
      CheckMatches (_pointcutEvaluator.MatchesTypeName, new TypeNamePointcut ("Str*"), joinPoint);
    }

    [Test]
    public void ReturnTypePointcut ()
    {
      var method = ObjectMother.GetMethodInfo (returnType: typeof (DomainType));
      var joinPoint = new JoinPoint (method);

      CheckMatches (_pointcutEvaluator.MatchesReturnType, new ReturnTypePointcut (typeof (DomainType)), joinPoint);
      CheckMatchesNot (_pointcutEvaluator.MatchesReturnType, new ReturnTypePointcut (typeof (int)), joinPoint);
      CheckMatches (_pointcutEvaluator.MatchesReturnType, new ReturnTypePointcut (typeof (DomainTypeBase)), joinPoint);
      CheckMatches (_pointcutEvaluator.MatchesReturnType, new ReturnTypePointcut (typeof (IDomainInterface)), joinPoint);
    }

    private void SetupPointcutParser (JoinPoint joinPoint, bool first, bool second)
    {
      var pointcutMock1 = MockRepository.GenerateStrictMock<IPointcut>();
      var pointcutMock2 = MockRepository.GenerateStrictMock<IPointcut>();
      var pointcutMocks = new[] { pointcutMock1, pointcutMock2 };

      _pointcutParserMock.Expect (x => x.GetPointcuts ("test")).Return (pointcutMocks);
      pointcutMock1.Expect (x => x.MatchVisit (_pointcutEvaluator, joinPoint)).Return (first);
      pointcutMock2.Expect (x => x.MatchVisit (_pointcutEvaluator, joinPoint)).Return (second);
    }

    private void CheckMatches<T> (Func<T, JoinPoint, bool> visit, T pointcut, JoinPoint joinPoint)
    {
      Assert.That (visit (pointcut, joinPoint), Is.True);
    }

    private void CheckMatchesNot<T> (Func<T, JoinPoint, bool> visit, T pointcut, JoinPoint joinPoint)
    {
      Assert.That (visit (pointcut, joinPoint), Is.False);
    }

    private interface IDomainInterface {}

    private class DomainTypeBase : IDomainInterface {}

    private class DomainType : DomainTypeBase {}
  }
}