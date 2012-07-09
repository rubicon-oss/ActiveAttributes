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
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.UnitTests.Assembly;
using NUnit.Framework;
using Remotion.Utilities;

[assembly: AspectsProviderTest.DomainAspectAttribute (IfType = "*.AspectsProviderTest.DomainType2", IfSignature = "* AssemblyMethod()")]

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
    public void GetAspects_Derived_Inheriting_Property ()
    {
      var methodInfo = typeof (DerivedType).GetMethods ().Where (x => x.Name == "get_Property1").First ();

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }


    [Test]
    public void GetAspects_Derived_NotInheriting_Property ()
    {
      var methodInfo = typeof (DerivedType).GetMethods ().Where (x => x.Name == "get_Property2").First ();

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
    public void GetAspects_OnlyAspectAttributes ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.AnotherMethod()));

      var result = _provider.GetAspects (methodInfo);

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetAspects_FromProperties ()
    {
      var methodInfo = typeof (DomainType).GetMethods().Where (x => x.Name == "get_Property").First();

      var result = _provider.GetAspects (methodInfo).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
    }


    [Test]
    public void GetAspects_IfSignature_Match ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.Method1 ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Test]
    public void GetAspects_IfSignature_NoMatch ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.SkipMethod ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (0));
    }

    [Test]
    public void GetAspects_AssemblyLevel ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType2 obj) => obj.AssemblyMethod ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }


    // TODO
    //[Test]
    //public void name ()
    //{
    //  var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
    //  var pattern = "Void .*";

    //  //var input = methodInfo.ToString();
    //  var input = "Void Method()";
    //  var regex = Regex.IsMatch (input, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
    //  Assert.That (regex, Is.True);
    //}

    public class DomainType
    {
      [DomainAspect]
      public void Method () { }

      [DomainAspect (Priority = 5)]
      [DomainAspect (Priority = 10)]
      public void OtherMethod () { }

      [Dummy]
      public void AnotherMethod () { }

      [DomainAspect]
      public string Property { get; set; }
    }

    public class DomainAspectAttribute : AspectAttribute
    {
    }

    public class DummyAttribute : Attribute
    {
    }

    public class BaseType
    {
      [InheritingAspect]
      public virtual void Method1 () { }

      [NotInheritingAspect]
      public virtual void Method2 () { }

      [InheritingAspect]
      public virtual string Property1 { get; set; }

      [NotInheritingAspect]
      public virtual string Property2 { get; set; }
    }

    public class DerivedType : BaseType
    {
      public override void Method1 () { }

      public override void Method2 () { }

      public override string Property1 { get; set; }

      public override string Property2 { get; set; }
    }

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    public class InheritingAspectAttribute : AspectAttribute
    {
    }

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    public class NotInheritingAspectAttribute : AspectAttribute
    {
    }

    [DomainAspect (IfSignature = "void Method*(*)")]
    public class DomainType2
    {
      public virtual void Method1 () { }

      public virtual void SkipMethod () { }

      public virtual void AssemblyMethod () { }
    }
  }
}
