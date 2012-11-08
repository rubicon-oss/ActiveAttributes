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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [Ignore]
  [TestFixture]
  public class ProceedTest : TestBase
  {
    [Test]
    public void Proceed ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      Assert.That (() => instance.ThrowingMethod1(), Throws.Nothing);
      Assert.That (() => instance.ThrowingMethod2(), Throws.Exception);
    }

    public class DomainType
    {
      [ProceedingAspect]
      public virtual void ThrowingMethod1 () { throw new Exception (); }

      [NotProceedingAspect]
      public virtual void ThrowingMethod2 () { throw new Exception (); }
    }

    public class ProceedingAspectAttribute : MethodInterceptionAspectAttributeBase
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Proceed();
      }
    }
    
    public class NotProceedingAspectAttribute : MethodInterceptionAspectAttributeBase
    {
      public override void OnIntercept (IInvocation invocation)
      {
        invocation.Proceed();
      }
    }
  }
}