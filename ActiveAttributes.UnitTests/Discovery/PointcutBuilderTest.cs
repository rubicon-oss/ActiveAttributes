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
using System.Text;
using ActiveAttributes.Aspects.Pointcuts;
using ActiveAttributes.Aspects.StrongContext;
using ActiveAttributes.Discovery;
using ActiveAttributes.Model;
using ActiveAttributes.Model.Pointcuts;
using ActiveAttributes.Weaving.Expressions;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Rhino.Mocks;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class PointcutBuilderTest
  {
    private PointcutBuilder _builder;

    private TypePointcutAttribute _typePointcutAttribute;
    private MemberNamePointcutAttribute _memberNamePointcutAttribute;
    private AndAttribute _andAttribute;
    private OrAttribute _orAttribute;
    private NotAttribute _notAttribute;

    private string _dummy;

    [SetUp]
    public void SetUp ()
    {
      _builder = new PointcutBuilder();

      _typePointcutAttribute = new TypePointcutAttribute (typeof (int));
      _memberNamePointcutAttribute = new MemberNamePointcutAttribute ("member");
      _andAttribute = new AndAttribute();
      _orAttribute = new OrAttribute();
      _notAttribute = new NotAttribute();
    }

    [Test]
    public void name ()
    {
      var expr = new MethodExecutionExpression (null);
      //var expr1 = new MethodExecutionExpression ()
    }

    [Test]
    public void GetPointcut_AttributeProvider_Simple ()
    {
      var pointcut = CheckPointcut<TypePointcut> (_typePointcutAttribute);
      Assert.That (pointcut.Type, Is.EqualTo (typeof (int)));
    }

    [Test]
    public void GetPointcut_AttributeProvider_None ()
    {
      CheckPointcut<TruePointcut>();
    }

    [Test]
    public void GetPointcut_AttributeProvider_Not ()
    {
      var pointcut = CheckPointcut<NotPointcut> (_typePointcutAttribute, _notAttribute);
      Assert.That (pointcut.Pointcut, Is.TypeOf<TypePointcut>());
    }

    [Test]
    public void GetPointcut_AttributeProvider_All ()
    {
      var pointcut = CheckPointcut<AllPointcut> (_typePointcutAttribute, _memberNamePointcutAttribute, _andAttribute);
      Assert.That (pointcut.Pointcuts, Has.Some.TypeOf<TypePointcut> ());
      Assert.That (pointcut.Pointcuts, Has.Some.TypeOf<MemberNamePointcut> ());
    }

    [Test]
    public void GetPointcut_AttributeProvider_Any ()
    {
      var pointcut = CheckPointcut<AnyPointcut> (_typePointcutAttribute, _memberNamePointcutAttribute, _orAttribute);
      Assert.That (pointcut.Pointcuts, Has.Some.TypeOf<TypePointcut> ());
      Assert.That (pointcut.Pointcuts, Has.Some.TypeOf<MemberNamePointcut> ());
    }

    [Test]
    public void GetPointcut_AttributeProvider_MethodPointcut ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Advice());
      var pointcut1 = CheckPointcut<MethodPointcut> (typeof (DomainType));
      var pointcut2 = CheckPointcut<MethodPointcut> (advice as ICustomAttributeProvider);

      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => DomainType.Method (null));
      Assert.That (pointcut1.Method, Is.EqualTo (method));
      Assert.That (pointcut2.Method, Is.EqualTo (method));
    }

    [Test]
    public void GetPointcut_StrongContext_Instance ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Instance (ref _dummy));

      var pointcut = CheckPointcut<TypePointcut> (advice);
      Assert.That (pointcut.Type, Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetPointcut_StrongContext_ReturnValue ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.ReturnValue (ref _dummy));

      var pointcut = CheckPointcut<ReturnTypePointcut> (advice);
      Assert.That (pointcut.ReturnType, Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetPointcut_StrongContext_Parameter ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Parameter (ref _dummy));

      var pointcut = CheckPointcut<ArgumentPointcut> (advice);
      Assert.That (pointcut.ArgumentType, Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetPointcut_StrongContext_ParameterIndex ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.ParameterIndex (ref _dummy));

      var pointcut = CheckPointcut<ArgumentIndexPointcut> (advice);
      Assert.That (pointcut.ArgumentType, Is.EqualTo (typeof (string)));
      Assert.That (pointcut.Index, Is.EqualTo (1));
    }

    [Test]
    public void GetPointcut_AttributeProvider_StrongContext ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.AttributeAndContext (ref _dummy));

      var pointcut = CheckPointcut<AllPointcut> (advice);
      Assert.That (pointcut.Pointcuts, Has.Length.EqualTo (2));
      Assert.That (pointcut.Pointcuts, Has.Some.TypeOf<TypePointcut>());
      Assert.That (pointcut.Pointcuts, Has.Some.TypeOf<MemberNamePointcut>());
    }

    private T CheckPointcut<T> (MethodInfo method)
    {
      var result = _builder.Build (method);

      Assert.That (result, Is.TypeOf<T>());
      return (T) result;
    }

    private T CheckPointcut<T> (ICustomAttributeProvider customAttributeProvider)
    {
      var result = _builder.Build (customAttributeProvider);

      Assert.That (result, Is.TypeOf<T>());
      return (T) result;
    }

    private T CheckPointcut<T> (params object[] attributes)
    {
      var customAttributeProvider = MockRepository.GenerateStrictMock<ICustomAttributeProvider>();
      customAttributeProvider.Expect (x => x.GetCustomAttributes (true)).Return (attributes);

      var result = _builder.Build (customAttributeProvider);

      Assert.That (result, Is.TypeOf<T>());
      return (T) result;
    }

    [MethodPointcut ("Method")]
    class DomainType
    {
      [MethodPointcut ("Method")]
      public void Advice () {}

      [MethodPointcut ("Method2")]
      public void OtherAdvice () {}

      public static bool Method (JoinPoint joinPoint)
      {
        return false;
      }

      public void Instance ([Instance] ref string instance) {}
      public void ReturnValue ([ReturnValue] ref string returnValue) {}
      public void Parameter ([Parameter] ref string arg) {}
      public void ParameterIndex ([Parameter (1)] ref string arg1) {}

      [MemberNamePointcut("Member")]
      public void AttributeAndContext ([Instance] ref string instance) {}
    }
  }
}