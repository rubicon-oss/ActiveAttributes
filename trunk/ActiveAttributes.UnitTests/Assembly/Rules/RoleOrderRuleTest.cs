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
// 
using System;
using System.Collections.Generic;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Assembly.Configuration.Rules;
using NUnit.Framework;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly.Rules
{
  [TestFixture]
  public class RoleOrderRuleTest
  {
    private IAspectConfiguration _configuration;

    private IAspectGenerator _generator1;
    private IAspectGenerator _generator2;
    private IAspectGenerator _generator3;

    [SetUp]
    public void SetUp ()
    {
      _configuration = MockRepository.GenerateMock<IAspectConfiguration>();

      var descriptor1 = MockRepository.GenerateMock<IAspectDescriptor>();
      var descriptor2 = MockRepository.GenerateMock<IAspectDescriptor>();
      var descriptor3 = MockRepository.GenerateMock<IAspectDescriptor>();

      descriptor1.Expect (x => x.AspectType).Return(typeof(Aspect1Attribute));
      descriptor2.Expect (x => x.AspectType).Return(typeof(Aspect2Attribute));
      descriptor3.Expect (x => x.AspectType).Return(typeof(Aspect3Attribute));

      _generator1 = MockRepository.GenerateMock<IAspectGenerator> ();
      _generator2 = MockRepository.GenerateMock<IAspectGenerator> ();
      _generator3 = MockRepository.GenerateMock<IAspectGenerator> ();

      _generator1.Expect (x => x.Descriptor).Return(descriptor1);
      _generator2.Expect (x => x.Descriptor).Return(descriptor2);
      _generator3.Expect (x => x.Descriptor).Return(descriptor3);
    }

    [Test]
    public void Normal ()
    {
      var role1 = "Role1";
      var role2 = "Role2";
      var roles = new Dictionary<Type, string>
                  {
                    { typeof(Aspect1Attribute), role1 },
                    { typeof(Aspect2Attribute), role2 }
                  };
      _configuration.Expect (x => x.Roles).Return (roles);

      var rule = new RoleOrderRule ("", role2, role1, _configuration);

      var resultGreater = rule.Compare (_generator1, _generator2);
      var resultLess = rule.Compare (_generator2, _generator1);
      var resultEqual = rule.Compare (_generator1, _generator3);

      Assert.That (resultGreater, Is.EqualTo (1));
      Assert.That (resultLess, Is.EqualTo (-1));
      Assert.That (resultEqual, Is.EqualTo (0));
    }

    [Test]
    public void ToString_ ()
    {
      var mySource = "my source";
      var role1 = "role1";
      var role2 = "role2";
      var rule = new RoleOrderRule (mySource, role1, role2, _configuration);

      var result = rule.ToString();

      Assert.That (result, Is.EqualTo ("RoleOrderRule [" + mySource + "]: " + role1 + " -> " + role2));
    }

    private class Aspect1Attribute : AspectAttribute { }
    private class Aspect2Attribute : AspectAttribute { }
    private class Aspect3Attribute : Aspect1Attribute { }
  }
}