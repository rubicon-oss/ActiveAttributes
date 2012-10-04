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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Configuration;
using ActiveAttributes.UnitTests.Assembly;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectProviderTest
  {
    private AspectProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new AspectProvider();
    }

    public class AspectAttribute : Core.Aspects.AspectAttribute { }

    public class NonAspectAttribute : Attribute { }

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    public class NonInheritingAspectAttribute2 : Core.Aspects.AspectAttribute { }

    [AttributeUsage (AttributeTargets.All, Inherited = true)]
    public class InheritingAspect2Attribute : Core.Aspects.AspectAttribute { }


    [Test]
    public void GetTypeLevelAspects ()
    {
      var type = typeof (TypeLevelAspectClass);

      var result = _provider.GetTypeLevelAspects (type).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Aspect]
    public class TypeLevelAspectClass { }



    [Test]
    public void GetTypeLevelAspectsSkipsNonAspectAttributes ()
    {
      var type = typeof (TypeLevelAspectClassWithNonAspectAttribute);

      var result = _provider.GetTypeLevelAspects (type).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Aspect]
    [NonAspect]
    public class TypeLevelAspectClassWithNonAspectAttribute { }



    [Test]
    public void GetTypeLevelAspectsRespectsInheritingAspect ()
    {
      var type = typeof (TypeLevelAspectClassWithInheritingAspect);

      var result = _provider.GetTypeLevelAspects (type).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [InheritingAspect2]
    public class TypeLevelAspectClassWithInheritingAspectBase { }
    public class TypeLevelAspectClassWithInheritingAspect : TypeLevelAspectClassWithInheritingAspectBase { }



    [Test]
    public void GetTypeLevelAspectsRespectsNonInheritingAspect ()
    {
      var type = typeof (TypeLevelAspectClassWithoutAspectButBase);

      var result = _provider.GetTypeLevelAspects (type).ToArray ();

      Assert.That (result, Has.Length.EqualTo (0));
    }

    [NonInheritingAspectAttribute2]
    public class TypeLevelAspectClassWithNonInheritingAspectBase { }
    public class TypeLevelAspectClassWithoutAspectButBase : TypeLevelAspectClassWithNonInheritingAspectBase { }



    [Test]
    public void GetTypeLevelAspectWithInheritingOnSelf ()
    {
      var type = typeof (TypeLevelAspectClassWithInheritingAspectOnSelf);

      var result = _provider.GetTypeLevelAspects (type).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [InheritingAspect2]
    public class TypeLevelAspectClassWithInheritingAspectOnSelf { }



    [Test]
    public void GetMethodLevelAspect ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((MethodLevelAspectClass obj) => obj.Method ());

      var result = _provider.GetMethodLevelAspects (method).ToArray();

      Assert.That (result, Has.All.Property ("AspectType").EqualTo (typeof (AspectAttribute)));
    }

    public class MethodLevelAspectClass
    {
      [Aspect]
      public void Method () { }
    }


    [Test]
    public void GetParameterLevelAspects ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((ParameterLevelAspectClass obj) => obj.Method ("sad"));

      var result = _provider.GetParameterLevelAspects (method).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.Property ("AspectType").EqualTo (typeof (AppliedAspectAttribute)));
    }

    public class ParameterLevelAspectClass
    {
      public void Method ([Parameter] string arg) { }
    }

    [ApplyAspect(typeof(AppliedAspectAttribute))]
    public class ParameterAttribute : Attribute
    {
    }

    public class AppliedAspectAttribute : AspectAttribute
    {
    }



    [Test]
    public void GetParameterLevelAspectsOnlyOnce ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((OnlyOnceParameterLevelAspectClass obj) => obj.Method ("sad", "sat"));

      var result = _provider.GetParameterLevelAspects (method).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result, Has.All.Property ("AspectType").EqualTo (typeof (AppliedAspectAttribute)));
    }

    public class OnlyOnceParameterLevelAspectClass
    {
      public void Method ([ParameterAttribute] string arg1, [ParameterAttribute] string arg2) { }
    }


    [Test]
    public void GetMethodLevelAspectsRespectsInheritingAspect ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((MethodLevelAspectClassWithInheritingAspect obj) => obj.Method ());

      var result = _provider.GetMethodLevelAspects (method).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    public abstract class MethodLevelAspectClassWithInheritingAspectBase
    {
      [InheritingAspect2]
      public virtual void Method () { }
    }
    public class MethodLevelAspectClassWithInheritingAspect : MethodLevelAspectClassWithInheritingAspectBase
    {
      public override void Method () { }
    }

    [Test]
    public void GetMethodLevelAspectsRespectsNotInheritingAspect ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((MethodLevelAspectClassWithNotInheritingAspect obj) => obj.Method ());

      var result = _provider.GetMethodLevelAspects (method).ToArray ();

      Assert.That (result, Has.Length.EqualTo (0));
    }

    public abstract class MethodLevelAspectClassWithNotInheritingAspectBase
    {
      [NotInheritingAspect]
      public virtual void Method () { }
    }
    public class MethodLevelAspectClassWithNotInheritingAspect : MethodLevelAspectClassWithNotInheritingAspectBase
    {
      public override void Method () { }
    }


    [Test]
    public void GetPropertyLevelAspect ()
    {
      var property = MemberInfoFromExpressionUtility.GetProperty ((PropertyLevelAspectClass obj) => obj.Property);
      var method = property.GetGetMethod();

      var result = _provider.GetPropertyLevelAspects (method).ToArray ();

      Assert.That (result, Has.All.Property ("AspectType").EqualTo (typeof (AspectAttribute)));
    }

    public class PropertyLevelAspectClass
    {
      [Aspect]
      public string Property { get; set; }
    }


    [Test]
    public void GetPropertyLevelAspectsRespectsInheritingAspect ()
    {
      var property = MemberInfoFromExpressionUtility.GetProperty ((PropertyLevelAspectClassWithInheritingAspect obj) => obj.Property);
      var method = property.GetGetMethod ();

      var result = _provider.GetPropertyLevelAspects (method).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    public abstract class PropertyLevelAspectClassWithInheritingAspectBase
    {
      [InheritingAspect2]
      public virtual string Property { get; set; }
    }
    public class PropertyLevelAspectClassWithInheritingAspect : PropertyLevelAspectClassWithInheritingAspectBase
    {
      public override string Property { get; set; }
    }


    [Test]
    public void GetPropertyLevelAspectsRespectsNotInheritingAspect ()
    {
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((PropertyLevelAspectClassWithNotInheritingAspect obj) => obj.Property);
      var method = property.GetGetMethod ();

      var result = _provider.GetPropertyLevelAspects (method).ToArray ();

      Assert.That (result, Has.Length.EqualTo (0));
    }

    public abstract class PropertyLevelAspectClassWithNotInheritingAspectBase
    {
      [NotInheritingAspect]
      public virtual string Property { get; set; }
    }
    public class PropertyLevelAspectClassWithNotInheritingAspect : PropertyLevelAspectClassWithNotInheritingAspectBase
    {
      public override string Property { get; set; }
    }


    [Test]
    public void GetMethodLevelAspectsIncludingInterfaces ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((ClassWithInterfaceAspect obj) => obj.Method ());

      var result = _provider.GetInterfaceLevelAspects (method).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    public interface IDomainInterface
    {
      [Aspect]
      void Method ();
    }
    public class ClassWithInterfaceAspect : IDomainInterface
    {
      public void Method ()
      {
      }
    }

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    public class NotInheritingAspectAttribute : Core.Aspects.AspectAttribute
    {
    }

    public class DomainType4
    {
      public virtual void Method () { }
    }
  }
}
