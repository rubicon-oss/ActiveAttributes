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
    private IConfiguration _configuration;

    private IDescriptor _descriptor1;
    private IDescriptor _descriptor2;
    private IDescriptor _descriptor3;

    [SetUp]
    public void SetUp ()
    {
      _configuration = MockRepository.GenerateMock<IConfiguration>();

      _descriptor1 = MockRepository.GenerateMock<IDescriptor>();
      _descriptor2 = MockRepository.GenerateMock<IDescriptor>();
      _descriptor3 = MockRepository.GenerateMock<IDescriptor>();

      _descriptor1.Expect (x => x.AspectType).Return(typeof(Aspect1Attribute));
      _descriptor2.Expect (x => x.AspectType).Return(typeof(Aspect2Attribute));
      _descriptor3.Expect (x => x.AspectType).Return(typeof(Aspect3Attribute));
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

      var resultGreater = rule.Compare (_descriptor1, _descriptor2);
      var resultLess = rule.Compare (_descriptor2, _descriptor1);
      var resultEqual = rule.Compare (_descriptor1, _descriptor3);

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


    [Test]
    public void Equals_ ()
    {
      var mySource = "my source";
      var type1 = "role1";
      var type2 = "role2";
      var type3 = "role3";

      var rule1 = new RoleOrderRule (mySource, type1, type2, _configuration);
      var rule2 = new RoleOrderRule (mySource, type1, type2, _configuration);
      var rule3 = new RoleOrderRule (mySource, type2, type3, _configuration);

      Assert.That (rule1, Is.EqualTo (rule2));
      Assert.That (rule1, Is.Not.EqualTo (rule3));
      Assert.That (rule1.GetHashCode (), Is.EqualTo (rule2.GetHashCode ()));
      Assert.That (rule1.GetHashCode (), Is.Not.EqualTo (rule3.GetHashCode ()));
    }

    private class Aspect1Attribute : AspectAttribute { }
    private class Aspect2Attribute : AspectAttribute { }
    private class Aspect3Attribute : Aspect1Attribute { }
  }
}