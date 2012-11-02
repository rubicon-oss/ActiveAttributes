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
using ActiveAttributes.Core.Infrastructure.Pointcuts;
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
    public void TypePointcut ()
    {
      var joinPoint = ObjectMother2.GetJoinPoint (typeof (DomainType));

      CheckMatching (joinPoint, new TypePointcut (joinPoint.Type));
      CheckNotMatching (joinPoint, new TypePointcut (typeof (int)));
      CheckMatching (joinPoint, new TypePointcut (typeof (DomainTypeBase)));
      CheckMatching (joinPoint, new TypePointcut (typeof (IDomainInterface)));
    }

    [Test]
    public void MemberNamePointcut ()
    {
      var method = ObjectMother2.GetMethodInfo ("Method");
      var joinPoint = new JoinPoint (method);

      CheckMatching (joinPoint, new MemberNamePointcut ("Method"));
      CheckNotMatching (joinPoint, new MemberNamePointcut ("Field"));
      CheckMatching (joinPoint, new MemberNamePointcut ("Meth*"));
    }

    [Test]
    public void Pointcut ()
    {
      var joinPoint = new JoinPoint (typeof (int));
      var pointcut = new PointcutExpression ("test");

      SetupPointcutParser (joinPoint, true, true);
      CheckMatching (joinPoint, pointcut);

      SetupPointcutParser (joinPoint, true, false);
      CheckNotMatching(joinPoint, pointcut);

      SetupPointcutParser (joinPoint, false, false);
      CheckNotMatching(joinPoint, pointcut);
    }

    [Test]
    public void TypeNamePointcut ()
    {
      var joinPoint = new JoinPoint (typeof (string));

      CheckMatching (joinPoint, new TypeNamePointcut ("String"));
      CheckNotMatching (joinPoint, new TypeNamePointcut ("int"));
      CheckMatching (joinPoint, new TypeNamePointcut ("Str*"));
    }

    [Test]
    public void ReturnTypePointcut ()
    {
      var method = ObjectMother2.GetMethodInfo (returnType: typeof (DomainType));
      var joinPoint = new JoinPoint (method);

      CheckMatching (joinPoint, new ReturnTypePointcut (typeof(DomainType)));
      CheckNotMatching (joinPoint, new ReturnTypePointcut (typeof(int)));
      CheckMatching (joinPoint, new ReturnTypePointcut (typeof(DomainTypeBase)));
      CheckMatching (joinPoint, new ReturnTypePointcut (typeof(IDomainInterface)));
    }

    private void SetupPointcutParser (JoinPoint joinPoint, bool first, bool second)
    {
      var pointcutMock1 = MockRepository.GenerateStrictMock<IPointcut> ();
      var pointcutMock2 = MockRepository.GenerateStrictMock<IPointcut> ();
      var pointcutMocks = new[] { pointcutMock1, pointcutMock2 };

      _pointcutParserMock.Expect (x => x.GetPointcuts ("test")).Return (pointcutMocks);
      pointcutMock1.Expect (x => x.MatchVisit (_pointcutVisitor, joinPoint)).Return (first);
      pointcutMock2.Expect (x => x.MatchVisit (_pointcutVisitor, joinPoint)).Return (second);
    }

    private void CheckMatching (JoinPoint joinPoint, IPointcut pointcut)
    {
      Assert.That (_pointcutVisitor.VisitPointcut (pointcut, joinPoint), Is.True);
    }

    private void CheckNotMatching (JoinPoint joinPoint, IPointcut pointcut)
    {
      Assert.That (_pointcutVisitor.VisitPointcut (pointcut, joinPoint), Is.False);
    }

    interface IDomainInterface {}

    class DomainTypeBase : IDomainInterface {}

    class DomainType : DomainTypeBase {}
  }
}