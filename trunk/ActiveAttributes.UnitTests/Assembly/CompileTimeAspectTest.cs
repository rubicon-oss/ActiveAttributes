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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration;
using NUnit.Framework;
using Remotion.Utilities;
using FluentAssertions;
using Xunit;
using Assert = NUnit.Framework.Assert;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class CompileTimeAspectTest
  {
    [Test]
    public void Initialization ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method1 ()));

      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();

      var result = new AspectDescriptor (customData);

      Assert.That (result.Scope, Is.EqualTo (AspectScope.Static));
      Assert.That (result.Priority, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "CustomAttributeData must be from an AspectAttribute")]
    public void ThrowsExceptionForNonAspectAttributes ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method2 ()));

      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();

      new AspectDescriptor (customData);
    }

    [Test]
    public void Initialization_WithData ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method3()));

      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single();

      var result = new AspectDescriptor (customData);

      Assert.That (result.Scope, Is.EqualTo (AspectScope.Instance));
      Assert.That (result.Priority, Is.EqualTo (10));
      Assert.That (result.AspectType, Is.EqualTo (typeof (AspectAttribute)));
      Assert.That (result.ConstructorInfo, Is.EqualTo (customData.Constructor));
      Assert.That (result.ConstructorArguments, Is.EqualTo (customData.ConstructorArguments));
      Assert.That (result.NamedArguments, Is.EqualTo (customData.NamedArguments));
    }
    [Test]
    public void Matches_Signature ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method4 ()));
      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();
      var aspect = new AspectDescriptor (customData);

      var result = aspect.Matches (methodInfo);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Matches_SignatureNot ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method5 ()));
      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();
      var aspect = new AspectDescriptor (customData);

      var result = aspect.Matches (methodInfo);

      Assert.That (result, Is.False);
    }

    [Test]
    public void Matches_Type ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method6 ()));
      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();
      var aspect = new AspectDescriptor (customData);

      var result = aspect.Matches (methodInfo);

      Assert.That (result, Is.True);
    }

    [Test]
    public void Matches_TypeNot ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method7 ()));
      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();
      var aspect = new AspectDescriptor (customData);

      var result = aspect.Matches (methodInfo);

      Assert.That (result, Is.False);
    }

    public class DomainType
    {
      [Aspect]
      public void Method1 () { }

      [NonAspect]
      public void Method2 () { }

      [Aspect (Scope = AspectScope.Instance, Priority = 10)]
      public void Method3 () { }

      [Aspect (IfSignature = "void *()")]
      public void Method4 () { }

      [Aspect (IfSignature = "void *4()")]
      public void Method5 () { }

      [Aspect (IfType = typeof (DomainType))]
      public void Method6 () { }

      [Aspect (IfType = typeof (object))]
      public void Method7 () { }
    }

    public class AspectAttribute : Core.Aspects.AspectAttribute
    {
    }

    public class NonAspectAttribute : Attribute
    {
    }
  }
}