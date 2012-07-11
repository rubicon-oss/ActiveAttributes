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
using Remotion.TypePipe.UnitTests.MutableReflection;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class FieldIntroducerTest
  {
    private FieldIntroducer _introducer;

    [SetUp]
    public void SetUp ()
    {
      _introducer = new FieldIntroducer();
    }

    [Test]
    public void Introduce_MethodInfoField ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

      var result = _introducer.Introduce (mutableMethod);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name == "_m_Method1_MethodInfo");

      Assert.That (fieldInfo, Is.EqualTo (result.MethodInfoField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (MethodInfo)));
    }

    [Test]
    public void Introduce_DelegateField ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

      var result = _introducer.Introduce (mutableMethod);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name == "_m_Method1_Delegate");

      Assert.That (fieldInfo, Is.EqualTo (result.DelegateField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (Action)));
    }

    [Test]
    public void Introduce_StaticAspectsField ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

      var result = _introducer.Introduce (mutableMethod);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name == "_s_Method1_StaticAspects");

      Assert.That (fieldInfo, Is.EqualTo (result.StaticAspectsField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Static | FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (AspectAttribute[])));
    }

    [Test]
    public void Introduce_InstanceAspectsField ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

      var result = _introducer.Introduce (mutableMethod);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name == "_m_Method1_InstanceAspects");

      Assert.That (fieldInfo, Is.EqualTo (result.InstanceAspectsField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (AspectAttribute[])));
    }

    [Test]
    public void Introduce_MethodOverloads ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo1 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var methodInfo2 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ("a")));
      var mutableMethod1 = mutableType.GetOrAddMutableMethod (methodInfo1);
      var mutableMethod2 = mutableType.GetOrAddMutableMethod (methodInfo2);

      _introducer.Introduce (mutableMethod1);
      var result = _introducer.Introduce (mutableMethod2);

      var methodInfoFieldInfo = mutableType.AddedFields.First (x => x.Name == "_m_Method2_MethodInfo");
      var delegateFieldInfo = mutableType.AddedFields.First (x => x.Name == "_m_Method2_Delegate");
      var staticAspectsFieldInfo = mutableType.AddedFields.First (x => x.Name == "_s_Method2_StaticAspects");
      var instanceAspectsFieldInfo = mutableType.AddedFields.First (x => x.Name == "_m_Method2_InstanceAspects");

      Assert.That (result.MethodInfoField, Is.EqualTo (methodInfoFieldInfo));
      Assert.That (result.DelegateField, Is.EqualTo (delegateFieldInfo));
      Assert.That (result.StaticAspectsField, Is.EqualTo (staticAspectsFieldInfo));
      Assert.That (result.InstanceAspectsField, Is.EqualTo (instanceAspectsFieldInfo));
    }

    [Test]
    public void IntroduceTypeLevelAspects_Instance ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var result = _introducer.IntroduceTypeLevelFields (mutableType);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name == "_m_TypeLevel_InstanceAspects");

      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo, Is.EqualTo (result.InstanceAspectsField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (AspectAttribute[])));
    }

    [Test]
    public void IntroduceTypeLevelAspects_Static ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var result = _introducer.IntroduceTypeLevelFields (mutableType);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name == "_s_TypeLevel_StaticAspects");

      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo, Is.EqualTo (result.StaticAspectsField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Static | FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (AspectAttribute[])));
    }

    public class DomainType
    {
      public void Method () { }
      public void Method (string a) { }
    }
  }
}