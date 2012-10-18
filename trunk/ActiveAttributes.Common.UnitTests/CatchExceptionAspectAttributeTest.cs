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
using ActiveAttributes.Core;
using NUnit.Framework;

namespace ActiveAttributes.Common.UnitTests
{
  [TestFixture]
  public class CatchExceptionAspectAttributeTest
  {
    [Test]
    public void CatchesException ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      Assert.That (() => instance.CatchableThrow1 (), Throws.Nothing);
      Assert.That (() => instance.CatchableThrow2 (), Throws.Nothing);
    }

    [Test]
    public void RethrowsException ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      Assert.That (() => instance.UncatchableThrow (), Throws.Exception);
    }

    [Test]
    public void Proceeds ()
    {
      var instance = ObjectFactory.Create<DomainType>();

      instance.Method();

      Assert.That (instance.MethodExecuted, Is.True);
    }

    public class DomainType
    {
      [CatchExceptionAspect]
      public virtual void CatchableThrow1 ()
      {
        throw new Exception();
      }

      [CatchExceptionAspect(typeof(Exception))]
      public virtual void CatchableThrow2 ()
      {
        throw new InvalidOperationException ();
      }

      [CatchExceptionAspect(typeof(InvalidOperationException))]
      public virtual void UncatchableThrow ()
      {
        throw new Exception ();
      }

      public bool MethodExecuted { get; private set; }

      [CatchExceptionAspect]
      public virtual void Method ()
      {
        MethodExecuted = true;
      }
    }
  }
}