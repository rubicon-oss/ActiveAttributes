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
using ActiveAttributes.Aspects.Pointcuts;
using ActiveAttributes.Discovery;
using ActiveAttributes.Model;
using ActiveAttributes.Model.Pointcuts;
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
    public void GetPointcut_AttributeProvider_Simple ()
    {
      var pointcut = GetPointcut (_typePointcutAttribute);

      Assert.That (pointcut, Is.TypeOf<TypePointcut>());
    }

    [Test]
    public void GetPointcut_AttributeProvider_None ()
    {
      var pointcut = GetPointcut();

      Assert.That (pointcut, Is.TypeOf<TruePointcut>());
    }

    [Test]
    public void GetPointcut_AttributeProvider_Not ()
    {
      var pointcut = GetPointcut (_typePointcutAttribute, _notAttribute);

      Assert.That (pointcut, Is.TypeOf<NotPointcut>());
      var notPointcut = (NotPointcut) pointcut;
      Assert.That (notPointcut.Pointcut, Is.TypeOf<TypePointcut>());
    }

    [Test]
    public void GetPointcut_AttributeProvider_All ()
    {
      var pointcut = GetPointcut (_typePointcutAttribute, _memberNamePointcutAttribute, _andAttribute);

      Assert.That (pointcut, Is.TypeOf<AllPointcut>());
      var allPointcut = (AllPointcut) pointcut;
      Assert.That (allPointcut.Pointcuts, Has.Some.TypeOf<TypePointcut> ());
      Assert.That (allPointcut.Pointcuts, Has.Some.TypeOf<MemberNamePointcut> ());
    }

    [Test]
    public void GetPointcut_AttributeProvider_Any ()
    {
      var pointcut = GetPointcut (_typePointcutAttribute, _memberNamePointcutAttribute, _orAttribute);

      Assert.That (pointcut, Is.TypeOf<AnyPointcut>());
      var allPointcut = (AnyPointcut) pointcut;
      Assert.That (allPointcut.Pointcuts, Has.Some.TypeOf<TypePointcut> ());
      Assert.That (allPointcut.Pointcuts, Has.Some.TypeOf<MemberNamePointcut> ());
    }

    [Test]
    public void GetPointcut_AttributeProvider_MethodPointcut ()
    {
      var pointcut1 = _builder.Build (typeof (DomainType));
      var pointcut2 = _builder.Build (NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Advice()));

      Assert.That (pointcut1, Is.TypeOf<MethodPointcut>());
      Assert.That (pointcut2, Is.TypeOf<MethodPointcut>());
      var methodPointcut1 = ((MethodPointcut) pointcut1);
      var methodPointcut2 = ((MethodPointcut) pointcut2);
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => DomainType.Method (null));
      Assert.That (methodPointcut1.Method, Is.EqualTo (method));
      Assert.That (methodPointcut2.Method, Is.EqualTo (method));
    }

    private IPointcut GetPointcut (params object[] attributes)
    {
      var customAttributeProvider = MockRepository.GenerateStrictMock<ICustomAttributeProvider>();
      customAttributeProvider.Expect (x => x.GetCustomAttributes (true)).Return (attributes);

      return _builder.Build (customAttributeProvider);
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
    }
  }
}