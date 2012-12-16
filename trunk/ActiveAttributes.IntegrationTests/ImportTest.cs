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
using System.Reflection;
using ActiveAttributes.Annotations;
using ActiveAttributes.Annotations.Ordering;
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
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ImportTest
  {
    [Test]
    public void Execution1 ()
    {
      var instance = ObjectFactory.Create<DomainType> ();
      ObjectFactory.Save();

      Assert.That (instance.Method1(), Is.EqualTo (2));
      Assert.That (instance.Method2(), Is.EqualTo (0));
    }

    [DomainAspect]
    public class DomainType
    {
      public virtual int Method1 ()
      {
        return 0;
      }

      public virtual int Method2 ()
      {
        return 0;
      }

      public virtual int Method3()
      {
        return 2;
      }
    }

    [AspectRoleOrdering (Position.Before, StandardRoles.ExceptionHandling)]
    public class DomainAspect : AspectAttributeBase
    {
      [ImportMember ("Method3", true)]
      public Func<int> Method3; 

      public DomainAspect ()
          : base (AspectScope.PerObject, StandardRoles.Caching) {}

      [Advice (AdviceExecution.Around)]
      [MethodExecutionPointcut (Name = "Method1")]
      public void Around (IContext context)
      {
        context.ReturnValue = Method3();
      }
    }
  }
}