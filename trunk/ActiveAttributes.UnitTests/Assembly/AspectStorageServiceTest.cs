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
using ActiveAttributes.Core.Assembly;
using ActiveAttributes.Core.Assembly.FieldWrapper;
using ActiveAttributes.Core.Attributes.Aspects;
using ActiveAttributes.Core.Infrastructure;
using ActiveAttributes.Core.Infrastructure.AdviceInfo;
using ActiveAttributes.Core.Infrastructure.Construction;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class AspectStorageServiceTest
  {
    private IFieldService _fieldServiceMock;
    private IAspectInitializationExpressionHelper _expressionHelperMock;
    private IFieldWrapper _fieldWrapperMock;

    private AspectStorageService _aspectStorageService;
    private MutableType _mutableType;

    [SetUp]
    public void SetUp ()
    {
      _mutableType = ObjectMother2.GetMutableType ();
      _fieldServiceMock = MockRepository.GenerateStrictMock<IFieldService> ();
      _expressionHelperMock = MockRepository.GenerateStrictMock<IAspectInitializationExpressionHelper>();
      _fieldWrapperMock = MockRepository.GenerateStrictMock<IFieldWrapper>();

      _aspectStorageService = new AspectStorageService (_fieldServiceMock, _expressionHelperMock);
    }

    [Test]
    public void GetOrAddAspect_Instance ()
    {
      var adviceScope = AdviceScope.Instance;
      var fieldAttributes = FieldAttributes.Private;

      CheckGetOrAddAspect(fieldAttributes, adviceScope, ObjectMother2.GetConstruction());
    }

    [Test]
    public void GetOrAddAspect_Static ()
    {
      var adviceScope = AdviceScope.Static;
      var fieldAttributes = FieldAttributes.Private | FieldAttributes.Static;

      CheckGetOrAddAspect(fieldAttributes, adviceScope, ObjectMother2.GetConstruction());
    }

    [Test]
    public void GetOrAddAspect_ChachesOnConstructionInfoAndScope ()
    {
      var construction = ObjectMother2.GetConstruction();
      var scope = AdviceScope.Instance;

      CheckGetOrAddAspect (FieldAttributes.Private, scope, construction);

      var secondAdvice = ObjectMother2.GetAdvice (construction, scope: scope);
      var result = _aspectStorageService.GetOrAdd (secondAdvice, _mutableType);

      Assert.That (result, Is.SameAs (_fieldWrapperMock));
    }

    private void CheckGetOrAddAspect (FieldAttributes fieldAttributes, AdviceScope adviceScope, IConstruction construction)
    {
      var method = ObjectMother2.GetMethodInfo (declaringType: typeof (IAspect));
      var advice = ObjectMother2.GetAdvice (method: method, construction: construction, scope: adviceScope);
      var fakeMember = ObjectMother2.GetMemberExpression (typeof (IAspect));
      var fakeMemberInit = ObjectMother2.GetMemberInitExpression (typeof (AspectAttributeBase));

      _fieldServiceMock.Expect (x => x.AddField (_mutableType, typeof (IAspect), "TODO fieldname", fieldAttributes)).Return (_fieldWrapperMock);
      _expressionHelperMock.Expect (x => x.CreateInitExpression (construction)).Return (fakeMemberInit);
      _fieldWrapperMock.Expect (x => x.GetMemberExpression (Arg<Expression>.Matches (y => y.Type == _mutableType))).Return (fakeMember);

      var result = _aspectStorageService.GetOrAdd (advice, _mutableType);

      _fieldServiceMock.VerifyAllExpectations();
      _expressionHelperMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_fieldWrapperMock));

      foreach (var constructor in _mutableType.AllMutableConstructors)
      {
        Assert.That (constructor.Body, Is.InstanceOf<BlockExpression> ());
        var blockExpression = (BlockExpression) constructor.Body;
        var expressions = blockExpression.Expressions;
        Assert.That (expressions[0], Is.TypeOf<OriginalBodyExpression> ());
        Assert.That (expressions[1], Is.InstanceOf<BinaryExpression> ());
        var binaryExpression = (BinaryExpression) expressions[1];
        Assert.That (binaryExpression.Left, Is.SameAs (fakeMember));
        Assert.That (binaryExpression.Right, Is.SameAs (fakeMemberInit));
      }
    }
  }
}