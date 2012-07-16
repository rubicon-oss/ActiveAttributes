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
using ActiveAttributes.Core.Extensions;

using NUnit.Framework;
using Remotion.Utilities;

namespace ActiveAttributes.UnitTests.Extensions
{
  [TestFixture]
  public class CustomAttributeDataExtensionsTest
  {
    [Test]
    public void ConstructorElementArg ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ConstructorElementArg()));
      var customAttributeData = CustomAttributeData.GetCustomAttributes (methodInfo).Single();
      var attribute = (DomainAttribute) customAttributeData.CreateAttribute();

      Assert.That (attribute.ConstructorElementArg, Is.EqualTo ("a"));
    }

    [Test]
    public void ConstructorArrayArg ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ConstructorArrayArg()));
      var customAttributeData = CustomAttributeData.GetCustomAttributes (methodInfo).Single();
      var attribute = (DomainAttribute) customAttributeData.CreateAttribute();

      Assert.That (attribute.ConstructorArrayArg, Is.EquivalentTo (new[] { "a" }));
    }

    [Test]
    public void PropertyElementArg ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.PropertyElementArg()));
      var customAttributeData = CustomAttributeData.GetCustomAttributes (methodInfo).Single();
      var attribute = (DomainAttribute) customAttributeData.CreateAttribute();

      Assert.That (attribute.PropertyElementArg, Is.EqualTo ("a"));
    }

    [Test]
    public void PropertyArrayArg ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.PropertyArrayArg()));
      var customAttributeData = CustomAttributeData.GetCustomAttributes (methodInfo).Single();
      var attribute = (DomainAttribute) customAttributeData.CreateAttribute();

      Assert.That (attribute.PropertyArrayArg, Is.EquivalentTo (new[] { "a" }));
    }

    [Test]
    public void FieldElementArg ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.FieldElementArg()));
      var customAttributeData = CustomAttributeData.GetCustomAttributes (methodInfo).Single();
      var attribute = (DomainAttribute) customAttributeData.CreateAttribute();

      Assert.That (attribute.FieldElementArg, Is.EqualTo ("a"));
    }

    [Test]
    public void FieldArrayArg ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.FieldArrayArg()));
      var customAttributeData = CustomAttributeData.GetCustomAttributes (methodInfo).Single();
      var attribute = (DomainAttribute) customAttributeData.CreateAttribute();

      Assert.That (attribute.FieldArrayArg, Is.EquivalentTo (new[] { "a" }));
    }


    public class DomainType
    {
      [Domain ("a")]
      public void ConstructorElementArg () { }

      [Domain (new[] { "a" })]
      public void ConstructorArrayArg () { }

      [Domain (PropertyElementArg = "a")]
      public void PropertyElementArg () { }

      [Domain (PropertyArrayArg = new[] { "a" })]
      public void PropertyArrayArg () { }

      [Domain (FieldElementArg = "a")]
      public void FieldElementArg () { }

      [Domain (FieldArrayArg = new[] { "a" })]
      public void FieldArrayArg () { }
    }

    public class DomainAttribute : Attribute
    {
      public string FieldElementArg;
      public string[] FieldArrayArg;

      public DomainAttribute ()
      {  
      }

      public DomainAttribute (string constructorElementArg)
      {
        ConstructorElementArg = constructorElementArg;
      }

      public DomainAttribute (string[] constructorArrayArg)
      {
        ConstructorArrayArg = constructorArrayArg;
      }

      public string ConstructorElementArg { get; set; }
      public string[] ConstructorArrayArg { get; set; }

      public string PropertyElementArg { get; set; }
      public string[] PropertyArrayArg { get; set; }
    }
  }
}