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
    public void IntroduceMethodLevelFields_MethodInfoField ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

      var result = _introducer.IntroduceMethodLevelFields (mutableMethod);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name.EndsWith ("MethodInfo"));

      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo, Is.EqualTo (result.MethodInfoField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (MethodInfo)));
    }

    [Test]
    public void IntroduceMethodLevelFields_DelegateField ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

      var result = _introducer.IntroduceMethodLevelFields (mutableMethod);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name.EndsWith ("Delegate"));

      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo, Is.EqualTo (result.DelegateField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (Action)));
    }

    [Test]
    public void IntroduceMethodLevelFields_StaticAspectsField ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

      var result = _introducer.IntroduceMethodLevelFields (mutableMethod);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name.EndsWith ("StaticAspects"));

      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo, Is.EqualTo (result.StaticAspectsField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Static | FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (AspectAttribute[])));
    }

    [Test]
    public void IntroduceMethodLevelFields_InstanceAspectsField ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var mutableMethod = mutableType.GetOrAddMutableMethod (methodInfo);

      var result = _introducer.IntroduceMethodLevelFields (mutableMethod);

      var fieldInfo = mutableType.AddedFields.First (x => x.Name.EndsWith ("InstanceAspects"));

      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo, Is.EqualTo (result.InstanceAspectsField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (AspectAttribute[])));
    }

    [Test]
    public void IntroduceMethodLevelFields_MethodOverloads ()
    {
      var mutableType = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var methodInfo1 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ()));
      var methodInfo2 = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.Method ("a")));
      var mutableMethod1 = mutableType.GetOrAddMutableMethod (methodInfo1);
      var mutableMethod2 = mutableType.GetOrAddMutableMethod (methodInfo2);

      var result1 = _introducer.IntroduceMethodLevelFields (mutableMethod1);
      var result2 = _introducer.IntroduceMethodLevelFields (mutableMethod2);

      Assert.That (result1.MethodInfoField, Is.Not.EqualTo (result2.MethodInfoField));
      Assert.That (result1.DelegateField, Is.Not.EqualTo (result2.DelegateField));
      Assert.That (result1.StaticAspectsField, Is.Not.EqualTo (result2.StaticAspectsField));
      Assert.That (result1.InstanceAspectsField, Is.Not.EqualTo (result2.InstanceAspectsField));
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

    [Test]
    public void IntroduceAssemblyLevelAspects_Instance ()
    {
      var mutableAssembly = MutableTypeObjectMother.CreateForExistingType (typeof (DomainType));
      var result = _introducer.IntroduceAssemblyLevelFields (mutableAssembly);

      var fieldInfo = mutableAssembly.AddedFields.First (x => x.Name == "_m_AssemblyLevel_InstanceAspects");

      Assert.That (fieldInfo, Is.Not.Null);
      Assert.That (fieldInfo, Is.EqualTo (result.InstanceAspectsField));
      Assert.That (fieldInfo.Attributes, Is.EqualTo (FieldAttributes.Private));
      Assert.That (fieldInfo.FieldType, Is.EqualTo (typeof (AspectAttribute[])));
    }


    public class DomainType
    {
      public void Method () { }
      public void Method (string a) { }
    }
  }
}