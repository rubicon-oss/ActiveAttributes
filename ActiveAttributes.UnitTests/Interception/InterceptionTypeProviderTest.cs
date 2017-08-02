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
using ActiveAttributes.Interception;
using ActiveAttributes.Weaving;
using ActiveAttributes.Weaving.Context;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.ServiceLocation;

namespace ActiveAttributes.UnitTests.Interception
{
  [TestFixture]
  public class InterceptionTypeProviderTest
  {
    private InvocationTypeProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new InvocationTypeProvider ();
    }

    [Test]
    public void GetInvocationTypes_Action ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method1 ());

      var actual = _provider.GetInvocationType (method);

      Assert.That (actual, Is.EqualTo (typeof (ActionContext<DomainType>)));
    }

    [Test]
    public void GetInvocationTypes_Action2 ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method2 (""));

      var actual = _provider.GetInvocationType (method);

      Assert.That (actual, Is.EqualTo (typeof (ActionContext<DomainType, string>)));
    }

    [Test]
    public void GetInvocationTypes_Func ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method3 ());

      var actual = _provider.GetInvocationType (method);

      Assert.That (actual, Is.EqualTo (typeof (FuncContext<DomainType, int>)));
    }


    [Test]
    public void GetInvocationTypes_Func2 ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method4 (1));

      var actual = _provider.GetInvocationType (method);

      Assert.That (actual, Is.EqualTo (typeof (FuncContext<DomainType, int, int>)));
    }

    [Test]
    public void GetInvocationTypes_Property_Get ()
    {
      var method = typeof (DomainType).GetMethods ().Single (x => x.Name == "get_Property");

      var actual = _provider.GetInvocationType (method);

      Assert.That (actual, Is.EqualTo (typeof (FuncContext<DomainType, string>)));
    }

    [Test]
    public void GetInvocationTypes_Property_Set ()
    {
      var method = typeof (DomainType).GetMethods ().Single (x => x.Name == "set_Property");

      var actual = _provider.GetInvocationType (method);

      Assert.That (actual, Is.EqualTo (typeof (ActionContext<DomainType, string>)));
    }

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IInvocationTypeProvider>();

      Assert.That (instance, Is.TypeOf<InvocationTypeProvider>());
    }

    class DomainType
    {
      public void Method1 () { }
      public void Method2 (string a) { }
      public int Method3 () { return 1; }
      public int Method4 (int i) { return i; }

      public string Property
      {
        get { return ""; }
        set { }
      }

      public string this[int idx]
      {
        get { return ""; }
        set { }
      }

      public event EventHandler Event;
    }
  }
}