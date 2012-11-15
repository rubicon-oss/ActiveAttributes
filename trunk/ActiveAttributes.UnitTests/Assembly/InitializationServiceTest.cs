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
using System.Text;
using ActiveAttributes.Advices;
using ActiveAttributes.Assembly;
using ActiveAttributes.Assembly.Storages;
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Assembly
{
  [TestFixture]
  public class InitializationServiceTest
  {
    private InitializationService _initializationService;

    private IStorageService _storageServiceMock;
    private IInitializationExpressionHelper _expressionHelperMock;
    private IStorage _storageMock;

    private MutableType _mutableType;
    private Type _aspectType;
    private MethodInfo _adviceMethod;

    [SetUp]
    public void SetUp ()
    {
      _mutableType = ObjectMother.GetMutableType (typeof (DomainType));
      _storageServiceMock = MockRepository.GenerateStrictMock<IStorageService> ();
      _expressionHelperMock = MockRepository.GenerateStrictMock<IInitializationExpressionHelper>();
      _storageMock = MockRepository.GenerateStrictMock<IStorage>();
      _aspectType = ObjectMother.GetAspectType();
      _adviceMethod = ObjectMother.GetMethodInfo (declaringType: _aspectType);

      _initializationService = new InitializationService (_storageServiceMock, _expressionHelperMock);
    }

    [Test]
    public void GetOrAddAspect_Instance ()
    {
      var construction = ObjectMother.GetConstruction();
      var advice = ObjectMother.GetAdvice (construction, _adviceMethod, scope: AdviceScope.Instance);
      var fakeMemberInit = ObjectMother.GetMemberInitExpression (_aspectType);
      var fakeMember = ObjectMother.GetMemberExpression (_aspectType);

      _storageServiceMock.Expect (x => x.AddInstanceStorage (_mutableType, _aspectType, "aspect")).Return (_storageMock);
      _expressionHelperMock
          .Expect (x => x.CreateAspectInitExpression (construction))
          .Return (fakeMemberInit).Repeat.Twice ();
      _storageMock
          .Expect (x => x.GetStorageExpression (Arg<Expression>.Matches (y => y.Type.Equals (_mutableType))))
          .Return (fakeMember).Repeat.Twice();

      var result = _initializationService.GetOrAddAspect (advice, _mutableType);

      _storageServiceMock.VerifyAllExpectations();
      _expressionHelperMock.VerifyAllExpectations();
      _storageMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_storageMock));
      CheckInstanceInitialization (_mutableType, fakeMember, fakeMemberInit);
    }

    // TODO add tests for scope and declaredOn
    [Test]
    public void GetOrAddAspect_Caches ()
    {
      var construction = ObjectMother.GetConstruction();
      var advice1 = ObjectMother.GetAdvice (construction, _adviceMethod, scope: AdviceScope.Instance);
      var advice2 = ObjectMother.GetAdvice (construction, _adviceMethod, scope: AdviceScope.Instance);
      var fakeMemberInit = ObjectMother.GetMemberInitExpression (_aspectType);
      var fakeMember = ObjectMother.GetMemberExpression (_aspectType);

      _storageServiceMock.Expect (x => x.AddInstanceStorage (_mutableType, _aspectType, "aspect")).Return (_storageMock);
      _expressionHelperMock
          .Expect (x => x.CreateAspectInitExpression (construction))
          .Return (fakeMemberInit).Repeat.Twice();
      _storageMock
          .Expect (x => x.GetStorageExpression (Arg<Expression>.Matches (y => y.Type.Equals (_mutableType))))
          .Return (fakeMember).Repeat.Twice();

      var result1 = _initializationService.GetOrAddAspect (advice1, _mutableType);
      var result2 = _initializationService.GetOrAddAspect (advice2, _mutableType);

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetOrAddAspect_Static ()
    {
      var construction = ObjectMother.GetConstruction();
      var advice = ObjectMother.GetAdvice (construction, _adviceMethod, scope: AdviceScope.Static);
      var fakeMemberInit = ObjectMother.GetMemberInitExpression (_aspectType);
      var fakeMember = ObjectMother.GetMemberExpression (_aspectType);

      _storageServiceMock.Expect (x => x.AddStaticStorage (_mutableType, _aspectType, "aspect")).Return (_storageMock);
      _expressionHelperMock.Expect (x => x.CreateAspectInitExpression (construction)).Return (fakeMemberInit);
      _storageMock.Expect (x => x.GetStorageExpression (Arg<Expression>.Is.Null)).Return (fakeMember);

      var result = _initializationService.GetOrAddAspect (advice, _mutableType);

      _storageServiceMock.VerifyAllExpectations();
      _expressionHelperMock.VerifyAllExpectations();
      _storageMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_storageMock));
      CheckStaticInitialization (_mutableType, fakeMember, fakeMemberInit);
    }

    [Test]
    public void AddMemberInfo_MethodInfo ()
    {
      var method = _mutableType.AllMutableMethods.Single (x => x.Name == "Method");
      var fakeExpression = ObjectMother.GetConstantExpression (typeof (MethodInfo));
      var fakeMember = ObjectMother.GetMemberExpression (typeof (MethodInfo));

      _storageServiceMock.Expect (x => x.AddStaticStorage (_mutableType, typeof (MethodInfo), method.Name)).Return (_storageMock);
      _expressionHelperMock.Expect (x => x.CreateMethodInfoInitExpression (method)).Return (fakeExpression);
      _storageMock.Expect (x => x.GetStorageExpression (Arg<Expression>.Is.Null)).Return (fakeMember);

      var result = _initializationService.AddMemberInfo (method);

      _storageServiceMock.VerifyAllExpectations();
      _expressionHelperMock.VerifyAllExpectations();
      _storageMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_storageMock));
      CheckStaticInitialization (_mutableType, fakeMember, fakeExpression);
    }

    [Test]
    public void AddMemberInfo_PropertyInfo ()
    {
      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.Property);
      var method = _mutableType.AllMutableMethods.Single (x => x.Name == "get_Property");
      var fakeExpression = ObjectMother.GetMethodCallExpression (typeof(PropertyInfo));
      var fakeMember = ObjectMother.GetMemberExpression (typeof (PropertyInfo));

      _storageServiceMock.Expect (x => x.AddStaticStorage (_mutableType, typeof (PropertyInfo), method.Name)).Return (_storageMock);
      _expressionHelperMock.Expect (x => x.CreatePropertyInfoInitExpression (property)).Return (fakeExpression);
      _storageMock.Expect (x => x.GetStorageExpression (Arg<Expression>.Is.Null)).Return (fakeMember);

      var result = _initializationService.AddMemberInfo (method);

      _storageServiceMock.VerifyAllExpectations();
      _expressionHelperMock.VerifyAllExpectations();
      _storageMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (_storageMock));
      CheckStaticInitialization (_mutableType, fakeMember, fakeExpression);
    }

    [Test]
    public void GetDelegate ()
    {
      var method = _mutableType.AllMutableMethods.Single (x => x.Name == "Method");
      var fakeMemberInit = ObjectMother.GetNewDelegateExpression (method);
      var fakeMember = ObjectMother.GetMemberExpression (typeof (Action));

      _storageServiceMock.Expect (x => x.AddStaticStorage (_mutableType, typeof (Action), method.Name)).Return (_storageMock);
      _expressionHelperMock
          .Expect (x => x.CreateDelegateInitExpression (Arg.Is (method), Arg<Expression>.Matches (y => y.Type.Equals (_mutableType))))
          .Return (fakeMemberInit).Repeat.Twice();
      _storageMock
          .Expect (x => x.GetStorageExpression (Arg<Expression>.Matches (y => y.Type.Equals (_mutableType))))
          .Return (fakeMember).Repeat.Twice();

      var result = _initializationService.AddDelegate (method);

      _storageServiceMock.VerifyAllExpectations ();
      _expressionHelperMock.VerifyAllExpectations ();
      _storageMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (_storageMock));
      var mutableType = _mutableType;
      CheckInstanceInitialization(mutableType, fakeMember, fakeMemberInit);
    }

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<IInitializationService> ();

      Assert.That (instance, Is.TypeOf<InitializationService> ());
    }

    private static void CheckInstanceInitialization (MutableType mutableType, Expression member, Expression initialization)
    {
      foreach (var constructor in mutableType.AllMutableConstructors)
      {
        var body = constructor.Body;
        Assert.That (body, Is.InstanceOf<BlockExpression>());
        var blockExpression = (BlockExpression) (body);
        var expressions = blockExpression.Expressions;
        Assert.That (expressions[0], Is.TypeOf<OriginalBodyExpression>());
        Assert.That (expressions[1], Is.InstanceOf<BinaryExpression>());
        var binaryExpression = (BinaryExpression) expressions[1];
        Assert.That (binaryExpression.Left, Is.SameAs (member));
        Assert.That (binaryExpression.Right, Is.SameAs (initialization));
      }
    }

    private static void CheckStaticInitialization (MutableType mutableType, Expression member, Expression initialization)
    {
      Assert.That (mutableType.TypeInitializations, Has.Count.EqualTo (1));
      var expression = mutableType.TypeInitializations.Single();
      Assert.That (expression, Is.InstanceOf<BinaryExpression>());
      var binaryExpression = (BinaryExpression) expression;
      Assert.That (binaryExpression.Left, Is.SameAs (member));
      Assert.That (binaryExpression.Right, Is.SameAs (initialization));
    }

    class DomainType
    {
      public DomainType () {}
      public DomainType (string arg) {}

      public void Method () {}

      public string Property { get; set; }
    }
  }
}