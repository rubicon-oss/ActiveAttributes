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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Pointcuts;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class PointcutVisitorTest
  {
    private PointcutVisitor _pointcutVisitor;

    private IPointcutParser _pointcutParserMock;

    [SetUp]
    public void SetUp ()
    {
      _pointcutParserMock = MockRepository.GenerateStrictMock<IPointcutParser>();

      _pointcutVisitor = new PointcutVisitor (_pointcutParserMock);
    }

    [Test]
    public void Matches_ReturnsTrue ()
    {
      var joinPoint = ObjectMother2.GetJoinPoint();
      var pointcut = MockRepository.GenerateStrictMock<IPointcut>();
      pointcut.Expect (x => x.MatchVisit (_pointcutVisitor, joinPoint)).Return (true);
      var advice = ObjectMother2.GetAdvice (pointcuts: new[] { pointcut });

      var result = _pointcutVisitor.Matches (advice, joinPoint);

      pointcut.VerifyAllExpectations();
      Assert.That (result, Is.True);
    }

    [Test]
    public void Matches_ReturnsTrue_ForEmptyPointcuts ()
    {
      var joinPoint = ObjectMother2.GetJoinPoint();
      var advice = ObjectMother2.GetAdvice ();

      var result = _pointcutVisitor.Matches (advice, joinPoint);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Matches_ReturnsFalse ()
    {
      var joinPoint = ObjectMother2.GetJoinPoint();
      var pointcut1 = MockRepository.GenerateStrictMock<IPointcut>();
      var pointcut2 = MockRepository.GenerateStrictMock<IPointcut>();
      pointcut1.Expect (x => x.MatchVisit (_pointcutVisitor, joinPoint)).Return (true);
      pointcut2.Expect (x => x.MatchVisit (_pointcutVisitor, joinPoint)).Return (false);
      var advice = ObjectMother2.GetAdvice (pointcuts: new[] { pointcut1, pointcut2 });

      var result = _pointcutVisitor.Matches (advice, joinPoint);

      pointcut1.VerifyAllExpectations();
      pointcut2.VerifyAllExpectations();
      Assert.That (result, Is.False);
    }

    [Test]
    public void TypePointcut ()
    {
      var joinPoint = ObjectMother2.GetJoinPoint (typeof (DomainType));

      CheckMatching (_pointcutVisitor.VisitType, new TypePointcut (joinPoint.Type), joinPoint);
      CheckNotMatching (_pointcutVisitor.VisitType, new TypePointcut (typeof (int)), joinPoint);
      CheckMatching (_pointcutVisitor.VisitType, new TypePointcut (typeof (DomainTypeBase)), joinPoint);
      CheckMatching (_pointcutVisitor.VisitType, new TypePointcut (typeof (IDomainInterface)), joinPoint);
    }

    [Test]
    public void MemberNamePointcut ()
    {
      var method = ObjectMother2.GetMethodInfo ("Method");
      var joinPoint = new JoinPoint (method);

      CheckMatching (_pointcutVisitor.VisitMemberName, new MemberNamePointcut ("Method"), joinPoint);
      CheckNotMatching (_pointcutVisitor.VisitMemberName, new MemberNamePointcut ("Field"), joinPoint);
      CheckMatching (_pointcutVisitor.VisitMemberName, new MemberNamePointcut ("Meth*"), joinPoint);
    }

    [Test]
    public void ExpressionPointcut ()
    {
      var joinPoint = new JoinPoint (typeof (int));
      var pointcut = new ExpressionPointcut ("test");

      SetupPointcutParser (joinPoint, true, true);
      CheckMatching (_pointcutVisitor.VisitExpression, pointcut, joinPoint);

      SetupPointcutParser (joinPoint, true, false);
      CheckNotMatching (_pointcutVisitor.VisitExpression, pointcut, joinPoint);

      SetupPointcutParser (joinPoint, false, false);
      CheckNotMatching (_pointcutVisitor.VisitExpression, pointcut, joinPoint);
    }

    [Test]
    public void TypeNamePointcut ()
    {
      var joinPoint = new JoinPoint (typeof (string));

      CheckMatching (_pointcutVisitor.VisitTypeName, new TypeNamePointcut ("String"), joinPoint);
      CheckNotMatching (_pointcutVisitor.VisitTypeName, new TypeNamePointcut ("int"), joinPoint);
      CheckMatching (_pointcutVisitor.VisitTypeName, new TypeNamePointcut ("Str*"), joinPoint);
    }

    [Test]
    public void ReturnTypePointcut ()
    {
      var method = ObjectMother2.GetMethodInfo (returnType: typeof (DomainType));
      var joinPoint = new JoinPoint (method);

      CheckMatching (_pointcutVisitor.VisitReturnType, new ReturnTypePointcut (typeof (DomainType)), joinPoint);
      CheckNotMatching (_pointcutVisitor.VisitReturnType, new ReturnTypePointcut (typeof (int)), joinPoint);
      CheckMatching (_pointcutVisitor.VisitReturnType, new ReturnTypePointcut (typeof (DomainTypeBase)), joinPoint);
      CheckMatching (_pointcutVisitor.VisitReturnType, new ReturnTypePointcut (typeof (IDomainInterface)), joinPoint);
    }

    private void SetupPointcutParser (JoinPoint joinPoint, bool first, bool second)
    {
      var pointcutMock1 = MockRepository.GenerateStrictMock<IPointcut>();
      var pointcutMock2 = MockRepository.GenerateStrictMock<IPointcut>();
      var pointcutMocks = new[] { pointcutMock1, pointcutMock2 };

      _pointcutParserMock.Expect (x => x.GetPointcuts ("test")).Return (pointcutMocks);
      pointcutMock1.Expect (x => x.MatchVisit (_pointcutVisitor, joinPoint)).Return (first);
      pointcutMock2.Expect (x => x.MatchVisit (_pointcutVisitor, joinPoint)).Return (second);
    }

    private void CheckMatching<T> (Func<T, JoinPoint, bool> visit, T pointcut, JoinPoint joinPoint)
    {
      Assert.That (visit (pointcut, joinPoint), Is.True);
    }

    private void CheckNotMatching<T> (Func<T, JoinPoint, bool> visit, T pointcut, JoinPoint joinPoint)
    {
      Assert.That (visit (pointcut, joinPoint), Is.False);
    }

    private interface IDomainInterface {}

    private class DomainTypeBase : IDomainInterface {}

    private class DomainType : DomainTypeBase {}
  }
}