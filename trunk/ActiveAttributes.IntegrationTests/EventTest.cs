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
using ActiveAttributes.Aspects;
using ActiveAttributes.Aspects.Ordering;
using NUnit.Framework;

namespace ActiveAttributes.IntegrationTests
{
  [TestFixture]
  public class EventTest
  {
    [Test]
    public void name ()
    {
      var instance = ObjectFactory.Create<DomainType2>();
      ObjectFactory.Save();
      var result = instance.Method ("abc");
      Assert.That (result, Is.EqualTo ("abc"));
    }

    public class DomainType2
    {
      public virtual string Method (string abc)
      {
        return abc;
      }
    }

    [Test]
    public void Execution1 ()
    {
      var instance = ObjectFactory.Create<DomainType> ();
      ObjectFactory.Save();

      instance.Event += InstanceOnEvent;
      instance.Raise ();
      instance.Event -= InstanceOnEvent;
    }

    private void InstanceOnEvent (object sender, EventArgs eventArgs)
    {
        
    }

    public class DomainType
    {
    [DomainAspect]
      public virtual event EventHandler Event;

      public void Raise ()
      {
        Event (this, null);
      }
    }

    [AspectRoleOrdering (Position.Before, StandardRoles.ExceptionHandling)]
    public class DomainAspect : EventInterceptionAttributeBase
    {
      public DomainAspect ()
          : base (AspectScope.PerType, StandardRoles.Caching) {}

      public override void OnInvoke (IInvocation invocation)
      {
        invocation.Proceed ();
      }

      public override void OnAdd (IInvocation invocation)
      {
        invocation.Proceed();
      }

      public override void OnRemove (IInvocation invocation)
      {
        invocation.Proceed();
      }
    }
  }
}