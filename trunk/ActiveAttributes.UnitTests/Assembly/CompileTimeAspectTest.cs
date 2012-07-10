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
// 

using System;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.CompileTimeAspects;
using ActiveAttributes.Core.Configuration;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly.CompileTimeAspects
{
  [TestFixture]
  public class CompileTimeAspectTest
  {
    [Test]
    public void Initialization ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method1()));

      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single();

      var result = new CompileTimeAspect (customData);

      Assert.That (result.Scope, Is.EqualTo (AspectScope.Instance));
      Assert.That (result.Priority, Is.EqualTo (10));
      Assert.That (result.AspectType, Is.EqualTo (typeof (DomainAspectAttribute)));
      Assert.That (result.ConstructorInfo, Is.EqualTo (customData.Constructor));
      Assert.That (result.ConstructorArguments, Is.EqualTo (customData.ConstructorArguments));
      Assert.That (result.NamedArguments, Is.EqualTo (customData.NamedArguments));
    }

    [Test]
    public void Initialization_NoData ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method3 ()));

      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();

      var result = new CompileTimeAspect (customData);

      Assert.That (result.Scope, Is.EqualTo (AspectScope.Static));
      Assert.That (result.Priority, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException), ExpectedMessage = "CustomAttributeData must be from an AspectAttribute")]
    public void name ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method2 ()));

      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();

      new CompileTimeAspect (customData);
    }

    public class DomainType
    {
      [DomainAspect(Scope = AspectScope.Instance, Priority = 10)]
      public void Method1 () { }

      [DomainNonAspect]
      public void Method2 () { }

      [DomainAspect]
      public void Method3 () { }
    }

    public class DomainAspectAttribute : AspectAttribute
    {
    }

    public class DomainNonAspectAttribute : Attribute
    {
    }
  }
}