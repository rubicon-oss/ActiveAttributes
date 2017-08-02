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
using ActiveAttributes.Annotations;
using ActiveAttributes.Annotations.Pointcuts;
using ActiveAttributes.Infrastructure;
using ActiveAttributes.Infrastructure.Builder;
using ActiveAttributes.Pointcuts;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Infrastructure
{
  [TestFixture]
  public class AspectBuilderTest
  {
    private AspectBuilder _builder;

    [SetUp]
    public void SetUp ()
    {
      _builder = new AspectBuilder (new AspectElementBuilder (new PointcutBuilder()), new PointcutBuilder());
    }

    [Test]
    public void name ()
    {
      var x = AspectActivation.Auto;
      Assert.That (x, Is.EqualTo (1));
      var aspect = _builder.Build (typeof (DomainAspect));
    }

    [Aspect (AspectActivation.Manual, AspectScope.Transient)]
    //[TypePointcut (typeof (int))]
    public class DomainAspect : IDisposable
    {
      [ImportMember ("test", true)]
      public Action Import;

      [IntroduceMember (ConflictAction.Override)]
      public void Dispose () {}

      [Advice (AdviceExecution.Around)]
      [InheritPointcut, And, TypePointcut (typeof (int))]
      public void Advice () {}
    }
  }
}