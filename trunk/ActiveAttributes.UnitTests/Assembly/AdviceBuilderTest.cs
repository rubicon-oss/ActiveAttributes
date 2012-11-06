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
using System.Reflection;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Construction;
using ActiveAttributes.Core.Infrastructure.Pointcuts;
using NUnit.Framework;
using System.Linq;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AdviceBuilderTest
  {
    [Test]
    public void SetValues ()
    {
      var construction = ObjectMother2.GetConstruction();
      var method = ObjectMother2.GetMethodInfo();
      var pointcut = ObjectMother2.GetPointcut();

      var builder = new AdviceBuilder()
          .SetConstruction (construction)
          .SetMethod (method)
          .SetName ("name")
          .SetRole ("role")
          .SetExecution (AdviceExecution.Around)
          .SetScope (AdviceScope.Static)
          .SetPriority (1)
          .AddPointcut (pointcut);

      var result = builder.Build();

      Assert.That (result.Construction, Is.SameAs (construction));
      Assert.That (result.Method, Is.SameAs (method));
      Assert.That (result.Name, Is.EqualTo ("name"));
      Assert.That (result.Role, Is.EqualTo ("role"));
      Assert.That (result.Execution, Is.EqualTo (AdviceExecution.Around));
      Assert.That (result.Scope, Is.EqualTo (AdviceScope.Static));
      Assert.That (result.Priority, Is.EqualTo (1));
      Assert.That (result.Pointcuts, Has.Member (pointcut));
    }

    [Test]
    public void ThrowsForMultipleSet ()
    {
      CheckThrowForMultipleSet (x => x.SetConstruction (ObjectMother2.GetConstruction()));
      CheckThrowForMultipleSet (x => x.SetMethod (ObjectMother2.GetMethodInfo()));
      CheckThrowForMultipleSet (x => x.SetName ("name"));
      CheckThrowForMultipleSet (x => x.SetRole ("role"));
      CheckThrowForMultipleSet (x => x.SetExecution (AdviceExecution.Around));
      CheckThrowForMultipleSet (x => x.SetScope (AdviceScope.Static));
      CheckThrowForMultipleSet (x => x.SetPriority (2));
    }

    [Test]
    public void ThrowsForMultiplePointcutTypes ()
    {
      var builder = new AdviceBuilder();

      var pointcutType = typeof (TypePointcut);

      builder.AddPointcut (ObjectMother2.GetPointcut (pointcutType));
      Assert.That (() => builder.AddPointcut (ObjectMother2.GetPointcut (pointcutType)), Throws.Exception);
    }

    [Test]
    public void ThrowsForForMissing ()
    {
      var construction = ObjectMother2.GetConstruction();
      var method = ObjectMother2.GetMethodInfo();
      var execution = AdviceExecution.Around;
      var scope = AdviceScope.Static;

      CheckThrowForMissing (construction: null, method: method, execution: execution, scope: scope);
      CheckThrowForMissing (method: null, construction: construction, execution: execution, scope: scope);
      CheckThrowForMissing (execution: AdviceExecution.Undefined, method: method, construction: construction, scope: scope);
      CheckThrowForMissing (scope: AdviceScope.Undefined, method: method, execution: execution, construction: construction);
    }

    [Test]
    public void CanOverwriteConstructionIfMoreMeaningful ()
    {
      // Construction from CustomAttributeData has more information than construction from Type
      var typeConstruction = ObjectMother2.GetConstructionByType (typeof (TypeConstruction));
      var customAttributeDataConstruction = ObjectMother2.GetConstructionByType (typeof (CustomAttributeDataConstruction));

      var builder = new AdviceBuilder();

      builder.SetConstruction (typeConstruction);
      Assert.That (() => builder.SetConstruction (customAttributeDataConstruction), Throws.Nothing);
      Assert.That (() => builder.SetConstruction (customAttributeDataConstruction), Throws.Exception);
    }

    [Test]
    public void Copy ()
    {
      var builder = new AdviceBuilder()
          .SetConstruction (ObjectMother2.GetConstruction())
          .SetMethod (ObjectMother2.GetMethodInfo())
          .SetName ("name")
          .SetRole ("name")
          .SetExecution (AdviceExecution.Around)
          .SetScope (AdviceScope.Static)
          .SetPriority (1)
          .AddPointcut (ObjectMother2.GetPointcut());

      var copiedAdvice = builder.Copy().Build();
      var originalAdvice = builder.Build();

      Assert.That (copiedAdvice.Construction, Is.SameAs (originalAdvice.Construction));
      Assert.That (copiedAdvice.Method, Is.SameAs (originalAdvice.Method));
      Assert.That (copiedAdvice.Name, Is.EqualTo (originalAdvice.Name));
      Assert.That (copiedAdvice.Role, Is.EqualTo (originalAdvice.Role));
      Assert.That (copiedAdvice.Execution, Is.EqualTo (originalAdvice.Execution));
      Assert.That (copiedAdvice.Scope, Is.EqualTo (originalAdvice.Scope));
      Assert.That (copiedAdvice.Priority, Is.EqualTo (originalAdvice.Priority));
      Assert.That (copiedAdvice.Pointcuts, Is.EquivalentTo (originalAdvice.Pointcuts));
    }

    private void CheckThrowForMissing (
        IConstruction construction = null,
        MethodInfo method = null,
        AdviceExecution execution = AdviceExecution.Undefined,
        AdviceScope scope = AdviceScope.Undefined)
    {
      var builder = new AdviceBuilder()
          .SetConstruction (construction)
          .SetMethod (method)
          .SetExecution (execution)
          .SetScope (scope);
      Assert.That (() => builder.Build(), Throws.Exception);
    }

    private void CheckThrowForMultipleSet (Action<AdviceBuilder> action)
    {
      var adviceBuilder = new AdviceBuilder();
      Assert.That (
          () =>
          {
            action (adviceBuilder);
            action (adviceBuilder);
          },
          Throws.Exception);
    }
  }
}