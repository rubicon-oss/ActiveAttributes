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

using System;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe.UnitTests.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class FieldIntroducerTest
  {
    private static void TestField (FieldInfo returned, FieldInfo queried, Type type, FieldAttributes attributes)
    {
      Assert.That (returned, Is.Not.Null);
      Assert.That (returned, Is.EqualTo (queried));
      Assert.That (returned.Attributes, Is.EqualTo (attributes));
      Assert.That (returned.FieldType, Is.EqualTo (type));
    }

    public class IntroduceTypeAspectFields
    {
      private IFieldIntroducer _introducer;

      [SetUp]
      public void SetUp ()
      {
        _introducer = new FieldIntroducer ();
      }

      [Test]
      public void AddsInstanceAspectField ()
      {
        TestIntroduction ("_m_TypeLevel_InstanceAspects", data => data.InstanceAspectsField, FieldAttributes.Private);
      }

      [Test]
      public void AddsStaticAspectField ()
      {
        TestIntroduction ("_s_TypeLevel_StaticAspects", data => data.StaticAspectsField, FieldAttributes.Static | FieldAttributes.Private);
      }

      private void TestIntroduction (string fieldName, Func<FieldInfoContainer, FieldInfo> fieldSelector, FieldAttributes attributes)
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));

        var data = _introducer.IntroduceTypeFields (mutableType);

        var returned = fieldSelector (data);
        var queried = mutableType.AddedFields.First (x => x.Name == fieldName);

        TestField (returned, queried, typeof (AspectAttribute[]), attributes);
      }
    }

    public class IntroduceMethodReflectionFields
    {
      private IFieldIntroducer _introducer;

      [SetUp]
      public void SetUp ()
      {
        _introducer = new FieldIntroducer ();
      }

      [Test]
      public void AddsPropertyInfoField ()
      {
        TestIntroduction ("PropertyInfo", data => data.PropertyInfoField, typeof (PropertyInfo));
      }

      [Test]
      public void AddsEventInfoField ()
      {
        TestIntroduction ("EventInfo", data => data.EventInfoField, typeof (EventInfo));
      }

      [Test]
      public void AddsMethodInfoField ()
      {
        TestIntroduction ("MethodInfo", data => data.MethodInfoField, typeof (MethodInfo));
      }

      [Test]
      public void AddsDelegateField ()
      {
        TestIntroduction ("Delegate", data => data.DelegateField, typeof (Action));
      }

      private void TestIntroduction (string fieldNameEnd, Func<FieldInfoContainer, FieldInfo> fieldSelector, Type type)
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
        var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));

        var fields = _introducer.IntroduceMethodFields (mutableType, methodInfo);

        var returned = fieldSelector (fields);
        var queried = mutableType.AddedFields.Single (x => x.Name.EndsWith (fieldNameEnd));

        TestField (returned, queried, type, FieldAttributes.Private);
      }

      [Test]
      public void CanHandleOverloads ()
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
        var methodInfo1 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
        var methodInfo2 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ("")));

        var fields1 = _introducer.IntroduceMethodFields (mutableType, methodInfo1);
        var fields2 = _introducer.IntroduceMethodFields (mutableType, methodInfo2);

        Assert.That (fields1.PropertyInfoField, Is.Not.Null);
        Assert.That (fields1.EventInfoField, Is.Not.Null);
        Assert.That (fields1.MethodInfoField, Is.Not.Null);
        Assert.That (fields1.DelegateField, Is.Not.Null);
        Assert.That (fields2.PropertyInfoField, Is.Not.Null);
        Assert.That (fields2.EventInfoField, Is.Not.Null);
        Assert.That (fields2.MethodInfoField, Is.Not.Null);
        Assert.That (fields2.DelegateField, Is.Not.Null);
        Assert.That (fields1.PropertyInfoField, Is.Not.EqualTo (fields2.PropertyInfoField));
        Assert.That (fields1.EventInfoField, Is.Not.EqualTo (fields2.EventInfoField));
        Assert.That (fields1.MethodInfoField, Is.Not.EqualTo (fields2.MethodInfoField));
        Assert.That (fields1.DelegateField, Is.Not.EqualTo (fields2.DelegateField));
      }
    }

    public class IntroduceMethodAspectFields
    {
      private IFieldIntroducer _introducer;

      [SetUp]
      public void SetUp ()
      {
        _introducer = new FieldIntroducer ();
      }

      [Test]
      public void AddsInstanceAspectField ()
      {
        Test ("InstanceAspects", data => data.InstanceAspectsField, FieldAttributes.Private);
      }

      [Test]
      public void AddsStaticAspectField ()
      {
        Test ("StaticAspects", data => data.StaticAspectsField, FieldAttributes.Static | FieldAttributes.Private);
      }

      private void Test (string fieldNameEnd, Func<FieldInfoContainer, FieldInfo> fieldSelector, FieldAttributes attributes)
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
        var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));

        var fields = _introducer.IntroduceMethodFields (mutableType, methodInfo);

        var returned = fieldSelector (fields);
        var queried = mutableType.AddedFields.Single (x => x.Name.EndsWith (fieldNameEnd));

        TestField (returned, queried, typeof (AspectAttribute[]), attributes);
      }

      [Test]
      public void CanHandleOverloads ()
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
        var methodInfo1 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
        var methodInfo2 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ("")));

        var fields1 = _introducer.IntroduceMethodFields (mutableType, methodInfo1);
        var fields2 = _introducer.IntroduceMethodFields (mutableType, methodInfo2);

        Assert.That (fields1.InstanceAspectsField, Is.Not.Null);
        Assert.That (fields1.StaticAspectsField, Is.Not.Null);
        Assert.That (fields2.InstanceAspectsField, Is.Not.Null);
        Assert.That (fields2.StaticAspectsField, Is.Not.Null);
        Assert.That (fields1.InstanceAspectsField, Is.Not.EqualTo (fields2.InstanceAspectsField));
        Assert.That (fields1.StaticAspectsField, Is.Not.EqualTo (fields2.StaticAspectsField));
      }
    }

    private class DomainType
    {
      public void Method () { }
      public void Method (string a) { Dev.Null = a; }
    }
  }
}