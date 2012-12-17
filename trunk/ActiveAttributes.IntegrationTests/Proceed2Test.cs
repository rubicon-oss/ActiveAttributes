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
using ActiveAttributes.Annotations;
using ActiveAttributes.Annotations.Pointcuts;
using ActiveAttributes.Aspects;
using ActiveAttributes.Infrastructure;
using ActiveAttributes.Infrastructure.Ordering;
using ActiveAttributes.Weaving;
using ActiveAttributes.Weaving.Context;
using ActiveAttributes.Weaving.Invocation;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using System.Linq;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class Proceed2Test
  {
    [Test]
    public void Execution1 ()
    {
      var instance = ObjectFactory.Create<DomainType> ();
      ObjectFactory.Save();
      var method = instance.Method (1);
      Assert.That (method, Is.EqualTo (4));
    }

    public class DomainType
    {

      [DomainAspect]
      public virtual int Method (int i)
      {
        Assert.That (i, Is.EqualTo (2));
        return i + 1;
      }
    }

    public class DomainAspect : MethodInterceptionAttributeBase
    {
      public DomainAspect ()
          : base (AspectScope.Singleton) {}

      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Arguments[0] = (int) invocation.Arguments[0] + 1;
        invocation.Proceed();
        invocation.ReturnValue = (int) invocation.ReturnValue + 1;
      }
    }
  }
}