//// Copyright (c) rubicon IT GmbH, www.rubicon.eu
////
//// See the NOTICE file distributed with this work for additional information
//// regarding copyright ownership.  rubicon licenses this file to you under 
//// the Apache License, Version 2.0 (the "License"); you may not use this 
//// file except in compliance with the License.  You may obtain a copy of the 
//// License at
////
////   http://www.apache.org/licenses/LICENSE-2.0
////
//// Unless required by applicable law or agreed to in writing, software 
//// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
//// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
//// License for the specific language governing permissions and limitations
//// under the License.
// TODO UNIT TEST
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using ActiveAttributes.Core.Assembly;
//using ActiveAttributes.Core.Assembly.Configuration;
//using ActiveAttributes.Core.Assembly.Descriptors;
//using NUnit.Framework;
//using Remotion.Utilities;

//namespace ActiveAttributes.UnitTests.Assembly.Descriptors
//{
//  [TestFixture]
//  public class CustomDataDescriptorTest
//  {
//    private static IAspectDescriptor GetFromMethod<T> (MethodBase methodInfo)
//    {
//      var customData = CustomAttributeData.GetCustomAttributes (methodInfo).Single (x => x.Constructor.DeclaringType == typeof (T));
//      return new CustomDataDescriptor (customData);
//    }

//    public class Initialize
//    {
//      [Test]
//      [Aspect]
//      public void Normal ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var descriptor = GetFromMethod<AspectAttribute> (method);

//        Assert.That (descriptor.Scope, Is.EqualTo (AspectScope.Static));
//        Assert.That (descriptor.Priority, Is.EqualTo (0));
//      }

//      [Test]
//      [ExpectedException (typeof (ArgumentException), ExpectedMessage = "CustomAttributeData must be from an AspectAttribute")]
//      [NonAspect]
//      public void ThrowsExceptionForNonAspectAttributes ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        GetFromMethod<NonAspectAttribute> (method);
//      }

//      [Test]
//      [Aspect (Scope = AspectScope.Instance, Priority = 5)]
//      public void SetsScopeAndPriority ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var descriptor = GetFromMethod<AspectAttribute> (method);

//        Assert.That (descriptor.Scope, Is.EqualTo (AspectScope.Instance));
//        Assert.That (descriptor.Priority, Is.EqualTo (5));
//      }

//      [Test]
//      [Aspect ("test", PropertyArgument = "test2")]
//      public void SetsCustomDataInformation ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var customData = CustomAttributeData.GetCustomAttributes (method).Single (x => x.Constructor.DeclaringType == typeof (AspectAttribute));
//        var descriptor = new CustomDataDescriptor (customData);

//        Assert.That (descriptor.ConstructorInfo, Is.EqualTo (customData.Constructor));
//        Assert.That (descriptor.ConstructorArguments, Is.EqualTo (customData.ConstructorArguments));
//        Assert.That (descriptor.NamedArguments, Is.EqualTo (customData.NamedArguments));
//      }
//    }

//    public class ToString_
//    {
//      [Test]
//      [Aspect]
//      public void PrintsTypeWithScopeAndPriority ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var descriptor = GetFromMethod<AspectAttribute> (method);

//        var result = descriptor.ToString ();
//        Assert.That (result, Is.EqualTo ("AspectAttribute(Scope = Static, Priority = 0)"));
//      }

//      [Test]
//      [Aspect ("muh")]
//      public void PrintsArguments ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var descriptor = GetFromMethod<AspectAttribute> (method);

//        var result = descriptor.ToString ();
//        Assert.That (result, Is.StringContaining ("Attribute(\"muh\", Scope"));
//      }

//      [Test]
//      [Aspect (PropertyArgument = "muh", MemberNameFilter = "_")]
//      public void PrintsNamedArguments ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var descriptor = GetFromMethod<AspectAttribute> (method);

//        var result = descriptor.ToString ();
//        Assert.That (result, Is.StringContaining ("Attribute(PropertyArgument = \"muh\", MemberNameFilter = \"_\", Scope"));
//      }

//      [Test]
//      [Aspect (Scope = AspectScope.Instance, Priority = 5)]
//      public void DoesntPrintPriorityOrScopeTwice ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var descriptor = GetFromMethod<AspectAttribute> (method);

//        var result = descriptor.ToString ();
//        Assert.That (result, Is.EqualTo ("AspectAttribute(Scope = Instance, Priority = 5)"));
//      }
//    }

//    public class Matches_
//    {
//      [Test]
//      [Aspect]
//      public void DelegatesToAttribute ()
//      {
//        var method = MethodInfo.GetCurrentMethod ();
//        var descriptor = GetFromMethod<AspectAttribute> (method);
//        descriptor.Matches ((MethodInfo) method);

//        Assert.That (AspectAttribute.MatchesMethod, Is.EqualTo (method));
//      }
//    }

//    private class AspectAttribute : Core.Aspects.AspectAttribute
//    {
//      public static MethodInfo MatchesMethod { get; private set; }

//      public AspectAttribute () { }

//      public AspectAttribute (string constructorArgument) { }

//      public string PropertyArgument { get; set; }

//      public override bool Matches (MethodInfo methodInfo)
//      {
//        MatchesMethod = methodInfo;
//        return true;
//      }
//    }

//    private class NonAspectAttribute : Attribute { }
//  }
//}