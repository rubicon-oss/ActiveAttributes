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
using System.Linq;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration2.AspectDescriptorProviders;
using NUnit.Framework;

namespace ActiveAttributes.UnitTests.Configuration2.AspectDescriptorProviders
{
  [TestFixture]
  public class EventBasedDescriptorProviderTest
  {
    [Test]
    public void GetAspects ()
    {
      var method = typeof (DomainType).GetMethod ("add_Event");
      var provider = new EventBasedAspectDescriptorProvider();

      var result = provider.GetDescriptors (method).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.Matches<IAspectDescriptor> (x => x.Type == typeof (InheritingAspectAttribute)));
    }

    private class BaseType
    {
      [InheritingAspect]
      [NotInheritingAspect]
      public virtual event EventHandler Event;
    }

    private class DomainType : BaseType
    {
      public override event EventHandler Event;
    }

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    private class InheritingAspectAttribute : AspectAttribute {}

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    private class NotInheritingAspectAttribute : AspectAttribute {}
  }
}