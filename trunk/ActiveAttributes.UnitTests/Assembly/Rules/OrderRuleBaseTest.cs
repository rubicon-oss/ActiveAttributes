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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Configuration.Rules;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Assembly.Rules
{
  [TestFixture]
  public class OrderRuleBaseTest
  {
    [Test]
    public void Source ()
    {
      var source = "my source";
      var rule = new TestableOrderRule (source);

      Assert.That (rule.Source, Is.EqualTo (source));
      Assert.That (rule.ToString (), Is.EqualTo ("TestableOrderRule [my source]"));
    }

    private class TestableOrderRule : OrderRuleBase
    {
      public TestableOrderRule (string source)
        : base(source)
      {
      }

      public override int Compare (IAspectGenerator x, IAspectGenerator y)
      {
        throw new NotImplementedException();
      }
    }
  }
}