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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Contexts;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class TypeProviderTest
  {
    [Test]
    public void ActionMethod ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 (""));
      var provider = new TypeProvider (method);

      Assert.That (provider.InvocationType, Is.EqualTo (typeof (ActionInvocation<DomainClass, string>)));
      Assert.That (provider.InvocationContextType, Is.EqualTo (typeof (ActionInvocationContext<DomainClass, string>)));
    }
    [Test]
    public void FuncMethod ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method2 (""));
      var provider = new TypeProvider (method);

      Assert.That (provider.InvocationType, Is.EqualTo (typeof (FuncInvocation<DomainClass, string, int>)));
      Assert.That (provider.InvocationContextType, Is.EqualTo (typeof (FuncInvocationContext<DomainClass, string, int>)));
    }

    [Test]
    public void PropertyGetMethod ()
    {
      var method = typeof (DomainClass).GetMethods ().Single (x => x.Name == "get_Property");
      var provider = new TypeProvider (method);

      Assert.That (provider.InvocationType, Is.EqualTo (typeof (PropertyGetInvocation<DomainClass, string>)));
      Assert.That (provider.InvocationContextType, Is.EqualTo (typeof (PropertyGetInvocationContext<DomainClass, string>)));
    }

    [Test]
    public void PropertySetMethod ()
    {
      var method = typeof (DomainClass).GetMethods ().Single (x => x.Name == "set_Property");
      var provider = new TypeProvider (method);

      Assert.That (provider.InvocationType, Is.EqualTo (typeof (PropertySetInvocation<DomainClass, string>)));
      Assert.That (provider.InvocationContextType, Is.EqualTo (typeof (PropertySetInvocationContext<DomainClass, string>)));
    }


    [Test]
    public void IndexerGetMethod ()
    {
      var method = typeof (DomainClass).GetMethods ().Single (x => x.Name == "get_Item");
      var provider = new TypeProvider (method);

      Assert.That (provider.InvocationType, Is.EqualTo (typeof (IndexerGetInvocation<DomainClass, int, string>)));
      Assert.That (provider.InvocationContextType, Is.EqualTo (typeof (IndexerGetInvocationContext<DomainClass, int, string>)));
    }

    [Test]
    public void IndexerSetMethod ()
    {
      var method = typeof (DomainClass).GetMethods ().Single (x => x.Name == "set_Item");
      var provider = new TypeProvider (method);

      Assert.That (provider.InvocationType, Is.EqualTo (typeof (IndexerSetInvocation<DomainClass, int, string>)));
      Assert.That (provider.InvocationContextType, Is.EqualTo (typeof (IndexerSetInvocationContext<DomainClass, int, string>)));
    }

    //[Test]
    //public void EventAddMethod ()
    //{
    //  var method = typeof (DomainClass).GetMethods ().Single (x => x.Name == "add_Event");
    //  var provider = new TypeProvider (method);

    //  Assert.That (provider.InvocationType, Is.EqualTo (typeof (IndexerGetInvocation<DomainClass, int, string>)));
    //  Assert.That (provider.InvocationContextType, Is.EqualTo (typeof (IndexerGetInvocationContext<DomainClass, int, string>)));
    //}

    //[Test]
    //public void EventRemoveMethod ()
    //{
    //  var method = typeof (DomainClass).GetMethods ().Single (x => x.Name == "remove_Event");
    //  var provider = new TypeProvider (method);

    //  Assert.That (provider.InvocationType, Is.EqualTo (typeof (IndexerSetInvocation<DomainClass, int, string>)));
    //  Assert.That (provider.InvocationContextType, Is.EqualTo (typeof (IndexerSetInvocationContext<DomainClass, int, string>)));
    //}


    public class DomainClass
    {
      public void Method1 (string a) { }
      public int Method2 (string a) { return 1; }

      public string Property
      {
        get { return ""; }
        set { }
      }

      public string this [int idx]
      {
        get { return ""; }
        set { }
      }

      public event EventHandler Event;
    }
  }
}