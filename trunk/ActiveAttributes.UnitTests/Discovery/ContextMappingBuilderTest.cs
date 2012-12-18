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
using ActiveAttributes.Aspects;
using ActiveAttributes.Aspects.StrongContext;
using ActiveAttributes.Discovery;
using ActiveAttributes.Model.Pointcuts;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using System.Linq;

namespace ActiveAttributes.UnitTests.Discovery
{
  [TestFixture]
  public class ContextMappingBuilderTest
  {
    private ContextMappingBuilder _builder;

    private string _dummy;

    [SetUp]
    public void SetUp ()
    {
      _builder = new ContextMappingBuilder();
    }

    [Test]
    public void GetMappingsAndPointcut_Instance ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Instance (ref _dummy));

      var pointcut = CheckMappingAndGetPointcut<TypePointcut> (advice, fieldName: "TypedInstance");
      Assert.That (pointcut.Type, Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetMappingsAndPointcut_ReturnValue ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.ReturnValue (ref _dummy));

      var pointcut = CheckMappingAndGetPointcut<ReturnTypePointcut> (advice, fieldName: "TypedReturnValue");
      Assert.That (pointcut.ReturnType, Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetMappingsAndPointcut_Parameter ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Parameter (ref _dummy));

      var pointcut = CheckMappingAndGetPointcut<ArgumentPointcut> (advice, fieldType: typeof (string));
      Assert.That (pointcut.ArgumentType, Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetMappingsAndPointcut_ParameterIndex ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.ParameterIndex2 (ref _dummy));

      var pointcut = CheckMappingAndGetPointcut<ArgumentIndexPointcut> (advice, fieldName: "Arg" + 2);
      Assert.That (pointcut.ArgumentType, Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetMappingsAndPointcut_Invocation ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Invocation (null));

      CheckMappingAndGetPointcut<TruePointcut> (advice, noField: true);
    }

    [Test]
    public void GetMappingsAndPointcut_Context ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Context (null));

      CheckMappingAndGetPointcut<TruePointcut> (advice, noField: true);
    }

    [Test]
    public void GetMappingsAndPointcut_AllPointcut ()
    {
      var advice = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Advice (ref _dummy, ref _dummy, ref _dummy));

      var result = _builder.GetMappingsAndPointcut (advice);
      var allPointcuts = ((AllPointcut)result.Item2).Pointcuts;

      Assert.That (allPointcuts, Has.Some.TypeOf<TypePointcut>());
      Assert.That (allPointcuts, Has.Some.TypeOf<ArgumentIndexPointcut>());
      Assert.That (allPointcuts, Has.Some.TypeOf<ReturnTypePointcut>());
    }

    private T CheckMappingAndGetPointcut<T> (MethodInfo advice, string fieldName = "field", Type fieldType = null, bool noField = false)
    {
      fieldType = fieldType ?? ObjectMother.GetDeclaringType();

      var result = _builder.GetMappingsAndPointcut (advice);
      var mapping = result.Item1.Single();
      var pointcut = result.Item2;

      Assert.That (mapping (ObjectMother.GetFieldInfo (name: fieldName, type: fieldType)), Is.Not.EqualTo (noField));
      Assert.That (mapping (ObjectMother.GetFieldInfo (name: "AnotherField", type: typeof (bool))), Is.False);
      Assert.That (pointcut, Is.TypeOf<AllPointcut>());
      return (T) ((AllPointcut) pointcut).Pointcuts.Single();
    }

    private class DomainType
    {
      public void Instance ([Instance] ref string instance) {}

      public void ReturnValue ([ReturnValue] ref string returnValue) {}

      public void Parameter ([Parameter] ref string parameter) {}

      public void ParameterIndex2 ([Parameter (2)] ref string parameter) {}

      public void Invocation (IInvocation invocation) {}

      public void Context (IContext context) {}

      public void Advice ([Instance] ref string instance, [Parameter (1)] ref string arg1, [ReturnValue] ref string returnValue) {}
    }
  }
}