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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.Old;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class ConstructorInitializationServiceTest
  {
    private MutableType _mutableType;
    private MutableConstructorInfo _firstConstructor;

    private IConstructorExpressionsHelperFactory _expressionsHelperFactoryStub;
    private IConstructorExpressionsHelper _expressionsHelperMock;
    private IFieldIntroducer2 _fieldIntroducer2Mock;

    private ConstructorInitializationService _service;

    [SetUp]
    public void SetUp ()
    {
      _mutableType = ObjectMother.GetMutableType (typeof (DomainType));

      _expressionsHelperFactoryStub = MockRepository.GenerateStub<IConstructorExpressionsHelperFactory>();
      _expressionsHelperMock = MockRepository.GenerateStrictMock<IConstructorExpressionsHelper>();
      _fieldIntroducer2Mock = MockRepository.GenerateStrictMock<IFieldIntroducer2>();
      _firstConstructor = _mutableType.AllMutableConstructors.First ();

      _expressionsHelperFactoryStub
          .Stub (x => x.CreateConstructorExpressionHelper (Arg<BodyContextBase>.Is.Anything))
          .Return (_expressionsHelperMock);

      _service = new ConstructorInitializationService (_fieldIntroducer2Mock, _expressionsHelperFactoryStub);
    }

    [Test]
    public void AddAspectInitialization ()
    {
      var aspectDescriptor1 = ObjectMother.GetStaticAspectDescriptor();
      var aspectDescriptor2 = ObjectMother.GetStaticAspectDescriptor();
      var aspectDescriptors = new[] { aspectDescriptor1, aspectDescriptor2 };
      var fakeField = ObjectMother.GetFieldWrapper ();
      var fakeExpression = Expression.Assign (Expression.Parameter (typeof (int)), Expression.Constant (1));

      _fieldIntroducer2Mock
          .Expect (x => x.AddField (_mutableType, typeof (AspectAttribute[]), "Aspects", FieldAttributes.Private | FieldAttributes.Static))
          .Return (fakeField);
      _expressionsHelperMock
          .Expect (x => x.CreateAspectAssignExpression (fakeField, aspectDescriptors))
          .Return (fakeExpression);
      var previousBody = _firstConstructor.Body;

      var result = _service.AddAspectInitialization (_mutableType, aspectDescriptors);

      _fieldIntroducer2Mock.VerifyAllExpectations ();
      _expressionsHelperMock.VerifyAllExpectations ();
      var expectedExpression = Expression.Block (typeof (void), Expression.Block (previousBody, fakeExpression));
      ExpressionTreeComparer.CheckAreEqualTrees (expectedExpression, _firstConstructor.Body);
      var expectedDictionary = new Dictionary<IAspectDescriptor, Tuple<IFieldWrapper, int>>()
                     {
                       { aspectDescriptor1, Tuple.Create(fakeField, 0) },
                       { aspectDescriptor2, Tuple.Create(fakeField, 1) }
                     };
      Assert.That (result, Is.EquivalentTo (expectedDictionary));
    }

    [Test]
    public void AddAspectInitialization_Instance ()
    {
      var aspectDescriptor1 = ObjectMother.GetInstanceAspectDescriptor();
      var aspectDescriptors = new[] { aspectDescriptor1 };
      var fakeExpression = Expression.Assign (Expression.Parameter (typeof (int)), Expression.Constant (1));

      _fieldIntroducer2Mock.Expect (x => x.AddField (_mutableType, typeof (AspectAttribute[]), "Aspects", FieldAttributes.Private));
      _expressionsHelperMock.Expect (x => x.CreateAspectAssignExpression (null, null)).IgnoreArguments().Return (fakeExpression);

      _service.AddAspectInitialization (_mutableType, aspectDescriptors);
    }

    [Test]
    public void AddDelegateInitialization ()
    {
      var method = _mutableType.AllMutableMethods.Single (x => x.Name == "Method");
      var fakeField = ObjectMother.GetFieldWrapper ();
      var fakeExpression = Expression.Assign (Expression.Parameter (typeof (int)), Expression.Constant (1));

      _fieldIntroducer2Mock
          .Expect (x => x.AddField (_mutableType, typeof(Action), "Delegate", FieldAttributes.Private | FieldAttributes.Static))
          .Return (fakeField);
      _expressionsHelperMock
          .Expect (x => x.CreateDelegateAssignExpression (fakeField, method))
          .Return (fakeExpression);
      var previousBody = _firstConstructor.Body;

      var result = _service.AddDelegateInitialization (method);

      Assert.That (result, Is.SameAs (fakeField));
      var expected = Expression.Block (typeof (void), Expression.Block (previousBody, fakeExpression));
      ExpressionTreeComparer.CheckAreEqualTrees (expected, _firstConstructor.Body);
    }

    [Test]
    public void AddMemberInfoInitialization_MethodInfo ()
    {
      var method = _mutableType.AllMutableMethods.Single (x => x.Name == "Method");

      TestAndCheckMemberInfoInitialization (method, method, typeof (MethodInfo), "Method");
    }

    [Test]
    public void AddMemberInfoInitialization_PropertyInfo ()
    {
      var method = _mutableType.AllMutableMethods.Single (x => x.Name == "get_Property");
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.Property);

      TestAndCheckMemberInfoInitialization (method, property, typeof (PropertyInfo), "get_Property");
    }

    private void TestAndCheckMemberInfoInitialization (MutableMethodInfo method, MemberInfo memberInfo, Type fieldType, string fieldName)
    {
      var fakeField = ObjectMother.GetFieldWrapper ();
      var fakeExpression = Expression.Assign (Expression.Parameter (typeof (int)), Expression.Constant (1));

      _fieldIntroducer2Mock
          .Expect (x => x.AddField (_mutableType, fieldType, fieldName, FieldAttributes.Private | FieldAttributes.Static))
          .Return (fakeField);
      _expressionsHelperMock
          .Expect (x => x.CreateMemberInfoAssignExpression (fakeField, memberInfo))
          .Return (fakeExpression);
      var previousBody = _firstConstructor.Body;

      var result = _service.AddMemberInfoInitialization (method);

      _fieldIntroducer2Mock.VerifyAllExpectations ();
      _expressionsHelperMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeField));
      var expected = Expression.Block (typeof (void), Expression.Block (previousBody, fakeExpression));
      ExpressionTreeComparer.CheckAreEqualTrees (expected, _firstConstructor.Body);
    }

    class DomainType
    {
      public virtual void Method () {}

      public virtual string Property { get; set; }

      public virtual event EventHandler Event;
    }
  }
}