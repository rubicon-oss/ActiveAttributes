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
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Ordering;
using ActiveAttributes.Core.Utilities;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting.Enumerables;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AdviceSequencerTest
  {
    private AdviceSequencer _adviceSequencer;

    private IAdviceDependencyProvider _dependencyProviderMock;

    private Advice _advice1;
    private Advice _advice2;
    private Advice _advice3;

    [SetUp]
    public void SetUp ()
    {
      _advice1 = ObjectMother2.GetAdvice();
      _advice2 = ObjectMother2.GetAdvice();
      _advice3 = ObjectMother2.GetAdvice();

      _dependencyProviderMock = MockRepository.GenerateStrictMock<IAdviceDependencyProvider>();
      _adviceSequencer = new AdviceSequencer (_dependencyProviderMock);
    }

    [Test]
    public void Sort ()
    {
      var advices = new[] { _advice1, _advice2, _advice3 };

      var dependencies = new[] { Tuple.Create (_advice3, _advice2), Tuple.Create (_advice2, _advice1) };

      _dependencyProviderMock
          .Expect (x => x.GetDependencies (advices))
          .Return (dependencies.AsOneTime());

      var result = _adviceSequencer.Sort (advices.AsOneTime());

      Assert.That (result, Is.EqualTo (new[] { _advice3, _advice2, _advice1 }));
    }

    [Test]
    public void Sort_ThrowsForUndefinedOrder ()
    {
      var advices = new[] { _advice1, _advice2, _advice3 };

      var dependencies = new[] { Tuple.Create (_advice3, _advice2) };
      _dependencyProviderMock.Expect (x => x.GetDependencies (null)).IgnoreArguments().Return (dependencies);

      Assert.That (() => _adviceSequencer.Sort (advices), Throws.TypeOf<UndefinedOrderException>());
    }

    [Test]
    public void Sort_ThrowsForCircularDependencies ()
    {
      var advices = new[] { _advice1, _advice2, _advice3 };

      var dependencies =
          new[]
          {
              Tuple.Create (_advice3, _advice2),
              Tuple.Create (_advice2, _advice1),
              Tuple.Create (_advice1, _advice3)
          };
      _dependencyProviderMock.Expect (x => x.GetDependencies (null)).IgnoreArguments().Return (dependencies);

      Assert.That (() => _adviceSequencer.Sort (advices), Throws.TypeOf<CircularDependencyException>());
    }
  }
}