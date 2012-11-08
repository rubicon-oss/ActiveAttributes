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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using ActiveAttributes.Core.Ordering;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AdviceComposerTest
  {
    [Test]
    public void Compose ()
    {
      var adviceBuilder1Mock = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var adviceBuilder2Mock = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var adviceBuilders = new[] { adviceBuilder1Mock, adviceBuilder2Mock };

      var advice1 = ObjectMother2.GetAdvice();
      var advice2 = ObjectMother2.GetAdvice();

      adviceBuilder1Mock.Expect (x => x.Build()).Return (advice1);
      adviceBuilder2Mock.Expect (x => x.Build()).Return (advice2);

      var joinPoint = ObjectMother2.GetJoinPoint();

      var pointcutVisitorMock = MockRepository.GenerateStrictMock<IPointcutVisitor>();
      var adviceSequencerMock = MockRepository.GenerateStrictMock<IAdviceSequencer>();

      var fakeAdvices = new Advice[0];

      pointcutVisitorMock.Expect (x => x.Matches (advice1, joinPoint)).Return (false);
      pointcutVisitorMock.Expect (x => x.Matches (advice2, joinPoint)).Return (true);
      adviceSequencerMock.Expect (x => x.Sort (Arg<IEnumerable<Advice>>.List.Equal (new[] { advice2 }))).Return (fakeAdvices);

      var composer = new AdviceComposer (adviceSequencerMock, pointcutVisitorMock);
      var result = composer.Compose (adviceBuilders.AsOneTime(), joinPoint);

      adviceBuilder1Mock.VerifyAllExpectations();
      adviceBuilder2Mock.VerifyAllExpectations();
      pointcutVisitorMock.VerifyAllExpectations ();
      adviceSequencerMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeAdvices));
    }
  }
}