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
using System.Reflection;
using ActiveAttributes.Core.Aspects;
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Configuration2;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.UnitTests.Expressions;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class ExpressionGeneratorContainerTest
  {
    private AspectAttribute[] _array;

    private FieldInfo _field;

    private IAspectDescriptor _instanceAspectDescriptorMock;
    private IAspectDescriptor _staticAspectDescriptorMock;
    private IFieldWrapper _instanceFieldWrapperMock;
    private IFieldWrapper _staticFieldWrapperMock;

    private IEnumerable<IAspectDescriptor> _aspectDescriptors;
      
    [SetUp]
    public void SetUp ()
    {
      _field = NormalizingMemberInfoFromExpressionUtility.GetField (() => _array);

      _instanceAspectDescriptorMock = MockRepository.GenerateMock<IAspectDescriptor>();
      _staticAspectDescriptorMock = MockRepository.GenerateMock<IAspectDescriptor>();
      _instanceFieldWrapperMock = MockRepository.GenerateMock<IFieldWrapper>();
      _staticFieldWrapperMock = MockRepository.GenerateMock<IFieldWrapper>();

      _aspectDescriptors = new[] { _instanceAspectDescriptorMock, _staticAspectDescriptorMock };

      _instanceAspectDescriptorMock
          .Expect (x => x.Scope)
          .Return (Scope.Instance);
      _staticAspectDescriptorMock
          .Expect (x => x.Scope)
          .Return (Scope.Static);
    }

    [Test]
    public void Initialization ()
    {
      var container = new ExpressionGeneratorContainer (_aspectDescriptors, _instanceFieldWrapperMock, _staticFieldWrapperMock);

      Assert.That (container.InstanceAspects, Is.EquivalentTo (new[] { _instanceAspectDescriptorMock }));
      Assert.That (container.StaticAspects, Is.EquivalentTo (new[] { _staticAspectDescriptorMock }));
    }

    [Test]
    public void AspectStorageInfo_InstanceAspects ()
    {
      var instanceAspectDescriptor2 = MockRepository.GenerateMock<IAspectDescriptor>();
      instanceAspectDescriptor2
          .Expect (x => x.Scope)
          .Return (Scope.Instance);
      var aspectDescriptors = new[] { _instanceAspectDescriptorMock, instanceAspectDescriptor2 };

      var container = new ExpressionGeneratorContainer (aspectDescriptors, _instanceFieldWrapperMock, _staticFieldWrapperMock);

      var actual1 = container.AspectStorageInfo[_instanceAspectDescriptorMock];
      var actual2 = container.AspectStorageInfo[instanceAspectDescriptor2];

      var expected1 = Tuple.Create (_instanceFieldWrapperMock, 0);
      var expected2 = Tuple.Create (_instanceFieldWrapperMock, 1);

      Assert.That (actual1, Is.EqualTo (expected1));
      Assert.That (actual2, Is.EqualTo (expected2));
    }

    [Test]
    public void AspectStorageInfo_StaticAspects ()
    {
      var staticAspectDescriptor2 = MockRepository.GenerateMock<IAspectDescriptor>();
      staticAspectDescriptor2
          .Expect (x => x.Scope)
          .Return (Scope.Static);
      var aspectDescriptors = new[] { _staticAspectDescriptorMock, staticAspectDescriptor2 };

      var container = new ExpressionGeneratorContainer (aspectDescriptors, _instanceFieldWrapperMock, _staticFieldWrapperMock);

      var actual1 = container.AspectStorageInfo[_staticAspectDescriptorMock];
      var actual2 = container.AspectStorageInfo[staticAspectDescriptor2];

      var expected1 = Tuple.Create (_staticFieldWrapperMock, 0);
      var expected2 = Tuple.Create (_staticFieldWrapperMock, 1);

      Assert.That (actual1, Is.EqualTo (expected1));
      Assert.That (actual2, Is.EqualTo (expected2));
    }

    [Test]
    public void GetStorageExpression_InstanceAspect ()
    {
      var container = new ExpressionGeneratorContainer (new[] { _instanceAspectDescriptorMock }, _instanceFieldWrapperMock, _staticFieldWrapperMock);

      var thisExpression = new ThisExpression (GetType());
      var fieldExpression = Expression.Field (thisExpression, _field);
      _instanceFieldWrapperMock
          .Expect (x => x.GetAccessExpression(thisExpression))
          .Return (fieldExpression);

      var actual = container.GetStorageExpression (_instanceAspectDescriptorMock, thisExpression);
      var expected = Expression.ArrayAccess (Expression.Field (thisExpression, _field), Expression.Constant (0));

      ExpressionTreeComparer.CheckAreEqualTrees (expected, actual);
    }

    [Test]
    public void GetInstanceAspectsInitExpression ()
    {
      var container = new ExpressionGeneratorContainer (_aspectDescriptors, _instanceFieldWrapperMock, _staticFieldWrapperMock);

      var dummyExpression = Expression.Default (typeof (AspectAttribute));
      var expected = Expression.NewArrayInit (typeof (AspectAttribute), dummyExpression);
      var actual = container.GetInstanceAspectsInitExpression();
    }
  }
}