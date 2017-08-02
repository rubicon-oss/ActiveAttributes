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
using ActiveAttributes.Advices;
using ActiveAttributes.Aspects;
using ActiveAttributes.Assembly;
using ActiveAttributes.Pointcuts;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class CustomPointcutTest
  {
    [Test]
    public void Test ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      Assert.That (() => instance.Method1(), Throws.Nothing);
      Assert.That (() => instance.Method2(), Throws.Exception);
    }

    public class DomainType
    {
      [DomainAspect]
      public virtual void Method1 () { throw new Exception (); }

      [DomainAspect]
      public virtual void Method2 () { throw new Exception (); }
    }

    public class DomainAspect : AspectAttributeBase
    {
      [AdviceInfo (Execution = AdviceExecution.Around, Scope = AdviceScope.Static)]
      [CustomPointcut("Method1", typeof(void))]
      public void Advice () { }
    }

    public class CustomPointcutAttribute : PointcutAttributeBase
    {
      public CustomPointcutAttribute (string memberName, Type returnType)
          : base (new CustomPointcut1 (memberName, returnType)) {}
    }

    public class CustomPointcut1 : IMemberNamePointcut, IReturnTypePointcut
    {
      public CustomPointcut1 (string memberName, Type returnType)
      {
        MemberName = memberName;
        ReturnType = returnType;
      }

      public bool Accept (IPointcutEvaluator evaluator, JoinPoint joinPoint)
      {
        return evaluator.Visit ((IPointcut) this, joinPoint);
      }

      public string MemberName { get; private set; }
      public Type ReturnType { get; private set; }
    }
  }
}