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

using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration;
using NUnit.Framework;
using Remotion.Utilities;

using Assert = NUnit.Framework.Assert;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectDescriptorTest
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
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method5()));
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
    public void DelegatesMatches1 ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method3 ());
      var customData = CustomAttributeData.GetCustomAttributes (method).Single ();
      var descriptor = new AspectDescriptor (customData);

      var result = descriptor.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void DelegatesMatches2 ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method4 ());
      var customData = CustomAttributeData.GetCustomAttributes (method).Single ();
      var descriptor = new AspectDescriptor (customData);

      var result = descriptor.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void ToString_ ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method1 ());
      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();
      var descriptor = new AspectDescriptor (customData);

      var result = descriptor.ToString ();

      Assert.That (result, Is.StringEnding ("AspectAttribute(Scope = Static)"));
    }

    [Test]
    public void ToStringWithArguments ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method6 ());
      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single ();
      var descriptor = new AspectDescriptor (customData);

      var result = descriptor.ToString ();

      Assert.That (result, Is.StringEnding ("AspectAttribute({test}, PropertyArgument = {test2}, Scope = Static)"));
    }

    public class DomainType
    {
      [Aspect]
      public void Method1 () { }

      [NonAspect]
      public void Method2 () { }

      [MatchingAspect]
      public void Method3 () { }

      [NotMatchingAspect]
      public void Method4 () { }

      [Aspect (Scope = AspectScope.Instance, Priority = 10)]
      public void Method5 () { }

      [Aspect("test", PropertyArgument = "test2")]
      public void Method6 () { }
    }
    
    public class AspectAttribute : Core.Aspects.AspectAttribute
    {
      public AspectAttribute () { }

      public AspectAttribute (string constructorArgument) { }

      public string PropertyArgument { get; set; }
    }

    public class NonAspectAttribute : Attribute { }

    public class MatchingAspectAttribute : AspectAttribute
    {
      public override bool Matches (MethodInfo methodInfo) { return true; }
    }

    public class NotMatchingAspectAttribute : AspectAttribute
    {
      public override bool Matches (MethodInfo methodInfo) { return false; }
    }
  }
}