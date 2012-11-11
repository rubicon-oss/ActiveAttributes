﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using ActiveAttributes.Advices;
using ActiveAttributes.Assembly;
using ActiveAttributes.Extensions;
using ActiveAttributes.Interception.Invocations;
using ActiveAttributes.Pointcuts;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class AspectClassTest : TypeAssemblerIntegrationTestBase
  {
    [Test]
    public void AspectClass ()
    {
      var assembleType = AssembleType<DomainType> (ObjectFactory.Assembler.ModifyType);
      var instance = assembleType.CreateInstance<DomainType> ();

      Assert.That (() => instance.ThrowingMethod(), Throws.Nothing);
    }

    public class DomainType
    {
      public virtual void ThrowingMethod () { throw new Exception (); }
    }

    [AdviceInfo (Execution = AdviceExecution.Around, Scope = AdviceScope.Static)]
    [TypePointcut (typeof (DomainType))]
    public class DomainAspect : IAspect
    {
      public void Advice (IInvocation invocation) {}
    }
  }
}