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
using System.Linq;
using ActiveAttributes.Core.Discovery;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class AdviceMergerTest
  {
    private AdviceMerger _merger;

    [SetUp]
    public void SetUp ()
    {
      _merger = new AdviceMerger();
    }

    [Test]
    public void Merges ()
    {
      var method = ObjectMother2.GetMethodInfo();

      var pointcuts1 = new IPointcut[] { new TypePointcut (typeof (int)) };
      var pointcuts2 = new IPointcut[] { new TypeNamePointcut ("type") };

      var advice1 = ObjectMother2.GetAdvice (role: "Role1", execution: AdviceExecution.After, method: method, pointcuts: pointcuts1);
      var advice2 = ObjectMother2.GetAdvice (name: "Name2", scope: AdviceScope.Static, priority: 4, pointcuts: pointcuts2);

      var result = _merger.Merge (advice1, advice2);

      Assert.That (result.Method, Is.SameAs (method));
      Assert.That (result.Name, Is.EqualTo ("Name2"));
      Assert.That (result.Role, Is.EqualTo ("Role1"));
      Assert.That (result.Execution, Is.EqualTo (AdviceExecution.After));
      Assert.That (result.Scope, Is.EqualTo (AdviceScope.Static));
      Assert.That (result.Priority, Is.EqualTo (4));
      Assert.That (result.Pointcuts, Is.EquivalentTo (pointcuts1.Concat (pointcuts2)));
    }

    [Test]
    public void ThrowsForMultipleDeclaration ()
    {
      CheckThrows (ObjectMother2.GetAdvice (priority: 2), ObjectMother2.GetAdvice (priority: 2));
      CheckThrows (ObjectMother2.GetAdvice (name: "name1"), ObjectMother2.GetAdvice (name: "name2"));
      CheckThrows (ObjectMother2.GetAdvice (role: "role1"), ObjectMother2.GetAdvice (role: "role2"));
      CheckThrows (ObjectMother2.GetAdvice (execution: AdviceExecution.Before), ObjectMother2.GetAdvice (execution: AdviceExecution.Around));
      CheckThrows (ObjectMother2.GetAdvice (scope: AdviceScope.Static), ObjectMother2.GetAdvice (scope: AdviceScope.Instance));
    }

    [Test]
    public void ThrowsForMultiplePointcutTypes ()
    {
      var advice1 = ObjectMother2.GetAdvice (pointcuts: new IPointcut[] { new TypePointcut (typeof (int)) });
      var advice2 = ObjectMother2.GetAdvice (pointcuts: new IPointcut[] { new TypePointcut (typeof (int)) });

      Assert.That (() => _merger.Merge (advice1, advice2), Throws.Exception);
    }

    private void CheckThrows (Advice advice1, Advice advice2)
    {
      Assert.That (() => _merger.Merge (advice1, advice2), Throws.Exception);
    }
  }
}