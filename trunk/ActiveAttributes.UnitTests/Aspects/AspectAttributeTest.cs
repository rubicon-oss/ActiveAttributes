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
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.Core.Configuration;
using ActiveAttributes.Core.Extensions;
using ActiveAttributes.Core.Invocations;
using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Aspects
{
  [TestFixture]
  public class AspectAttributeTest
  {
    //[Test]
    //public void ApplyToType_Matching ()
    //{
    //  var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1());
    //  var aspect = new TestableAspectAttribute { ApplyToType = typeof (DomainClass) };
    //  var result = aspect.Matches (method);

    //  Assert.That (result, Is.True);
    //}

    //[Test]
    //public void ApplyToType_NotMatching ()
    //{
    //  var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
    //  var aspect = new TestableAspectAttribute { ApplyToType = typeof (int) };
    //  var result = aspect.Matches (method);

    //  Assert.That (result, Is.False);
    //}

    //[Test]
    //public void ApplyToType_MatchingSub ()
    //{
    //  var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
    //  var aspect = new TestableAspectAttribute { ApplyToType = typeof (object) };
    //  var result = aspect.Matches (method);

    //  Assert.That (result, Is.True);
    //}

    //[Test]
    //public void ApplyToTypeNamePattern_Matching ()
    //{
    //  var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method2 ());
    //  var aspect = new TestableAspectAttribute { ApplyToTypeNamePattern = "ActiveAttributes.UnitTests.Aspects" };
    //  var result = aspect.Matches (method);

    //  Assert.That (result, Is.True);
    //}

    //[Test]
    //public void ApplyToTypeNamePattern_NotMatching ()
    //{
    //  var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
    //  var aspect = new TestableAspectAttribute { ApplyToTypeNamePattern = "ActiveAttributes.UnitTests2.Aspects" };
    //  var result = aspect.Matches (method);
    //  Assert.That (result, Is.False);
    //}
    //[Test]
    //public void ApplyToTypeNamePattern_MatchingWildcard ()
    //{
    //  var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method2 ());
    //  var aspect = new TestableAspectAttribute { ApplyToTypeNamePattern = "ActiveAttributes.UnitTests.*" };
    //  var result = aspect.Matches (method);
    //  Assert.That (result, Is.True);
    //}

    private class AspectAttribute : Core.Aspects.AspectAttribute
    {
    }

    public class Matches_MemberNameFilter
    {
      [Test]
      public void Matches ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var aspect = new AspectAttribute { MemberNameFilter = "Matches" };

        Assert.That (aspect.Matches ((MethodInfo) method), Is.True);
      }
      [Test]
      public void MatchesNot ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var aspect = new AspectAttribute { MemberNameFilter = "AnyMethod" };

        Assert.That (aspect.Matches ((MethodInfo) method), Is.False);
      }
      [Test]
      public void MatchesWildcard ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var aspect = new AspectAttribute { MemberNameFilter = "Matches*card" };

        Assert.That (aspect.Matches ((MethodInfo) method), Is.True);
      }
    }

    public class Matches_MemberVisibilityFilter
    {
      private void PrivateMethod () { }
      internal void InternalMethod () { }
      protected void ProtectedMethod () { }

      [Test]
      public void MatchesPublic ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var aspect = new AspectAttribute { MemberVisibilityFilter = Visibility.Public };

        Assert.That (aspect.Matches ((MethodInfo) method), Is.True);
      }

      [Test]
      public void DoesntMatchPublic ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var aspect = new AspectAttribute { MemberVisibilityFilter = Visibility.All & ~Visibility.Public };

        Assert.That (aspect.Matches ((MethodInfo) method), Is.False);
      }

      [Test]
      public void MatchesPrivate ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => PrivateMethod ());
        var aspect = new AspectAttribute { MemberVisibilityFilter = Visibility.Private };

        Assert.That (aspect.Matches (method), Is.True);
      }

      [Test]
      public void DoesntMatchPrivate ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => PrivateMethod ());
        var aspect = new AspectAttribute { MemberVisibilityFilter = Visibility.All & ~Visibility.Private };

        Assert.That (aspect.Matches (method), Is.False);
      }

      [Test]
      public void REVIEW ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => PrivateMethod ());
        var aspect = new AspectAttribute { MemberVisibilityFilter = Visibility.All & ~Visibility.Public };

        Assert.That (aspect.Matches (method), Is.False);
      }

      [Test]
      public void MatchesInternal ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => InternalMethod ());
        var aspect = new AspectAttribute { MemberVisibilityFilter = Visibility.Assembly };

        Assert.That (aspect.Matches (method), Is.True);
      }

      [Test]
      public void DoesntMatchInternal ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => InternalMethod ());
        var aspect = new AspectAttribute { MemberVisibilityFilter = Visibility.All & ~Visibility.Assembly };

        Assert.That (aspect.Matches (method), Is.False);
      }

      [Test]
      public void MatchesProtected ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => ProtectedMethod ());
        var aspect = new AspectAttribute { MemberVisibilityFilter = Visibility.Family };

        Assert.That (aspect.Matches (method), Is.True);
      }

      [Test]
      public void DoesntMatchProtected ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => ProtectedMethod ());
        var aspect = new AspectAttribute { MemberVisibilityFilter = Visibility.All & ~Visibility.Family };

        Assert.That (aspect.Matches (method), Is.False);
      }
    }

    public class Matches_CustomAttributeFilter
    {
      private class Domain1Attribute : Attribute { }
      private class Domain2Attribute : Attribute { }

      [Test]
      [Domain1]
      public void Matches ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var aspect = new AspectAttribute { MemberCustomAttributeFilter = new[] { typeof (Domain1Attribute) } };

        Assert.That (aspect.Matches ((MethodInfo) method), Is.True);
      }

      [Test]
      [Domain1]
      public void DoesntMatch ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var aspect = new AspectAttribute { MemberCustomAttributeFilter = new[] { typeof (Domain2Attribute) } };

        Assert.That (aspect.Matches ((MethodInfo) method), Is.False);
      }

      [Test]
      [Domain1, Domain2]
      public void MatchesMultiple ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var aspect = new AspectAttribute { MemberCustomAttributeFilter = new[] { typeof (Domain1Attribute), typeof (Domain2Attribute) } };

        Assert.That (aspect.Matches ((MethodInfo) method), Is.True);
      }

      [Test]
      [Domain1]
      public void DoesntMatchMultiple ()
      {
        var method = MethodInfo.GetCurrentMethod ();
        var aspect = new AspectAttribute { MemberCustomAttributeFilter = new[] { typeof (Domain1Attribute), typeof (Domain2Attribute) } };

        Assert.That (aspect.Matches ((MethodInfo) method), Is.False);
      }
    }

    public class Matches_ReturnTypeFilter
    {
      private string StringMethod () { return ""; }

      [Test]
      public void Matches ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => StringMethod());
        var aspect = new AspectAttribute { MemberReturnTypeFilter = typeof (string) };

        Assert.That (aspect.Matches (method), Is.True);
      }

      [Test]
      public void DoesntMatch ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => StringMethod());
        var aspect = new AspectAttribute { MemberReturnTypeFilter = typeof (int) };

        Assert.That (aspect.Matches (method), Is.False);
      }

      [Test]
      public void MatchesBaseType ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => StringMethod());
        var aspect = new AspectAttribute { MemberReturnTypeFilter = typeof (object) };

        Assert.That (aspect.Matches (method), Is.True);
      }
    }

    public class Matches_MemberAttributeFilter
    {
      private static void StaticMethod () { }

      [Test]
      public void MatchesOverridable ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => ToString ());
        var aspect = new AspectAttribute { MemberFlagsFilter = MemberFlags.Overridable };

        Assert.That (aspect.Matches (method), Is.True);
      }

      [Test]
      public void DoesntMatchOverridable ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => StaticMethod ());
        var aspect = new AspectAttribute { MemberFlagsFilter = MemberFlags.Overridable };

        Assert.That (aspect.Matches (method), Is.False);
      }
    }

    public class Matches_MemberArgumentsFilter
    {
      private void MethodWithArgs1 (int arg0, string arg1) { }

      [Test]
      public void Matches ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => MethodWithArgs1 (1, ""));
        var aspect = new AspectAttribute { MemberArgumentsFilter = new[] { typeof (int), typeof (string) } };

        Assert.That (aspect.Matches (method), Is.True);
      }

      [Test]
      public void DoesntMatch ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => MethodWithArgs1 (1, ""));
        var aspect = new AspectAttribute { MemberArgumentsFilter = new[] { typeof (string), typeof (string) } };

        Assert.That (aspect.Matches (method), Is.False);
      }

      [Test]
      public void MatchesBaseType ()
      {
        var method = MemberInfoFromExpressionUtility.GetMethod (() => MethodWithArgs1 (1, ""));
        var aspect = new AspectAttribute { MemberArgumentsFilter = new[] { typeof (object), typeof (string) } };

        Assert.That (aspect.Matches (method), Is.True);
      }
    }
  }
}