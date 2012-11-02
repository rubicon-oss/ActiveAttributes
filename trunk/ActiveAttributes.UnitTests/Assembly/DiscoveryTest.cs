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
using ActiveAttributes.Core.Attributes.Pointcuts;
using ActiveAttributes.Core.Infrastructure;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class DiscoveryTest
  {
    class DomainAspect1 : IAspect
    {
      [TypePointcut (typeof (string))]
      public void AdviceMethod1 () {}

      [TypePointcut (typeof (string))]
      [ReturnTypePointcut (typeof (bool))]
      public void AdviceMethod2 () {}
    }

    [TypePointcut (typeof (string))]
    class DomainAspect2 : IAspect
    {
      [ReturnTypePointcut (typeof (bool))]
      public void AdviceMethod () {}

      public void NonAdviceMethod () {}
    }

    [TypePointcut (typeof (string))]
    class DomainAspect3 : IAspect
    {
      [TypePointcut (typeof (object))]
      public void InvalidAdviceMethod () {}
    }


  }
}