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
using System.Reflection;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Interception.Contexts;
using ActiveAttributes.Core.Interception.Invocations;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace ActiveAttributes.UnitTests.Assembly.Done
{
  [TestFixture]
  public class InvocationTypeProvider2Test
  {
    private InvocationTypeProvider2 _provider;

    private Type _invocationType;
    private Type _invocationContextType;

    [SetUp]
    public void SetUp ()
    {
      _provider = new InvocationTypeProvider2();
    }

    [Test]
    public void GetInvocationTypes_Action ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method1 (""));
      var expectedInvocationType = typeof (ActionInvocation<DomainType, string>);
      var expectedInvocationContextType = typeof (ActionInvocationContext<DomainType, string>);

      CheckInvocationTypes (method, expectedInvocationType, expectedInvocationContextType);
    }

    [Test]
    public void GetInvocationTypes_Func ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method2 (""));
      var expectedInvocationType = typeof (FuncInvocation<DomainType, string, int>);
      var expectedInvocationContextType = typeof (FuncInvocationContext<DomainType, string, int>);

      CheckInvocationTypes (method, expectedInvocationType, expectedInvocationContextType);
    }

    [Test]
    public void GetInvocationTypes_Property_Get ()
    {
      var method = typeof (DomainType).GetMethods().Single (x => x.Name == "get_Property");
      var expectedInvocationType = typeof (PropertyGetInvocation<DomainType, string>);
      var expectedInvocationContextType = typeof (PropertyGetInvocationContext<DomainType, string>);

      CheckInvocationTypes (method, expectedInvocationType, expectedInvocationContextType);
    }

    [Test]
    public void GetInvocationTypes_Property_Set ()
    {
      var method = typeof (DomainType).GetMethods().Single (x => x.Name == "set_Property");
      var expectedInvocationType = typeof (PropertySetInvocation<DomainType, string>);
      var expectedInvocationContextType = typeof (PropertySetInvocationContext<DomainType, string>);

      CheckInvocationTypes (method, expectedInvocationType, expectedInvocationContextType);
    }

    private void CheckInvocationTypes (MethodInfo method, Type expectedInvocationType, Type expectedInvocationContextType)
    {
      Type actualInvocationType;
      Type actualInvocationContextType;
      var provider = new InvocationTypeProvider2();

      provider.GetInvocationTypes (method, out actualInvocationType, out actualInvocationContextType);

      Assert.That (actualInvocationType, Is.EqualTo (expectedInvocationType));
      Assert.That (actualInvocationContextType, Is.EqualTo (expectedInvocationContextType));
    }

    class DomainType
    {
      public void Method1 (string a) { }
      public int Method2 (string a) { return 1; }

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