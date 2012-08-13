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
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ActiveAttributes.Core.Aspects;
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
    [Test]
    public void ApplyToType_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1());
      var aspect = new TestableAspectAttribute { ApplyToType = typeof (DomainClass) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void ApplyToType_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { ApplyToType = typeof (int) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void ApplyToType_MatchingSub ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { ApplyToType = typeof (object) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void ApplyToTypeNamePattern_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { ApplyToTypeNamePattern = "ActiveAttributes.UnitTests.Aspects" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void ApplyToTypeNamePattern_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { ApplyToTypeNamePattern = "ActiveAttributes.UnitTests2.Aspects" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void ApplyToTypeNamePattern_MatchingWildcard ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { ApplyToTypeNamePattern = "ActiveAttributes.UnitTests.*" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void MemberNameFilter_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { MemberNameFilter = "Method2" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void MemberNameFilter_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { MemberNameFilter = "Method2" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void MemberNameFilter_MatchingWildcard ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { MemberNameFilter = "*2" };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }


    [Test]
    public void MemberVisibilityFilter_PublicMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.PublicMethod ());
      var aspect = new TestableAspectAttribute { MemberVisibilityFilter = Visibility.Public };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void MemberVisibilityFilter_PublicNotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.InternalMethod ());
      var aspect = new TestableAspectAttribute { MemberVisibilityFilter = Visibility.Public };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void MemberCustomAttributeFilter_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { MemberCustomAttributeFilter = new[] { typeof (CompilerGeneratedAttribute) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void MemberCustomAttributeFilter_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method2 ());
      var aspect = new TestableAspectAttribute { MemberCustomAttributeFilter = new[] { typeof (CompilerGeneratedAttribute) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void MemberCustomAttributeFilter_MatchingMultiple ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { MemberCustomAttributeFilter = new[] { typeof (CompilerGeneratedAttribute), typeof (MethodImplAttribute) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void MemberCustomAttributeFilter_NotMatchingMultiple ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.Method1 ());
      var aspect = new TestableAspectAttribute { MemberCustomAttributeFilter = new[] { typeof (CompilerGeneratedAttribute), typeof (ObsoleteAttribute) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void MemberReturnTypeFilter_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.IntegerMethod ());
      var aspect = new TestableAspectAttribute { MemberReturnTypeFilter = typeof (int) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void MemberReturnTypeFilter_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.IntegerMethod ());
      var aspect = new TestableAspectAttribute { MemberReturnTypeFilter = typeof (double) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    [Test]
    public void MemberReturnTypeFilter_MatchingSub ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.IntegerMethod ());
      var aspect = new TestableAspectAttribute { MemberReturnTypeFilter = typeof (object) };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void MemberAttributeFilter_Matching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.MethodWithArg (1, ""));
      var aspect = new TestableAspectAttribute { MemberArgumentsFilter = new[] { typeof (int), typeof (string) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.True);
    }

    [Test]
    public void MemberAttributeFilter_NotMatching ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((DomainClass obj) => obj.MethodWithArg (1, ""));
      var aspect = new TestableAspectAttribute { MemberArgumentsFilter = new[] { typeof (int), typeof (int) } };
      var result = aspect.Matches (method);

      Assert.That (result, Is.False);
    }

    public class DomainClass
    {
      [CompilerGenerated]
      [MethodImpl]
      public void Method1 ()
      {
      }

      public void Method2 ()
      {
      }

      public void PublicMethod ()
      {
      }

      protected void ProtectedMethod ()
      {
      }

      private void PrivateMethod ()
      {
      }

      internal void InternalMethod ()
      {
      }

      public int IntegerMethod ()
      {
        return 1;
      }

      public void MethodWithArg (int arg1, string arg2)
      {
      }
    }

    public class TestableAspectAttribute : AspectAttribute
    {
    }
  }
}