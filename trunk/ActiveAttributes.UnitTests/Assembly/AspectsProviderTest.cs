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
using System.Text.RegularExpressions;

using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectsProviderTest
  {
    private AspectsProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new AspectsProvider();
    }

    [Test]
    public void GetAspects_OneElement ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result.Length, Is.EqualTo (1));
      Assert.That (result, Has.Some.Property ("AspectType").EqualTo (typeof (DomainAspectAttribute)));
    }

    [Test]
    public void GetAspects_MultipleElement ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.OtherMethod ()));

      var result = _provider.GetAspects (methodInfo).ToArray();

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result, Has.Some.Property ("Priority").EqualTo (5));
      Assert.That (result, Has.Some.Property ("Priority").EqualTo (10));
    }

    [Test]
    public void GetAspects_Derived_Inheriting ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DerivedType obj) => obj.Method1 ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Test]
    public void GetAspects_Derived_NonInheriting ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DerivedType obj) => obj.Method2 ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (0));
    }

    [Test]
    public void GetAspects_Base_NonInheriting ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((BaseType obj) => obj.Method2 ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Test]
    public void GetAspects_ApplyAspects_ClassLevel ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.Method1 ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Test]
    public void name ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
      var pattern = "Void .*";

      //var input = methodInfo.ToString();
      var input = "Void Method()";
      var regex = Regex.IsMatch (input, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
      Assert.That (regex, Is.True);
    }

    public class DomainType
    {
      [DomainAspect]
      public void Method () { }

      [DomainAspect (Priority = 5)]
      [DomainAspect (Priority = 10)]
      public void OtherMethod () { }
    }

    [UsedImplicitly]
    public class DomainAspectAttribute : AspectAttribute
    {
    }

    public class BaseType
    {
      [InheritingAspect]
      public virtual void Method1 () { }

      [NotInheritingAspect]
      public virtual void Method2 () { }
    }

    public class DerivedType : BaseType
    {
      public override void Method1 () { }

      public override void Method2 () { }
    }

    [UsedImplicitly]
    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    public class InheritingAspectAttribute : AspectAttribute
    {
    }

    [UsedImplicitly]
    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    public class NotInheritingAspectAttribute : AspectAttribute
    {
    }

    [ApplyAspect(typeof(DomainAspectAttribute))]
    public class DomainType2
    {
      public virtual void Method1 () { }
    }
  }
}