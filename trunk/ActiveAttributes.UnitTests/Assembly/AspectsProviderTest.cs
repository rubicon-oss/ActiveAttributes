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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.UnitTests.Assembly;
using NUnit.Framework;
using Remotion.Utilities;
using Assert = NUnit.Framework.Assert;

//[assembly: AspectsProviderTest.AssemblyAttribute]

[assembly: AspectsProviderTest.DomainAspectAttribute (IfType = "*AspectsProviderTest+DomainType3")]
[assembly: AspectsProviderTest.DomainAspectAttribute (IfType = typeof (AspectsProviderTest.DomainType4))]
[assembly: AspectsProviderTest.DomainAspectAttribute (IfType = "ActiveAttributes.UnitTests.Assembly.AspectsProviderTest+NestedClass+*")]

[assembly: AspectsProviderTest.AssemblyLevelAspect (IfType = typeof(object))]

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
    public void GetAspects_AssemblyLevel_StringType ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType3 obj) => obj.Method ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Test]
    public void GetAspects_AssemblyLevel_StrongType ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType4 obj) => obj.Method ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Test]
    public void GetAspects_AssemblyLevel_Namespace ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((NestedClass.DomainType5 obj) => obj.Method ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (1));
    }

    [Test]
    public void GetAspects_AssemblyLevel_Namespace_EscapeDot ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((NestedClassX.DomainType5 obj) => obj.Method ()));

      var result = _provider.GetAspects (methodInfo).ToArray ();

      Assert.That (result, Has.Length.EqualTo (0));
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
    public void GetAssemblyLevelAspect ()
    {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly ();

      var result = _provider.GetAssemblyLevelAspects (assembly).ToArray();

      Assert.That (result, Has.Some.Property ("AspectType").EqualTo (typeof (AssemblyLevelAspect)));
    }

    public class AssemblyLevelAspect : AspectAttribute { }



    [Test]
    public void GetMethodLevelAspect ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod ((MethodLevelAspectClass obj) => obj.Method ());

      var result = _provider.GetMethodLevelAspects (method).ToArray();

      Assert.That (result, Has.All.Property ("AspectType").EqualTo (typeof (DomainAspectAttribute)));
    }

    public class MethodLevelAspectClass
    {
      [DomainAspect]
      public void Method () { }
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
      var method = MemberInfoFromExpressionUtility.GetMethod ((MethodLevelAspectClassWithNotInheritingAspect obj) => obj.Method ());

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

      var result = _provider.GetPropertyLevelAspects (property).ToArray ();

      Assert.That (result, Has.All.Property ("AspectType").EqualTo (typeof (DomainAspectAttribute)));
    }

    public class PropertyLevelAspectClass
    {
      [DomainAspect]
      public string Property { get; set; }
    }


    [Test]
    public void GetPropertyLevelAspectsRespectsInheritingAspect ()
    {
      var Property = MemberInfoFromExpressionUtility.GetProperty ((PropertyLevelAspectClassWithInheritingAspect obj) => obj.Property);

      var result = _provider.GetPropertyLevelAspects (Property).ToArray ();

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
      var Property = MemberInfoFromExpressionUtility.GetProperty ((PropertyLevelAspectClassWithNotInheritingAspect obj) => obj.Property);

      var result = _provider.GetPropertyLevelAspects (Property).ToArray ();

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
      var method2 = typeof (IDomainInterface).GetMethod ("Method", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

      var map = typeof (ClassWithInterfaceAspect).GetInterfaceMap (typeof (IDomainBase));


      var result = _provider.GetMethodLevelAspects (method).ToArray ();

      //var interfaces = method.DeclaringType.GetInterfaces();

      //Assert.That (interfaces, Has.Length.EqualTo (1));


      Assert.That (result, Has.Length.EqualTo (1));
    }


    public interface IDomainBase
    {
      void Method2 ();
    }

    public class Class21 : IDomainBase
    {
      public void Method2 ()
      {
        throw new NotImplementedException();
      }
    }

    public interface IDomainInterface
    {
      [DomainAspect]
      void Method ();

      string Property { get; set; }
    }

    public class ClassWithInterfaceAspect : Class21, IDomainInterface
    {
      public void Method ()
      {
      }

      public string Property
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }
    }
























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

    public class DomainAspectAttribute : Core.Aspects.AspectAttribute
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
    public class InheritingAspectAttribute : Core.Aspects.AspectAttribute
    {
    }

    [AttributeUsage (AttributeTargets.All, Inherited = false)]
    public class NotInheritingAspectAttribute : Core.Aspects.AspectAttribute
    {
    }

    [DomainAspect (IfSignature = "void Method*(*)")]
    public class DomainType2
    {
      public virtual void Method1 () { }

      public virtual void SkipMethod () { }
    }

    public class DomainType3
    {
      public virtual void Method () { }
    }

    public class DomainType4
    {
      public virtual void Method () { }
    }

    public class NestedClass
    {
      public class DomainType5
      {
        public virtual object Method ()
        {
          return null;
        }
      }
    }

    public class NestedClassX
    {
      public class DomainType5
      {
        public virtual object Method ()
        {
          return null;
        }
      }
    }
  }
}
