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
using ActiveAttributes.Assembly;
using ActiveAttributes.Pointcuts;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.ServiceLocation;
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
      var advice = ObjectMother.GetAdvice (pointcuts: new[] { pointcut });
      pointcut.Expect (x => x.Accept (_pointcutEvaluator, joinPoint)).Return (true);

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
      var advice = ObjectMother.GetAdvice (pointcuts: new[] { pointcut1, pointcut2 });
      pointcut1.Expect (x => x.Accept (_pointcutEvaluator, joinPoint)).Return (true);
      pointcut2.Expect (x => x.Accept (_pointcutEvaluator, joinPoint)).Return (false);

      var result = _pointcutEvaluator.Matches (advice, joinPoint);

      pointcut1.VerifyAllExpectations();
      pointcut2.VerifyAllExpectations();
      Assert.That (result, Is.False);
    }

    [Test]
    public void TypePointcut ()
    {
      var joinPoint = ObjectMother.GetJoinPoint (typeof (DomainType));

      CheckMatches (new TypePointcut (joinPoint.Type), joinPoint);
      CheckMatchesNot (new TypePointcut (typeof (int)), joinPoint);
      CheckMatches (new TypePointcut (typeof (DomainTypeBase)), joinPoint);
      CheckMatches (new TypePointcut (typeof (IDomainInterface)), joinPoint);
    }

    [Test]
    public void MemberNamePointcut ()
    {
      var method = ObjectMother.GetMethodInfo ("Method");
      var joinPoint = new JoinPoint (method);

      CheckMatches (new MemberNamePointcut ("Method"), joinPoint);
      CheckMatchesNot (new MemberNamePointcut ("Field"), joinPoint);
      CheckMatches (new MemberNamePointcut ("Meth*"), joinPoint);
    }

    [Test]
    public void ExpressionPointcut ()
    {
      var joinPoint = new JoinPoint (typeof (int));
      var pointcut = new ExpressionPointcut ("test");

      SetupForExpressionPointcut (joinPoint, true, true);
      CheckMatches (pointcut, joinPoint);

      SetupForExpressionPointcut (joinPoint, true, false);
      CheckMatchesNot (pointcut, joinPoint);

      SetupForExpressionPointcut (joinPoint, false, false);
      CheckMatchesNot (pointcut, joinPoint);
    }

    [Test]
    public void TypeNamePointcut ()
    {
      var joinPoint = new JoinPoint (typeof (string));

      CheckMatches (new TypeNamePointcut ("String"), joinPoint);
      CheckMatchesNot (new TypeNamePointcut ("int"), joinPoint);
      CheckMatches (new TypeNamePointcut ("Str*"), joinPoint);
    }

    [Test]
    public void ReturnTypePointcut ()
    {
      var method = ObjectMother.GetMethodInfo (returnType: typeof (DomainType));
      var joinPoint = new JoinPoint (method);

      CheckMatches (new ReturnTypePointcut (typeof (DomainType)), joinPoint);
      CheckMatchesNot (new ReturnTypePointcut (typeof (int)), joinPoint);
      CheckMatches (new ReturnTypePointcut (typeof (DomainTypeBase)), joinPoint);
      CheckMatches (new ReturnTypePointcut (typeof (IDomainInterface)), joinPoint);
    }

    [Test]
    public void MethodPointcut ()
    {
      var trueMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => DomainAspect.TruePointcutMethod());
      var falseMethod = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => DomainAspect.FalsePointcutMethod());
      var joinPoint = ObjectMother.GetJoinPoint();

      CheckMatches (new MethodPointcut (trueMethod), joinPoint);
      CheckMatchesNot (new MethodPointcut (falseMethod), joinPoint);
    }

    [Test]
    public void PropertyPointcut ()
    {
      var setPointcut = new PropertySetPointcut();
      var getPointcut = new PropertyGetPointcut();
      var setJoinPoint = new JoinPoint (typeof (DomainType).GetMethod ("set_Property"));
      var getJoinPoint = new JoinPoint (typeof (DomainType).GetMethod ("get_Property"));
      var methodJoinPoint = new JoinPoint (ObjectMother.GetMethodInfo());
      var constructorJoinPoint = new JoinPoint (ObjectMother.GetConstructorInfo());

      CheckMatches (setPointcut, setJoinPoint);
      CheckMatches (getPointcut, getJoinPoint);
      CheckMatchesNot (setPointcut, getJoinPoint);
      CheckMatchesNot (getPointcut, setJoinPoint);
      CheckMatchesNot (setPointcut, methodJoinPoint);
      CheckMatchesNot (getPointcut, methodJoinPoint);
      CheckMatchesNot (setPointcut, constructorJoinPoint);
      CheckMatchesNot (getPointcut, constructorJoinPoint);
    }

    [Test]
    public void ArgumentTypesPointcut ()
    {
      var method = ObjectMother.GetMethodInfo (parameterTypes: new[] { typeof (int) });
      var joinPoint = new JoinPoint (method);

      CheckMatches (new ArgumentTypesPointcut (new[] { typeof (int) }), joinPoint);
      CheckMatchesNot (new ArgumentTypesPointcut (new[] { typeof (string) }), joinPoint);
      CheckMatches (new ArgumentTypesPointcut (new[] { typeof (object) }), joinPoint);
    }

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IPointcutEvaluator> ();

      Assert.That (instance, Is.TypeOf<PointcutEvaluator> ());
    }

    private void SetupForExpressionPointcut (JoinPoint joinPoint, bool first, bool second)
    {
      var pointcutMock1 = MockRepository.GenerateStrictMock<IPointcut>();
      var pointcutMock2 = MockRepository.GenerateStrictMock<IPointcut>();
      var pointcutMocks = new[] { pointcutMock1, pointcutMock2 };

      _pointcutParserMock.Expect (x => x.GetPointcuts ("test")).Return (pointcutMocks);
      pointcutMock1.Expect (x => x.Accept (_pointcutEvaluator, joinPoint)).Return (first);
      pointcutMock2.Expect (x => x.Accept (_pointcutEvaluator, joinPoint)).Return (second);
    }

    private void CheckMatches<T> (T pointcut, JoinPoint joinPoint) where T : IPointcut
    {
      var result = _pointcutEvaluator.Visit (pointcut, joinPoint);
      Assert.That (result, Is.True);
    }

    private void CheckMatchesNot<T> (T pointcut, JoinPoint joinPoint) where T : IPointcut
    {
      var result = _pointcutEvaluator.Visit (pointcut, joinPoint);
      Assert.That (result, Is.False);
    }

    private interface IDomainInterface {}

    private class DomainTypeBase : IDomainInterface {}

    private class DomainType : DomainTypeBase
    {
      public string Property { get; set; }
    }

    public static class DomainAspect
    {
      public static bool TruePointcutMethod () { return true; }

      public static bool FalsePointcutMethod () { return false; } 
    }
  }
}