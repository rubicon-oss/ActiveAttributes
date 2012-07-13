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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.Utilities;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectExpressionGeneratorBaseTest
  {
    private FieldInfo _dummyField;
    private Expression _elementExpression;
    private Expression _arrayExpression;

    [SetUp]
    public void SetUp ()
    {
      _dummyField = MemberInfoFromExpressionUtility.GetField (((DomainType obj) => obj.Dummy));
      _elementExpression = Expression.Convert (Expression.Constant ("a"), typeof (string));
      _arrayExpression = Expression.NewArrayInit (typeof (string), _elementExpression);      
    }

    [Test]
    public void GetInitExpression_ConstructorElementArg ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ConstructorElementArg ()));
      var customData = CustomAttributeData.GetCustomAttributes (method).First ();
      var descriptor = GetAspectAttributeDescriptorMock (customData);
      var generator = new AspectGenerator (_dummyField, 0, descriptor);

      var expected = Expression.MemberInit (
          Expression.New (
              customData.Constructor,
              _elementExpression));
      var actual = generator.GetInitExpression ();

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void GetInitExpression_ConstructorArrayArg ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.ConstructorArrayArg ()));
      var customData = CustomAttributeData.GetCustomAttributes (method).First ();
      var descriptor = GetAspectAttributeDescriptorMock (customData);
      var generator = new AspectGenerator (_dummyField, 0, descriptor);

      var expected = Expression.MemberInit (Expression.New (customData.Constructor, _arrayExpression));
      var actual = generator.GetInitExpression ();

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void GetInitExpression_PropertyElementArg ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.PropertyElementArg ()));
      var customData = CustomAttributeData.GetCustomAttributes (method).First ();
      var descriptor = GetAspectAttributeDescriptorMock (customData);
      var generator = new AspectGenerator (_dummyField, 0, descriptor);
      var member = MemberInfoFromExpressionUtility.GetProperty (((DomainAttribute obj) => obj.PropertyElementArg));

      var expected = Expression.MemberInit (
          Expression.New (customData.Constructor),
          Expression.Bind (member, _elementExpression));
      var actual = generator.GetInitExpression ();

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void GetInitExpression_PropertyArrayArg ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.PropertyArrayArg ()));
      var customData = CustomAttributeData.GetCustomAttributes (method).First ();
      var descriptor = GetAspectAttributeDescriptorMock (customData);
      var generator = new AspectGenerator (_dummyField, 0, descriptor);
      var member = MemberInfoFromExpressionUtility.GetProperty (((DomainAttribute obj) => obj.PropertyArrayArg));

      var expected = Expression.MemberInit (
          Expression.New (customData.Constructor),
          Expression.Bind (member, _arrayExpression));
      var actual = generator.GetInitExpression ();

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void GetInitExpression_FieldElementArg ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.FieldElementArg ()));
      var customData = CustomAttributeData.GetCustomAttributes (method).First ();
      var descriptor = GetAspectAttributeDescriptorMock (customData);
      var generator = new AspectGenerator (_dummyField, 0, descriptor);
      var member = MemberInfoFromExpressionUtility.GetField (((DomainAttribute obj) => obj.FieldElementArg));

      var expected = Expression.MemberInit (
          Expression.New (customData.Constructor),
          Expression.Bind (member, _elementExpression));
      var actual = generator.GetInitExpression ();

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void GetInitExpression_FieldArrayArg ()
    {
      var method = MemberInfoFromExpressionUtility.GetMethod (((DomainType obj) => obj.FieldArrayArg ()));
      var customData = CustomAttributeData.GetCustomAttributes (method).First ();
      var descriptor = GetAspectAttributeDescriptorMock (customData);
      var generator = new AspectGenerator (_dummyField, 0, descriptor);
      var member = MemberInfoFromExpressionUtility.GetField (((DomainAttribute obj) => obj.FieldArrayArg));

      var expected = Expression.MemberInit (
          Expression.New (customData.Constructor),
          Expression.Bind (member, _arrayExpression));
      var actual = generator.GetInitExpression ();

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    private static IAspectDescriptor GetAspectAttributeDescriptorMock (CustomAttributeData customData)
    {
      var descriptor = MockRepository.GenerateMock<IAspectDescriptor>();
      descriptor.Expect (x => x.ConstructorInfo).Return (customData.Constructor);
      descriptor.Expect (x => x.ConstructorArguments).Return (customData.ConstructorArguments);
      descriptor.Expect (x => x.NamedArguments).Return (customData.NamedArguments);
      return descriptor;
    }

    public class DomainType
    {
      public AspectAttribute[] Dummy;

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

      public DomainAttribute () { }

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

  public class AspectGenerator : AspectGeneratorBase
  {
      public AspectGenerator (FieldInfo fieldInfo, int index, IAspectDescriptor descriptor)
          : base(fieldInfo, index, descriptor)
      {
      }

      public override Expression GetStorageExpression (Expression thisExpression)
      {
        throw new NotImplementedException();
      }
    }
  }
}