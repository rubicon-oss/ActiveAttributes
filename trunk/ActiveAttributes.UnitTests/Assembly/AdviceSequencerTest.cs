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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Ordering;
using ActiveAttributes.Core.Utilities;
using NUnit.Framework;
using Remotion.Collections;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AdviceSequencerTest
  {
    private AdviceSequencer _adviceSequencer;

    private IAdviceDependencyMerger _dependencyMergerMock;
    private IAdviceDependencyProvider _dependencyProviderMock1;
    private IAdviceDependencyProvider _dependencyProviderMock2;

    private Advice _advice1;
    private Advice _advice2;
    private Advice _advice3;

    [SetUp]
    public void SetUp ()
    {
      _advice1 = ObjectMother2.GetAdvice();
      _advice2 = ObjectMother2.GetAdvice();
      _advice3 = ObjectMother2.GetAdvice();

      _dependencyMergerMock = MockRepository.GenerateStrictMock<IAdviceDependencyMerger>();
      _dependencyProviderMock1 = MockRepository.GenerateStrictMock<IAdviceDependencyProvider>();
      _dependencyProviderMock2 = MockRepository.GenerateStrictMock<IAdviceDependencyProvider>();

      _adviceSequencer = new AdviceSequencer (_dependencyMergerMock, new[] { _dependencyProviderMock1, _dependencyProviderMock2 });
    }

    [Test]
    public void UsesProviders ()
    {
      var advices = new[] { _advice1, _advice2, _advice3 };

      var fakeDependencies1 = new Tuple<Advice, Advice>[0];
      var fakeDependencies2 = new Tuple<Advice, Advice>[0];
      var fakeDependencies3 = new Tuple<Advice, Advice>[0];
      var dependencies = new[] { Tuple.Create (_advice3, _advice2), Tuple.Create (_advice2, _advice1) };

      _dependencyProviderMock1
          .Expect (x => x.GetDependencies (advices))
          .Return (fakeDependencies1);
      _dependencyMergerMock
          .Expect (x => x.MergeDependencies (Arg<IEnumerable<Tuple<Advice, Advice>>>.Matches (y => !y.Any()), Arg.Is (fakeDependencies1)))
          .Return (fakeDependencies2);
      _dependencyProviderMock2
          .Expect (x => x.GetDependencies (advices))
          .Return (fakeDependencies3);
      _dependencyMergerMock
          .Expect (x => x.MergeDependencies (fakeDependencies2, fakeDependencies3))
          .Return (dependencies);

      var result = _adviceSequencer.Sort (advices);

      Assert.That (result, Is.EqualTo (new[] { _advice3, _advice2, _advice1 }));
    }

    [Test]
    [Ignore ("TODO")]
    public void UsesPriorityFirst () {}

    [Test]
    public void ThrowsForUndefinedOrder ()
    {
      var advices = new[] { _advice1, _advice2, _advice3 };

      var dependencies = new[] { Tuple.Create (_advice3, _advice2) };
      _dependencyProviderMock1.Expect (x => x.GetDependencies (null)).IgnoreArguments().Return (dependencies);
      _dependencyMergerMock.Expect (x => x.MergeDependencies (null, null)).IgnoreArguments().Return (dependencies);

      var adviceSequencer = new AdviceSequencer (_dependencyMergerMock, new[] { _dependencyProviderMock1 });

      Assert.That (() => adviceSequencer.Sort (advices), Throws.TypeOf<UndefinedOrderException>());
    }

    [Test]
    public void ThrowsForCircularDependencies ()
    {
      var advices = new[] { _advice1, _advice2, _advice3 };

      var dependencies =
          new[]
          {
              Tuple.Create (_advice3, _advice2),
              Tuple.Create (_advice2, _advice1),
              Tuple.Create (_advice1, _advice3)
          };
      _dependencyProviderMock1.Expect (x => x.GetDependencies (null)).IgnoreArguments().Return (dependencies);
      _dependencyMergerMock.Expect (x => x.MergeDependencies (null, null)).IgnoreArguments().Return (dependencies);

      var adviceSequencer = new AdviceSequencer (_dependencyMergerMock, new[] { _dependencyProviderMock1 });

      Assert.That (() => adviceSequencer.Sort (advices), Throws.TypeOf<CircularDependencyException>());
    }
  }
}