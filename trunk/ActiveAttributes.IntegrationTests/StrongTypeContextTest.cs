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

using ActiveAttributes.Annotations;
using ActiveAttributes.Annotations.Pointcuts;
using ActiveAttributes.Aspects;
using ActiveAttributes.Infrastructure;
using ActiveAttributes.Infrastructure.Ordering;
using ActiveAttributes.Weaving;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class StrongTypeContextTest
  {
    [Test]
    public void TypeResolution ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      instance.Method("arg");
    }

    public class DomainType
    {
      [DomainAspect]
      public virtual void Method (string arg)
      {
        
      }
    }

    public class DomainAspectAttribute : AspectAttributeBase
    {
      public DomainAspectAttribute ()
          : base(AspectScope.Transient) {}

      [Advice (AdviceExecution.Before)]
      [MethodExecutionPointcut]
      public void Advice (ref string arg)
      {
        Assert.That (arg, Is.Not.Null.And.EqualTo ("arg"));
      }
    }
  }
}