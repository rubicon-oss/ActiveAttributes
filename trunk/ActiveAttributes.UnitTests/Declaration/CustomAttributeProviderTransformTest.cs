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
using System.Reflection;
using ActiveAttributes.Advices;
using ActiveAttributes.Assembly;
using ActiveAttributes.Declaration;
using ActiveAttributes.Pointcuts;
using ActiveAttributes.Weaving.Construction;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Declaration
{
  [TestFixture]
  public class CustomAttributeProviderTransformTest
  {
    private CustomAttributeProviderTransform _transform;

    private IAdviceBuilder _builderMock;
    private IAdviceBuilderFactory _adviceBuilderFactoryMock;

    [SetUp]
    public void SetUp ()
    {
      _builderMock = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      _adviceBuilderFactoryMock = MockRepository.GenerateStrictMock<IAdviceBuilderFactory>();

      _transform = new CustomAttributeProviderTransform (_adviceBuilderFactoryMock);
    }

    [Test]
    public void GetAdviceBuilder ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainAspect obj) => obj.Advice1());

      _adviceBuilderFactoryMock.Expect (x => x.Create()).Return (_builderMock);
      _builderMock.Expect (x => x.SetMethod (method)).Return (_builderMock);
      _builderMock.Expect (x => x.SetName ("Name")).Return (_builderMock);
      _builderMock.Expect (x => x.SetRole ("Role")).Return (_builderMock);
      _builderMock.Expect (x => x.SetExecution (AdviceExecution.Around)).Return (_builderMock);
      _builderMock.Expect (x => x.SetScope (AdviceScope.Instance)).Return (_builderMock);
      _builderMock.Expect (x => x.SetPriority (10)).Return (_builderMock);

      var result = _transform.GetAdviceBuilder (method);

      Assert.That (result, Is.SameAs (_builderMock));
      _builderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetAdviceBuilder_Pointcuts ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainAspect obj) => obj.Advice2());

      _adviceBuilderFactoryMock.Stub (x => x.Create()).Return (_builderMock);
      _builderMock.Stub (x => x.SetMethod (method)).Return (_builderMock);
      _builderMock.Expect (x => x.AddPointcut (Arg<TypePointcut>.Is.TypeOf)).Return (_builderMock);
      _builderMock.Expect (x => x.AddPointcut (Arg<MemberNamePointcut>.Is.TypeOf)).Return (_builderMock);

      _transform.GetAdviceBuilder (method);

      var args = _builderMock.GetArgumentsForCallsMadeOn (x => x.AddPointcut (null));
      var pointcuts = new[] { (IPointcut) args[0][0], (IPointcut) args[1][0] };
      Assert.That (pointcuts, Has.Some.TypeOf<TypePointcut>().With.Property ("Type").EqualTo (typeof (string)));
      Assert.That (pointcuts, Has.Some.TypeOf<MemberNamePointcut>().With.Property ("MemberName").EqualTo ("MemberName"));
      _builderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetAdviceBuilder_Pointcuts_MethodPointcut ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainAspect obj) => obj.Advice4());

      _adviceBuilderFactoryMock.Stub (x => x.Create()).Return (_builderMock);
      _builderMock.Stub (x => x.SetMethod (method)).Return (_builderMock);
      _builderMock.Expect (x => x.AddPointcut (Arg<MethodPointcut>.Is.TypeOf)).Return (_builderMock);

      _transform.GetAdviceBuilder (method);

      var args = _builderMock.GetArgumentsForCallsMadeOn (x => x.AddPointcut (null));
      var methodPointcut = (MethodPointcut) args[0][0];
      var pointcutMethod = typeof (DomainAspect).GetMethod ("PointcutMethod");
      Assert.That (methodPointcut.Method, Is.EqualTo (pointcutMethod));
      _builderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetAdviceBuilder_Pointcuts_MethodPointcut_ThrowsForNotStatic ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainAspect obj) => obj.AdviceWithInstanceMethodPointcut());

      CheckMethodPointcut (method, "Pointcut method 'NotStaticPointcutMethod' is missing or not declared as static.");
    }

    [Test]
    public void GetAdviceBuilder_Pointcuts_MethodPointcut_ThrowsForWrongSignature ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainAspect obj) => obj.AdviceWithWrongSignature());

      CheckMethodPointcut (method, "Pointcut method 'WrongSignaturePointcutMethod' must have JoinPoint as argument and bool as return type.");
    }

    [Test]
    public void GetAdviceBuilder_Pointcuts_MethodPointcut_ThrowsForMissing ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainAspect obj) => obj.AdviceWithMissingMethodPointcut());

      CheckMethodPointcut (method, "Pointcut method 'MissingPointcutMethod' is missing or not declared as static.");
    }

    private void CheckMethodPointcut (MethodInfo method, string message)
    {
      _adviceBuilderFactoryMock.Stub (x => x.Create()).Return (_builderMock);
      _builderMock.Stub (x => x.SetMethod (method)).Return (_builderMock);

      Assert.That (() => _transform.GetAdviceBuilder (method), Throws.InvalidOperationException.With.Message.EqualTo (message));
    }

    [Test]
    public void GetAdviceBuilder_SetConstructionIfType ()
    {
      var type = typeof (DomainAspect);

      _builderMock.Expect (x => x.SetConstruction (Arg<TypeConstruction>.Is.TypeOf)).Return (_builderMock);
      _builderMock.Expect (x => x.SetMethod (null)).Return (_builderMock);

      _adviceBuilderFactoryMock.Expect (x => x.Create()).Return (_builderMock);
      _transform.GetAdviceBuilder (type);

      var args = _builderMock.GetArgumentsForCallsMadeOn (x => x.SetConstruction (null));
      Assert.That (((IAspectConstruction) args[0][0]).ConstructorInfo.DeclaringType, Is.EqualTo (typeof (DomainAspect)));
      _builderMock.VerifyAllExpectations();
    }

    [Test]
    public void GetAdviceBuilder_CopyParent ()
    {
      var type = typeof (DomainAspect);
      var parentAdviceBuilderMock = MockRepository.GenerateStrictMock<IAdviceBuilder>();
      var builderStub = MockRepository.GenerateStub<IAdviceBuilder>();

      parentAdviceBuilderMock.Expect (x => x.Copy()).Return (builderStub);

      var result = _transform.GetAdviceBuilder (type, parentAdviceBuilderMock);

      Assert.That (result, Is.SameAs (builderStub));
    }

    [Test]
    public void GetAdviceBuilder_ConsidersDerivedAttributes ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DerivedDomainAspect obj) => obj.Advice3());
      
      var adviceBuilderMock = MockRepository.GenerateMock<IAdviceBuilder>();
      _adviceBuilderFactoryMock.Expect (x => x.Create ()).Return (adviceBuilderMock);

      _transform.GetAdviceBuilder (method);

      adviceBuilderMock.AssertWasCalled (x => x.SetName ("Name"));
      adviceBuilderMock.AssertWasCalled (x => x.AddPointcut (Arg<ReturnTypePointcut>.Is.Anything));
      adviceBuilderMock.AssertWasNotCalled (x => x.SetPriority (0));
    }

    [Test]
    public void Resolution ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<ICustomAttributeProviderTransform>();

      Assert.That (instance, Is.TypeOf<CustomAttributeProviderTransform>());
    }

    class DomainAspect : IAspect
    {
      [AdviceInfo (
          Name = "Name",
          Role = "Role",
          Execution = AdviceExecution.Around,
          Scope = AdviceScope.Instance,
          Priority = 10)]
      public void Advice1 () {}

      [MemberNamePointcut ("MemberName")]
      [TypePointcut (typeof (string))]
      public void Advice2 () {}

      [AdviceInfo (Name = "Name")]
      [ReturnTypePointcut (typeof (string))]
      public virtual void Advice3 () {}

      [MethodPointcut ("PointcutMethod")]
      public void Advice4 () {}

      public static bool PointcutMethod (JoinPoint joinPoint) { return true; }

      [MethodPointcut ("NotStaticPointcutMethod")]
      public void AdviceWithInstanceMethodPointcut () {}

      public bool NotStaticPointcutMethod () { return true; }

      [MethodPointcut ("WrongSignaturePointcutMethod")]
      public void AdviceWithWrongSignature () {}

      public static void WrongSignaturePointcutMethod () { }

      [MethodPointcut ("MissingPointcutMethod")]
      public void AdviceWithMissingMethodPointcut () {}
    }

    class DerivedDomainAspect : DomainAspect
    {
      public override void Advice3 () {}
    }
  }
}