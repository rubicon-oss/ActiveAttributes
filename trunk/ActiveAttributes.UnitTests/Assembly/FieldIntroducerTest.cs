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
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
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
        Test ("_m_TypeLevel_InstanceAspects", data => data.InstanceAspectsField, FieldAttributes.Private);
      }

      [Test]
      public void AddsStaticAspectField ()
      {
        Test ("_s_TypeLevel_StaticAspects", data => data.StaticAspectsField, FieldAttributes.Static | FieldAttributes.Private);
      }

      private void Test (string fieldName, Func<FieldIntroducer.Data, FieldInfo> fieldSelector, FieldAttributes attributes)
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));

        var data = _introducer.IntroduceTypeAspectFields (mutableType);

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
        Test ("PropertyInfo", data => data.PropertyInfoField, typeof (PropertyInfo));
      }

      [Test]
      public void AddsEventInfoField ()
      {
        Test ("EventInfo", data => data.EventInfoField, typeof (EventInfo));
      }

      [Test]
      public void AddsMethodInfoField ()
      {
        Test ("MethodInfo", data => data.MethodInfoField, typeof (MethodInfo));
      }

      [Test]
      public void AddsDelegateField ()
      {
        Test ("Delegate", data => data.DelegateField, typeof (Action));
      }

      private void Test (string fieldNameEnd, Func<FieldIntroducer.Data, FieldInfo> fieldSelector, Type type)
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
        var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));

        var fieldData = _introducer.IntroduceMethodReflectionFields (mutableType, methodInfo);

        var returned = fieldSelector (fieldData);
        var queried = mutableType.AddedFields.Single (x => x.Name.EndsWith (fieldNameEnd));

        TestField (returned, queried, type, FieldAttributes.Private);
      }

      [Test]
      public void CanHandleOverloads ()
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
        var methodInfo1 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
        var methodInfo2 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ("")));

        var fieldData1 = _introducer.IntroduceMethodReflectionFields (mutableType, methodInfo1);
        var fieldData2 = _introducer.IntroduceMethodReflectionFields (mutableType, methodInfo2);

        Assert.That (fieldData1.PropertyInfoField, Is.Not.Null);
        Assert.That (fieldData1.EventInfoField, Is.Not.Null);
        Assert.That (fieldData1.MethodInfoField, Is.Not.Null);
        Assert.That (fieldData1.DelegateField, Is.Not.Null);
        Assert.That (fieldData2.PropertyInfoField, Is.Not.Null);
        Assert.That (fieldData2.EventInfoField, Is.Not.Null);
        Assert.That (fieldData2.MethodInfoField, Is.Not.Null);
        Assert.That (fieldData2.DelegateField, Is.Not.Null);
        Assert.That (fieldData1.PropertyInfoField, Is.Not.EqualTo (fieldData2.PropertyInfoField));
        Assert.That (fieldData1.EventInfoField, Is.Not.EqualTo (fieldData2.EventInfoField));
        Assert.That (fieldData1.MethodInfoField, Is.Not.EqualTo (fieldData2.MethodInfoField));
        Assert.That (fieldData1.DelegateField, Is.Not.EqualTo (fieldData2.DelegateField));
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

      private void Test (string fieldNameEnd, Func<FieldIntroducer.Data, FieldInfo> fieldSelector, FieldAttributes attributes)
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
        var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));

        var fieldData = _introducer.IntroduceMethodAspectFields (mutableType, methodInfo);

        var returned = fieldSelector (fieldData);
        var queried = mutableType.AddedFields.Single (x => x.Name.EndsWith (fieldNameEnd));

        TestField (returned, queried, typeof (AspectAttribute[]), attributes);
      }

      [Test]
      public void CanHandleOverloads ()
      {
        var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
        var methodInfo1 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
        var methodInfo2 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ("")));

        var fieldData1 = _introducer.IntroduceMethodAspectFields (mutableType, methodInfo1);
        var fieldData2 = _introducer.IntroduceMethodAspectFields (mutableType, methodInfo2);

        Assert.That (fieldData1.InstanceAspectsField, Is.Not.Null);
        Assert.That (fieldData1.StaticAspectsField, Is.Not.Null);
        Assert.That (fieldData2.InstanceAspectsField, Is.Not.Null);
        Assert.That (fieldData2.StaticAspectsField, Is.Not.Null);
        Assert.That (fieldData1.InstanceAspectsField, Is.Not.EqualTo (fieldData2.InstanceAspectsField));
        Assert.That (fieldData1.StaticAspectsField, Is.Not.EqualTo (fieldData2.StaticAspectsField));
      }
    }

    private class DomainType
    {
      public void Method () { }
      public void Method (string a) { }
    }
  }
}