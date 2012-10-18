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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration2;
using ActiveAttributes.Core.Configuration2.Rules;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Configuration2.Rules
{
  [TestFixture]
  public class TypeOrderingRuleTest
  {
    private IActiveAttributesConfiguration _activeAttributesConfiguration;

    private IAspectDescriptor _descriptor1;
    private IAspectDescriptor _descriptor2;
    private IAspectDescriptor _descriptor3;
    private IAspectDescriptor _descriptor4;

    [SetUp]
    public void SetUp ()
    {
      _activeAttributesConfiguration = MockRepository.GenerateMock<IActiveAttributesConfiguration> ();

      _descriptor1 = MockRepository.GenerateMock<IAspectDescriptor> ();
      _descriptor2 = MockRepository.GenerateMock<IAspectDescriptor> ();
      _descriptor3 = MockRepository.GenerateMock<IAspectDescriptor> ();
      _descriptor4 = MockRepository.GenerateMock<IAspectDescriptor> ();

      _descriptor1.Expect (x => x.Type).Return (typeof (Aspect1Attribute));
      _descriptor2.Expect (x => x.Type).Return (typeof (Aspect2Attribute));
      _descriptor3.Expect (x => x.Type).Return (typeof (Aspect3Attribute));
      _descriptor4.Expect (x => x.Type).Return (typeof (Aspect4Attribute));
    }

    [Test]
    public void Compare ()
    {
      var rule = new TypeOrderingRule ("source", typeof (Aspect1Attribute), typeof (Aspect2Attribute));

      var resultGreater = rule.Compare (_descriptor2, _descriptor1);
      var resultLess = rule.Compare (_descriptor1, _descriptor2);
      var resultEqual = rule.Compare (_descriptor2, _descriptor3);

      Assert.That (resultGreater, Is.EqualTo (1));
      Assert.That (resultLess, Is.EqualTo (-1));
      Assert.That (resultEqual, Is.EqualTo (0));
    }

    [Test]
    public void Compare_Subtype ()
    {
      var rule = new TypeOrderingRule ("source", typeof (Aspect1Attribute), typeof (Aspect2Attribute));

      var result = rule.Compare (_descriptor2, _descriptor4);

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void ToString_ ()
    {
      var type1 = typeof(Aspect1Attribute);
      var type2 = typeof(Aspect2Attribute);
      var rule = new TypeOrderingRule ("source", type1, type2);

      var result = rule.ToString ();

      Assert.That (result, Is.EqualTo ("TypeOrderingRule [source]: Aspect1Attribute -> Aspect2Attribute"));
    }

    [Test]
    public void Equals_ ()
    {
      var mySource = "source";
      var type1 = typeof (Aspect1Attribute);
      var type2 = typeof (Aspect2Attribute);
      var type3 = typeof (Aspect3Attribute);

      var rule1 = new TypeOrderingRule (mySource, type1, type2);
      var rule2 = new TypeOrderingRule (mySource, type1, type2);
      var rule3 = new TypeOrderingRule (mySource, type2, type3);

      Assert.That (rule1, Is.EqualTo (rule2));
      Assert.That (rule1, Is.Not.EqualTo (rule3));
      Assert.That (rule1.GetHashCode(), Is.EqualTo (rule2.GetHashCode()));
      Assert.That (rule1.GetHashCode(), Is.Not.EqualTo (rule3.GetHashCode()));
    }

    private class Aspect1Attribute : AspectAttribute { }
    private class Aspect2Attribute : AspectAttribute { }
    private class Aspect3Attribute : AspectAttribute { }
    private class Aspect4Attribute : Aspect1Attribute { }
  }
}