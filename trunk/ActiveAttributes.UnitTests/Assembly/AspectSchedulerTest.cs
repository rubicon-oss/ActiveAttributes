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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Assembly.Configuration.Rules;
using ActiveAttributes.Core.Utilities;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectSchedulerTest
  {
    private IAspectScheduler _scheduler;
    private IAspectConfiguration _configuration;

    private IAspectGenerator _generator1;
    private IAspectGenerator _generator2;
    private IAspectGenerator _generator3;
    private IAspectGenerator _generator4;

    private IAspectDescriptor _descriptor1;
    private IAspectDescriptor _descriptor2;
    private IAspectDescriptor _descriptor3;
    private IAspectDescriptor _descriptor4;

    [SetUp]
    public void SetUp ()
    {
      _configuration = MockRepository.GenerateMock<IAspectConfiguration>();
      _scheduler = new AspectScheduler (_configuration);

      _descriptor1 = MockRepository.GenerateMock<IAspectDescriptor>();
      _descriptor2 = MockRepository.GenerateMock<IAspectDescriptor>();
      _descriptor3 = MockRepository.GenerateMock<IAspectDescriptor>();
      _descriptor4 = MockRepository.GenerateMock<IAspectDescriptor>();

      _descriptor1.Expect (x => x.AspectType).Return (typeof (Aspect1));
      _descriptor2.Expect (x => x.AspectType).Return (typeof (Aspect2));
      _descriptor3.Expect (x => x.AspectType).Return (typeof (Aspect3));
      _descriptor4.Expect (x => x.AspectType).Return (typeof (Aspect4));

      _generator1 = MockRepository.GenerateMock<IAspectGenerator>();
      _generator2 = MockRepository.GenerateMock<IAspectGenerator>();
      _generator3 = MockRepository.GenerateMock<IAspectGenerator>();
      _generator4 = MockRepository.GenerateMock<IAspectGenerator>();

      _generator1.Expect (x => x.Descriptor).Return (_descriptor1);
      _generator2.Expect (x => x.Descriptor).Return (_descriptor2);
      _generator3.Expect (x => x.Descriptor).Return (_descriptor3);
      _generator4.Expect (x => x.Descriptor).Return (_descriptor4);
    }

    [Test]
    public void Orders ()
    {
      var rules = new List<IOrderRule>
                  {
                    new TypeOrderRule("", typeof(Aspect2), typeof(Aspect1)),
                    new TypeOrderRule("", typeof(Aspect1), typeof(Aspect3))
                  };
      _configuration.Expect (x => x.Rules).Return (rules);
      var aspects = new[] { _generator1, _generator2, _generator3 };

      var actual = _scheduler.GetOrdered (aspects);
      var expected = new[] { _generator2, _generator1, _generator3 };
      Assert.That (actual, Is.EqualTo (expected));
    }

    [Test]
    public void ThrowsForCircularDependency ()
    {
      var rules = new List<IOrderRule>
                  {
                    new TypeOrderRule("", typeof(Aspect2), typeof(Aspect1)),
                    new TypeOrderRule("", typeof(Aspect1), typeof(Aspect2))
                  };
      _configuration.Expect (x => x.Rules).Return (rules);
      var aspects = new[] { _generator1, _generator2, _generator3 };

      Assert.That (() => _scheduler.GetOrdered (aspects), Throws.TypeOf<CircularDependencyException<IAspectGenerator>>());
    }

    [Test]
    public void ThrowsForUndefinedOrder ()
    {
      var rules = new List<IOrderRule>
                  {
                    new TypeOrderRule("", typeof(Aspect2), typeof(Aspect1)),
                  };
      _configuration.Expect (x => x.Rules).Return (new ReadOnlyCollection<IOrderRule> (rules));
      var aspects = new[] { _generator1, _generator2, _generator3 };

      Assert.That (() => _scheduler.GetOrdered (aspects), Throws.TypeOf<UndefinedOrderException>());
    }

    [Test]
    public void PriorityOverRules ()
    {
      _descriptor3.Expect (x => x.Priority).Return (1);
      var rules = new List<IOrderRule>
                  {
                    new TypeOrderRule("", typeof(Aspect2), typeof(Aspect1)),
                    new TypeOrderRule("", typeof(Aspect1), typeof(Aspect3)),
                  };
      _configuration.Expect (x => x.Rules).Return (rules);
      var aspects = new[] { _generator1, _generator3, _generator2 };

      var actual = _scheduler.GetOrdered (aspects).ToArray();
      var expected = new[] { _generator3, _generator2, _generator1 };
      Assert.That (actual, Is.EqualTo (expected));
    }

    private class Aspect1 : AspectAttribute { }
    private class Aspect2 : AspectAttribute { }
    private class Aspect3 : AspectAttribute { }
    private class Aspect4 : AspectAttribute { }
  }
}