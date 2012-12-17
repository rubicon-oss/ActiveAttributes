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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using System.Linq;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class ProceedTest
  {
    [Test]
    public void Execution1 ()
    {
      var instance = ObjectFactory.Create<DomainType> ();
      instance.Method (1);
      ObjectFactory.Save();
      var execution = string.Join (" ", instance.Execution.ToArray ());
      Assert.That (execution, Is.EqualTo ("Before Method AfterReturning After"));
    }

    [Test]
    public void Execution2 ()
    {
      var instance = ObjectFactory.Create<DomainType> ();
      instance.ThrowingMethod ();

      var execution = string.Join (" ", instance.Execution.ToArray ());
      Assert.That (execution, Is.EqualTo ("Before Method AfterThrowing After"));
    }

    public class DomainType
    {
      public List<string> Execution = new List<string>();

      [DomainAspect]
      public virtual void Method (int i)
      {
        Execution.Add ("Method");
      }

      [DomainAspect]
      public virtual void ThrowingMethod ()
      {
        Execution.Add ("Method");
        throw new Exception();
      }
    }

    public class DomainAspect : AspectAttributeBase
    {
      public DomainAspect ()
          : base (AspectScope.Transient) {}

      [Advice (AdviceExecution.Before)]
      [MethodExecutionPointcut]
      public void Before (IContext context)
      {
        var instance = (DomainType) context.Instance;
        instance.Execution.Add ("Before");
      }

      [Advice (AdviceExecution.After)]
      [MethodExecutionPointcut]
      public void After (IContext context)
      {
        var instance = (DomainType) context.Instance;
        instance.Execution.Add ("After");
      }

      [Advice (AdviceExecution.AfterReturning)]
      [MethodExecutionPointcut]
      public void AfterReturning (IContext context)
      {
        var instance = (DomainType) context.Instance;
        instance.Execution.Add ("AfterReturning");
      }

      [Advice (AdviceExecution.AfterThrowing)]
      [MethodExecutionPointcut]
      public void AfterThrowing (IContext context)
      {
        var instance = (DomainType) context.Instance;
        instance.Execution.Add ("AfterThrowing");
      }
    }
  }
}